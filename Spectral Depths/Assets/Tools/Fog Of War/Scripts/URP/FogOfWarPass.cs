using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FOW
{
    public class FogOfWarPass : ScriptableRenderPass
    {
        public FilterMode filterMode { get; set; }
        //public FogOfWarRenderFeature.FOWURPSettings settings;

        RenderTargetIdentifier source;
        RenderTargetIdentifier destination;

        int temporaryRTId = Shader.PropertyToID("_FowTempRT");
        int sourceId;
        int destinationId;

        string m_ProfilerTag;
        public FogOfWarPass(string tag)
        {
            m_ProfilerTag = tag;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor blitTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            //blitTargetDescriptor.depthBufferBits = 0;

            var renderer = renderingData.cameraData.renderer;

            sourceId = -1;
#if UNITY_2022_2_OR_NEWER
            source = renderer.cameraColorTargetHandle;
#else
            source = renderer.cameraColorTarget;
#endif
            destinationId = temporaryRTId;
            cmd.GetTemporaryRT(destinationId, blitTargetDescriptor, filterMode);
            destination = new RenderTargetIdentifier(destinationId);
            //ConfigureInput(ScriptableRenderPassInput.Normal);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (FogOfWarWorld.instance == null || !FogOfWarWorld.instance.enabled)
            {
                //Debug.Log("returning");
                return;
            }
            if (renderingData.cameraData.camera.GetUniversalAdditionalCameraData().renderType == CameraRenderType.Overlay)
                return;


            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            renderingData.cameraData.camera.depthTextureMode = DepthTextureMode.DepthNormals;

            if (!FogOfWarWorld.instance.is2D)
            {
                Matrix4x4 camToWorldMatrix = renderingData.cameraData.camera.cameraToWorldMatrix;

                //Matrix4x4 projectionMatrix = renderingData.cameraData.camera.projectionMatrix;
                //Matrix4x4 inverseProjectionMatrix = GL.GetGPUProjectionMatrix(projectionMatrix, true).inverse;

                //inverseProjectionMatrix[1, 1] *= -1;

                FogOfWarWorld.instance.FogOfWarMaterial.SetMatrix("_camToWorldMatrix", camToWorldMatrix);
                //FogOfWarWorld.instance.fowMat.SetMatrix("_inverseProjectionMatrix", inverseProjectionMatrix);
            }
            else
            {
                FogOfWarWorld.instance.FogOfWarMaterial.SetFloat("_cameraSize", renderingData.cameraData.camera.orthographicSize);
                FogOfWarWorld.instance.FogOfWarMaterial.SetVector("_cameraPosition", renderingData.cameraData.camera.transform.position);
                FogOfWarWorld.instance.FogOfWarMaterial.SetFloat("_cameraRotation", Mathf.DeltaAngle(0, renderingData.cameraData.camera.transform.eulerAngles.z));
            }

            // Can't read and write to same color target, create a temp render target to blit. 
            cmd.Blit(source, destination, FogOfWarWorld.instance.FogOfWarMaterial, 0);
            cmd.Blit(destination, source);
            //Blit(cmd, source, destination, FogOfWarWorld.instance.FogOfWarMaterial, 0);
            //Blit(cmd, destination, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (destinationId != -1)
                cmd.ReleaseTemporaryRT(destinationId);

            if (source == destination && sourceId != -1)
                cmd.ReleaseTemporaryRT(sourceId);
        }
    }
}
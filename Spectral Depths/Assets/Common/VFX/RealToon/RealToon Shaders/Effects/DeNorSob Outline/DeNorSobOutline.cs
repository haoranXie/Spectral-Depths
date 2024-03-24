//RealToon - DeNorSob Outline Effect (URP - Post Processing)
//MJQStudioWorks
//2023

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.Universal;

public class DeNorSobOutline : ScriptableRendererFeature
{
    [Reload("Hidden/URP/RealToon/Effects/DeNorSobOutline")]
    public static Material m_Mat;

    [System.Serializable]
        public class DeNorSobOutlineSettings
        {
            [Space(10)]
            [Tooltip("How thick or thin the outline is.")]
            [Min(0)]
            public float OutlineWidth = 1.0f;

            [Space(10)]

            [Header("**Depth and Normal Based Outline**")]
            [Space(20)]
            [Tooltip("This will adjust the depth based outline threshold.")]
            public float DepthThreshold = 900.0f;

            [Space(10)]

            [Tooltip("This will adjust the normal based outline threshold.")]
            public float NormalThreshold = 1.3f;
            [Tooltip("This will adjust the min of the normal to get more normal based outline details.")]
            public float NormalMin = 1.0f;
            [Tooltip("This will adjust the max of the normal to get more normal based outline details.")]
            public float NormalMax = 1.0f;

            [Space(10)]

            [Header("**Sobel Outline**")]
            [Space(20)]
            [Tooltip("This will render outline all on the screen")]
            public bool SobelOutline = false;

            [Tooltip("This will adjust the sobel threshold.\n\n*Sobel Outline is needed to be enabled for this to work.")]
            [Min(0)]
            public float SobelOutlineThreshold = 50.0f;

            [Space(6)]

            [Tooltip("The amount of whites or bright colors to be affected by the outline.\n\n*Sobel Outline is needed to be enabled for this to work.")]
            public float WhiteThreshold = 0.0f;

            [Tooltip("The amount of blacks or dark colors to be affected by the outline.\n\n*Sobel Outline is needed to be enabled for this to work.")]
            public float BlackThreshold = 0.0f;

            [Space(10)]

            [Header("**Color**")]
            [Tooltip("Outline Color")]
            [Space(20)]
            public Color OutlineColor = Color.black;

            [Tooltip("How strong the outline color is.")]
            public float ColorIntensity = 1.0f;

            [Tooltip("Mix full screen color image to the outline color.")]
            public bool MixFullScreenColor = false;

            [Space(10)]

            [Header("**Settings**")]
            [Space(20)]
            [Tooltip("Show the outline only.")]
            public bool ShowOutlineOnly = false;

            [Tooltip("Mix Depth-Normal Based Outline and Sobel Outline.")]
            public bool MixDephNormalAndSobelOutline = false;

            [Space(10)]

            [Tooltip("Where to insert or inject the DeNorSob Outline.")]
            public RenderPassEvent WhenToInsert = RenderPassEvent.BeforeRenderingTransparents;

            [Space(10)]

            [Tooltip("Show the DeNorSob Outline in the scene view panel.")]
            public bool ShowInSceneView = true;

    }

    public class DeNorSobOutlinePass : ScriptableRenderPass
    {

        private ProfilingSampler _profilingSampler;

        private ShaderTagId[] shaderTagsList = { new ShaderTagId ("SRPDefaultUnlit"), new ShaderTagId("UniversalForward"), new ShaderTagId("UniversalForwardOnly") };
        private RTHandle rtCustomColor, rtTempColor;

        public Material DeNorSobOutlineMat;

        public DeNorSobOutlinePass(string profilerTag, RenderPassEvent renderPassEvent ,
            float _OutlineWidth,
            float _DepthThreshold,
            float _NormalThreshold,
            float _NormalMin,
            float _NormalMax,

            bool _SobOutSel,
            float _SobelOutlineThreshold,
            float _WhiThres,
            float _BlaThres,

            Color _OutlineColor,
            float _OutlineColorIntensity,
            bool _ColOutMiSel,

            bool _OutOnSel,
            bool _MixDeNorSob
            )
        {

            DeNorSobOutlineMat = new Material(Shader.Find("Hidden/URP/RealToon/Effects/DeNorSobOutline"));

            this.renderPassEvent = renderPassEvent;

            DeNorSobOutlineMat.SetFloat("_OutlineWidth", _OutlineWidth);
            DeNorSobOutlineMat.SetFloat("_DepthThreshold", _DepthThreshold);

            DeNorSobOutlineMat.SetFloat("_NormalThreshold", _NormalThreshold);
            DeNorSobOutlineMat.SetFloat("_NormalMin", _NormalMin);
            DeNorSobOutlineMat.SetFloat("_NormalMax", _NormalMax);

            DeNorSobOutlineMat.SetFloat("_SobOutSel", _SobOutSel ? 1 : 0);
            DeNorSobOutlineMat.SetFloat("_SobelOutlineThreshold", _SobelOutlineThreshold);
            DeNorSobOutlineMat.SetFloat("_WhiThres", (1.0f - _WhiThres));
            DeNorSobOutlineMat.SetFloat("_BlaThres", _BlaThres);

            DeNorSobOutlineMat.SetColor("_OutlineColor", _OutlineColor);
            DeNorSobOutlineMat.SetFloat("_OutlineColorIntensity", _OutlineColorIntensity);
            DeNorSobOutlineMat.SetFloat("_ColOutMiSel", _ColOutMiSel ? 1 : 0);

            DeNorSobOutlineMat.SetFloat("_OutOnSel", _OutOnSel ? 1 : 0);

            DeNorSobOutlineMat.SetFloat("_MixDeNorSob", _MixDeNorSob ? 1 : 0);

            m_Mat = DeNorSobOutlineMat;

            switch (_SobOutSel)
            {
                case true:
                    DeNorSobOutlineMat.EnableKeyword("RENDER_OUTLINE_ALL");
                    break;
                default:
                    DeNorSobOutlineMat.DisableKeyword("RENDER_OUTLINE_ALL");
                    break;
            }

            switch (_MixDeNorSob)
            {
                case true:
                    DeNorSobOutlineMat.EnableKeyword("MIX_DENOR_SOB");
                    break;
                default:
                    DeNorSobOutlineMat.DisableKeyword("MIX_DENOR_SOB");
                    break;
            }

        }

        internal bool Setup(ScriptableRenderer renderer)
        {

            ConfigureInput(ScriptableRenderPassInput.Normal);

            return true;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor colorDesc = renderingData.cameraData.cameraTargetDescriptor;
            colorDesc.depthBufferBits = 0;

            RenderingUtils.ReAllocateIfNeeded(ref rtTempColor, colorDesc, name: "_TemporaryColorTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, _profilingSampler))
            {

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                RTHandle camTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

                RendererListDesc desc = new RendererListDesc(shaderTagsList, renderingData.cullResults, renderingData.cameraData.camera);
                RendererList rendererList = context.CreateRendererList(desc);
                cmd.DrawRendererList(rendererList);

                Blitter.BlitTexture(cmd, camTarget, rtTempColor, m_Mat, 0);
                Blitter.BlitCameraTexture(cmd, rtTempColor, camTarget);

            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public void Dispose()
        {
            rtTempColor?.Release();
        }

    }

    public DeNorSobOutlineSettings settings = new DeNorSobOutlineSettings();
    public DeNorSobOutlinePass DeNorSobOutlinepass;

    public override void Create()
    {

        this.name = "DeNorSob Outline";
        DeNorSobOutlinepass = new DeNorSobOutlinePass(
          "DeNorSob Outline",
          settings.WhenToInsert,
          settings.OutlineWidth,
          settings.DepthThreshold,
          settings.NormalThreshold,
          settings.NormalMin,
          settings.NormalMax,
          settings.SobelOutline,
          settings.SobelOutlineThreshold,
          settings.WhiteThreshold,
          settings.BlackThreshold,
          settings.OutlineColor,
          settings.ColorIntensity,
          settings.MixFullScreenColor,
          settings.ShowOutlineOnly,
          settings.MixDephNormalAndSobelOutline
        );

    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        CameraType cameraType = renderingData.cameraData.cameraType;
        
        if (!settings.ShowInSceneView && cameraType == CameraType.SceneView) return;
        
        bool shouldAdd = DeNorSobOutlinepass.Setup(renderer);
        if (shouldAdd)
        {
           renderer.EnqueuePass(DeNorSobOutlinepass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(m_Mat);
        DeNorSobOutlinepass.Dispose();
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace FOW
{
    public class FogOfWarRenderFeature : ScriptableRendererFeature
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingSkybox;
        FogOfWarPass fowPass;
        public override void Create()
        {
            fowPass = new FogOfWarPass(name);
        }
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            fowPass.renderPassEvent = renderPassEvent;
            fowPass.ConfigureInput(ScriptableRenderPassInput.Normal);
            renderer.EnqueuePass(fowPass);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FOW
{
    public class MiniMapZoomable : MonoBehaviour
    {
        public int ResolutionX = 256;
        public int ResolutionY = 256;
        public UnityEngine.UI.RawImage RawImageComponent;

        private Material blitMaterial;
        private RenderTexture Minimap_RT;
        public RenderTexture GetMiniMapRT() { return Minimap_RT; }
        private Vector4 _worldBounds;

        private void Start()
        {
            //Debug.Log(blitMaterial.shader.name);
            blitMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
            blitMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
            blitMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            blitMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            blitMaterial.SetInt("_ZWrite", 0);
            blitMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
            //blitMaterial = new Material(Shader.Find("Sprites/Default"));
            InitMinimapRT();
        }

        void InitMinimapRT()
        {
            Minimap_RT = new RenderTexture(ResolutionX, ResolutionY, 0);
            Minimap_RT.format = RenderTextureFormat.ARGBHalf;
            Minimap_RT.antiAliasing = 8;
            Minimap_RT.filterMode = FilterMode.Trilinear;
            Minimap_RT.anisoLevel = 9;
            Minimap_RT.Create();
            RenderTexture.active = Minimap_RT;
            GL.Begin(GL.TRIANGLES);
            GL.Clear(true, true, new Color(0, 0, 0, 0));
            GL.End();
            if (RawImageComponent != null)
                RawImageComponent.texture = Minimap_RT;
        }

        private void Update()
        {

            DrawMiniMapFrustum();
        }

        void DrawMiniMapFrustum()
        {

        }
    }

}
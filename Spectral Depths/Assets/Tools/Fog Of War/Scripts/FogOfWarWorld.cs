using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine.Profiling;
#endif

namespace FOW
{
    [DefaultExecutionOrder(-100)]
    public class FogOfWarWorld : MonoBehaviour
    {
        public static FogOfWarWorld instance;

        public FogOfWarType FogType = FogOfWarType.Soft;
        public bool AllowBleeding = false;
        
        public bool UsingSoftening;
        //[Tooltip("how far to blur the edges. only used for soft fog types")]
        //public float SoftenDistance = 3;
        public float EdgeSoftenDistance = 0f;
        public float UnobscuredSoftenDistance = .25f;
        public bool UseInnerSoften = true;
        public float InnerSoftenAngle = 5;
        public FogOfWarFadeType FogFade = FogOfWarFadeType.Smoothstep;
        public FogOfWarBlendMode BlendType = FogOfWarBlendMode.Max;
        public float FogFadePower = 1;

        [SerializeField] private FogOfWarAppearance FogAppearance;

        [Tooltip("The color of the fog")]
        public Color UnknownColor = new Color(.35f, .35f, .35f);

        public float SaturationStrength = 0;

        public float BlurStrength = 1;
        //public float blurPixelOffset = 2.5f;
        [Range(0, 2)]
        public float BlurDistanceScreenPercentMin = .1f;
        [Range(0, 2)]
        public float BlurDistanceScreenPercentMax = 1;
        public int BlurSamples = 6;

        public Texture2D FogTexture;
        public Vector2 FogTextureTiling = Vector2.one;
        public float FogScrollSpeed = 0;

        public float LineThickness = .1f;

        public FogSampleMode FOWSamplingMode = FogSampleMode.Pixel_Perfect;
        public bool UseRegrow;
        public float FogRegrowSpeed = .5f;
        public float InitialFogExplorationValue = 0;
        public float MaxFogRegrowAmount = .3f;
        RenderTexture FOW_RT;
        RenderTexture FOW_REGROW_RT;
        public Material FowTextureMaterial;
        public int FowResX = 256;
        public int FowResY = 256;
        public bool UseConstantBlur = true;
        public int ConstantTextureBlurQuality = 2;
        public float ConstantTextureBlurAmount = 0.75f;

        public bool UseWorldBounds;
        public float WorldBoundsSoftenDistance = 1f;
        public float WorldBoundsInfluence = 1;
        public Bounds WorldBounds = new Bounds(Vector3.zero, Vector3.one);
        
        public bool UseMiniMap;
        public Color MiniMapColor = new Color(.4f, .4f, .4f, .95f);
        public RawImage UIImage;

        //public bool AllowMinimumDistance = false;

        //[Range(0.001f, 1f)]
        public float SightExtraAmount = .01f;

        public float MaxFogDistance = 10000f;

        public RevealerUpdateMode revealerMode = RevealerUpdateMode.Every_Frame;
        [Tooltip("The number of revealers to update each frame. Only used when Revealer Mode is set to N_Per_Frame")]
        public int numRevealersPerFrame = 3;

        [Tooltip("The Max possible number of revealers. Keep this as low as possible to use less GPU memory")]
        public int maxPossibleRevealers = 256;
        [Tooltip("The Max possible number of segments per revealer. Keep this as low as possible to use less GPU memory")]
        public int maxPossibleSegmentsPerRevealer = 125;

        public bool is2D;
        public GamePlane gamePlane = GamePlane.XZ;

        public Material FogOfWarMaterial;

        public static List<PartialHider> PartialHiders = new List<PartialHider>();

        int maxCones;
        public ComputeBuffer indicesBuffer;
        public ComputeBuffer circleBuffer;
        public ComputeBuffer anglesBuffer;
        int numRevealers;
        public int numDynamicRevealers;

        int numCirclesID = Shader.PropertyToID("_NumCircles");
        int materialColorID = Shader.PropertyToID("_unKnownColor");
        //int blurRadiusID = Shader.PropertyToID("_fadeOutDistance");
        int unobscuredBlurRadiusID = Shader.PropertyToID("_unboscuredFadeOutDistance");
        int extraRadiusID = Shader.PropertyToID("_extraRadius");
        int maxDistanceID = Shader.PropertyToID("_maxDistance");
        int fadePowerID = Shader.PropertyToID("_fadePower");
        int saturationStrengthID = Shader.PropertyToID("_saturationStrength");
        int blurStrengthID = Shader.PropertyToID("_blurStrength");
        //int blurPixelOffsetID = Shader.PropertyToID("_blurPixelOffset");
        int blurPixelOffsetMinID = Shader.PropertyToID("_blurPixelOffsetMin");
        int blurPixelOffsetMaxID = Shader.PropertyToID("_blurPixelOffsetMax");
        int blurSamplesID = Shader.PropertyToID("_blurSamples");
        int blurPeriodID = Shader.PropertyToID("_samplePeriod");
        int fowTetureID = Shader.PropertyToID("_fowTexture");
        int fowTilingID = Shader.PropertyToID("_fowTiling");
        int fowSpeedID = Shader.PropertyToID("_fowScrollSpeed");

        #region Data Structures

        [StructLayout(LayoutKind.Sequential)]
        public struct CircleStruct
        {
            public Vector2 CircleOrigin;
            public int StartIndex;
            public int NumSegments;
            public float CircleHeight;
            public float UnobscuredRadius;
            //public int isComplete;
            public float CircleRadius;
            public float CircleFade;
            public float VisionHeight;
            public float HeightFade;
            public float Opacity;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct ConeEdgeStruct
        {
            public float angle;
            public float length;
            public int cutShort;
        };
        public enum RevealerUpdateMode
        {
            Every_Frame,
            N_Per_Frame,
            Controlled_ElseWhere,
        };
        public enum FogSampleMode
        {
            Pixel_Perfect,
            Texture,
            Both,
        };
        public enum FogOfWarType
        {
            //No_Bleed,
            //No_Bleed_Soft,
            Hard,
            Soft,
        };
        public enum FogOfWarFadeType
        {
            Linear,
            Exponential,
            Smooth,
            Smoother,
            Smoothstep,
        };
        public enum FogOfWarBlendMode
        {
            Max,
            Addative,
        };
        public enum FogOfWarAppearance
        {
            Solid_Color,
            GrayScale,
            Blur,
            Texture_Sample,
            Outline,
            None
        };
        public enum GamePlane
        {
            XZ,
            XY,
            ZY,
        };
        #endregion

        #region Unity Methods
        private void Awake()
        {
#if UNITY_EDITOR
            // see the unity bug workaround section
            UnityBugWorkaround.OnAssetPostProcess += ReInitMaterial;
#endif
            Initialize();
        }
        private void OnEnable()
        {
            Initialize();
        }
        private void OnDisable()
        {
            Cleanup();
        }
        private void OnDestroy()
        {
#if UNITY_EDITOR
            // see the unity bug workaround section
            UnityBugWorkaround.OnAssetPostProcess -= ReInitMaterial;
#endif
            Cleanup();
        }

        int currentIndex = 0;
        private void Update()
        {
            if (numRevealers > 0)
            {
                switch (revealerMode)
                {
                    case RevealerUpdateMode.Every_Frame:
                        for (int i = 0; i < numRevealers; i++)
                        {
                            Revealers[i].RevealHiders();
                            if (!Revealers[i].StaticRevealer)
                                Revealers[i].LineOfSightPhase1();
                        }
                        for (int i = 0; i < numRevealers; i++)
                        {
                            if (!Revealers[i].StaticRevealer)
                                Revealers[i].LineOfSightPhase2();
                        }
                        break;
                    case RevealerUpdateMode.N_Per_Frame:
                        int index = currentIndex;
                        for (int i = 0; i < Mathf.Clamp(numRevealersPerFrame, 0, numDynamicRevealers); i++)
                        {
                            index = (index + 1) % numRevealers;
                            Revealers[index].RevealHiders();
                            if (!Revealers[index].StaticRevealer)
                                Revealers[index].LineOfSightPhase1();
                            else
                                i--;
                        }
                        for (int i = 0; i < Mathf.Clamp(numRevealersPerFrame, 0, numDynamicRevealers); i++)
                        {
                            currentIndex = (currentIndex + 1) % numRevealers;
                            if (!Revealers[currentIndex].StaticRevealer)
                                Revealers[currentIndex].LineOfSightPhase2();
                            else
                                i--;
                        }
                        break;
                    case RevealerUpdateMode.Controlled_ElseWhere: break;
                }
            }

            if (UseMiniMap || FOWSamplingMode == FogSampleMode.Texture || FOWSamplingMode == FogSampleMode.Both)
            {
                if (UseRegrow)
                {
                    //RenderTexture temp = RenderTexture.GetTemporary(FOW_RT.descriptor);
                    //Graphics.Blit(FOW_RT, temp, FowTextureMaterial, 1);
                    Graphics.Blit(FOW_RT, FOW_REGROW_RT, FowTextureMaterial, 1);

                    Graphics.Blit(FOW_REGROW_RT, FOW_RT, FowTextureMaterial, 0);
                    //Graphics.Blit(temp, FOW_RT, FowTextureMaterial, 0);
                    //RenderTexture.ReleaseTemporary(temp);
                }
                else
                    Graphics.Blit(null, FOW_RT, FowTextureMaterial, 0);
            }
        }
        #endregion

        #region Dumb Unity Bug Workaround :)
#if UNITY_EDITOR
        //BASICALLY, every time an asset is updated in the project folder, materials are losing the compute buffer data. 
        //So, im hooking onto asset post processing, and re-initializing the material with the necessary data
        public void ReInitMaterial()
        {
            StartCoroutine(ReInitMaterialDebug());
        }
        IEnumerator ReInitMaterialDebug()
        {
            yield return new WaitForEndOfFrame();
            FogOfWarMaterial.SetBuffer(Shader.PropertyToID("_ActiveCircleIndices"), indicesBuffer);
            FogOfWarMaterial.SetBuffer(Shader.PropertyToID("_CircleBuffer"), circleBuffer);
            FogOfWarMaterial.SetBuffer(Shader.PropertyToID("_ConeBuffer"), anglesBuffer);
            UpdateMaterialProperties(FogOfWarMaterial);
        }
#endif
        #endregion

        void Cleanup()
        {
            int n = numRevealers;
            for (int i = 0; i < n; i++)
            {
                FogOfWarRevealer revealer = Revealers[0];
                revealer.DeregisterRevealer();
                RevealersToRegister.Add(revealer);
            }
            if (circleBuffer != null)
            {
                //setAnglesBuffersJobHandle.Complete();
                //AnglesNativeArray.Dispose();
                indicesBuffer.Dispose();
                circleBuffer.Dispose();
                anglesBuffer.Dispose();
            }
            instance = null;
        }

        //private JobHandle setAnglesBuffersJobHandle;
        //private SetAnglesBuffersJob setAnglesBuffersJob;
        //NativeArray<ConeEdgeStruct> AnglesNativeArray;    //was used when using computebuffer.beginwrite. will be used again when unity fixes a bug internally
        ConeEdgeStruct[] anglesArray;

        public void Initialize()
        {
            if (instance)
                return;
            instance = this;

            maxCones = maxPossibleRevealers * maxPossibleSegmentsPerRevealer;

            Revealers = new FogOfWarRevealer[maxPossibleRevealers];
            //indicesBuffer = new ComputeBuffer(maxPossibleRevealers, Marshal.SizeOf(typeof(int)), ComputeBufferType.Default, ComputeBufferMode.SubUpdates);
            indicesBuffer = new ComputeBuffer(maxPossibleRevealers, Marshal.SizeOf(typeof(int)), ComputeBufferType.Default);

            //circleBuffer = new ComputeBuffer(maxPossibleRevealers, Marshal.SizeOf(typeof(CircleStruct)), ComputeBufferType.Default, ComputeBufferMode.SubUpdates);
            circleBuffer = new ComputeBuffer(maxPossibleRevealers, Marshal.SizeOf(typeof(CircleStruct)), ComputeBufferType.Default);

            anglesArray = new ConeEdgeStruct[maxPossibleSegmentsPerRevealer];
            //AnglesNativeArray = new NativeArray<ConeEdgeStruct>(maxPossibleSegmentsPerRevealer, Allocator.Persistent);
            //anglesBuffer = new ComputeBuffer(maxCones, Marshal.SizeOf(typeof(ConeEdgeStruct)), ComputeBufferType.Default, ComputeBufferMode.SubUpdates);
            anglesBuffer = new ComputeBuffer(maxCones, Marshal.SizeOf(typeof(ConeEdgeStruct)), ComputeBufferType.Default);

            FogOfWarMaterial = new Material(Shader.Find("Hidden/FullScreen/FOW/SolidColor"));
            //SetFogShader();

            //UpdateMaterialProperties(FogOfWarMaterial);
            if (UseMiniMap || FOWSamplingMode == FogSampleMode.Texture || FOWSamplingMode == FogSampleMode.Both)
            {
                FowTextureMaterial = new Material(Shader.Find("Hidden/FullScreen/FOW/FOW_RT"));
                InitFOWRT();

                UpdateMaterialProperties(FowTextureMaterial);
                FowTextureMaterial.SetBuffer(Shader.PropertyToID("_ActiveCircleIndices"), indicesBuffer);
                FowTextureMaterial.SetBuffer(Shader.PropertyToID("_CircleBuffer"), circleBuffer);
                FowTextureMaterial.SetBuffer(Shader.PropertyToID("_ConeBuffer"), anglesBuffer);
                FowTextureMaterial.EnableKeyword("IGNORE_HEIGHT");
            }
            SetFogShader();
            SetMaterialBounds();

            //setAnglesBuffersJob = new SetAnglesBuffersJob();

            foreach (FogOfWarRevealer revealer in RevealersToRegister)
            {
                if (revealer != null)
                    revealer.RegisterRevealer();
            }
            RevealersToRegister.Clear();
        }

        public void InitFOWRT()
        {
            FOW_RT = new RenderTexture(FowResX, FowResY, 0);
            //FOW_RT.format = RenderTextureFormat.ARGBFloat;
            //FOW_RT.format = RenderTextureFormat.Default;
            FOW_RT.format = RenderTextureFormat.ARGBHalf;
            //Debug.Log(FOW_RT.filterMode);
            //Debug.Log(FOW_RT.antiAliasing);
            //Debug.Log(FOW_RT.anisoLevel);
            FOW_RT.antiAliasing = 8;
            FOW_RT.filterMode = FilterMode.Trilinear;
            FOW_RT.anisoLevel = 9;
            FOW_RT.Create();
            RenderTexture.active = FOW_RT;
            Material mat = new Material(Shader.Find("Hidden/Internal-Colored"));
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            mat.SetInt("_ZWrite", 0);
            mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
            mat.SetPass(0);
            GL.Begin(GL.TRIANGLES);
            GL.Clear(true, true, new Color(0, 0, 0, 1-InitialFogExplorationValue));
            GL.End();
            if (UseMiniMap && UIImage != null)
                UIImage.texture = FOW_RT;
            if (UseRegrow)
            {
                //FOW_REGROW_RT = new RenderTexture(FowResX, FowResY, 0);
                FOW_REGROW_RT = new RenderTexture(FOW_RT);
                FOW_REGROW_RT.Create();
            }
        }
        public RenderTexture GetFOWRT()
        {
            return FOW_RT;
        }

        public static Vector3 UpVector;
        public static Vector3 ForwardVector;
        public void SetFogShader()
        {
            if (!Application.isPlaying)
                return;

            UsingSoftening = false;
            string shaderName = "Hidden/FullScreen/FOW";
            switch (FogAppearance)
            {
                case FogOfWarAppearance.Solid_Color: shaderName += "/SolidColor"; break;
                case FogOfWarAppearance.GrayScale: shaderName += "/GrayScale"; break;
                case FogOfWarAppearance.Blur: shaderName += "/Blur"; break;
                case FogOfWarAppearance.Texture_Sample: shaderName += "/TextureSample"; break;
                case FogOfWarAppearance.Outline: shaderName += "/Outline"; break;
                case FogOfWarAppearance.None: shaderName = "Hidden/BlitCopy"; break;
            }
            FogOfWarMaterial.shader = Shader.Find(shaderName);
#if UNITY_2021_2_OR_NEWER
#else
            //this was required in unity 2020.3.28. when updating to 2020.3.48, its no longer required. not sure what version fixes it exactly.
            //FogOfWarMaterial.EnableKeyword("_VS_NORMAL");   //this is only for urp/texture sample fog mode
#endif

            InitializeFogProperties(FogOfWarMaterial);
            UpdateMaterialProperties(FogOfWarMaterial);
            SetMaterialBounds();
        }

        public void InitializeFogProperties(Material material)
        {
            material.DisableKeyword("IS_2D");
            material.DisableKeyword("IS_3D");
            if (!is2D)
            {
                material.EnableKeyword("IS_3D");
                material.DisableKeyword("PLANE_XZ");
                material.DisableKeyword("PLANE_XY");
                material.DisableKeyword("PLANE_ZY");
                switch (gamePlane)
                {
                    case GamePlane.XZ:
                        material.EnableKeyword("PLANE_XZ");
                        UpVector = Vector3.up;
                        break;
                    case GamePlane.XY:
                        material.EnableKeyword("PLANE_XY");
                        UpVector = -Vector3.forward;
                        break;
                    case GamePlane.ZY:
                        material.EnableKeyword("PLANE_ZY");
                        UpVector = Vector3.right;
                        break;
                }
            }
            else
            {
                UpVector = -Vector3.forward;
                material.EnableKeyword("IS_2D");
            }

            material.SetBuffer(Shader.PropertyToID("_ActiveCircleIndices"), indicesBuffer);
            material.SetBuffer(Shader.PropertyToID("_CircleBuffer"), circleBuffer);
            material.SetBuffer(Shader.PropertyToID("_ConeBuffer"), anglesBuffer);
        }

        public void UpdateFogConfig()
        {
            if (!Application.isPlaying)
                return;

            UpdateMaterialProperties(FogOfWarMaterial);
            if (FowTextureMaterial != null)
                UpdateMaterialProperties(FowTextureMaterial);

            foreach (PartialHider hider in PartialHiders)
                UpdateMaterialProperties(hider.HiderMaterial);
            SetMaterialBounds();
        }
        public void UpdateMaterialProperties(Material material)
        {
#if UNITY_EDITOR
            if (material == null)   //fix for "Enter Playmode Options"
                return;
#endif
            material.DisableKeyword("HARD");
            material.DisableKeyword("SOFT");
            UsingSoftening = false;
            switch (FogType)
            {
                case FogOfWarType.Hard: material.EnableKeyword("HARD"); break;
                case FogOfWarType.Soft: material.EnableKeyword("SOFT"); UsingSoftening = true; break;
            }

            //material.DisableKeyword("BLEED");
            //if (AllowBleeding)
            //    material.EnableKeyword("BLEED");
            material.SetInt("BLEED", 0);
            if (AllowBleeding)
                material.SetInt("BLEED", 1);

            material.SetColor(materialColorID, material == FowTextureMaterial ? MiniMapColor : UnknownColor);
            material.SetFloat(unobscuredBlurRadiusID, UnobscuredSoftenDistance);
            material.DisableKeyword("INNER_SOFTEN");
            if (FogType == FogOfWarType.Soft && UseInnerSoften)
            {
                material.EnableKeyword("INNER_SOFTEN");
                material.SetFloat(Shader.PropertyToID("_fadeOutDegrees"), InnerSoftenAngle);
            }
            else
                material.SetFloat(Shader.PropertyToID("_fadeOutDegrees"), 0);

            material.SetFloat(extraRadiusID, SightExtraAmount);
            material.SetFloat(Shader.PropertyToID("_edgeSoftenDistance"), EdgeSoftenDistance);
            material.SetFloat(maxDistanceID, MaxFogDistance);

            //material.DisableKeyword("FADE_LINEAR");
            //material.DisableKeyword("FADE_SMOOTH");
            //material.DisableKeyword("FADE_SMOOTHER");
            //material.DisableKeyword("FADE_SMOOTHSTEP");
            //material.DisableKeyword("FADE_EXP");
            //switch (FogFade)
            //{
            //    case FogOfWarFadeType.Linear:
            //        material.EnableKeyword("FADE_LINEAR");
            //        break;
            //    case FogOfWarFadeType.Exponential:
            //        material.EnableKeyword("FADE_EXP");
            //        material.SetFloat(fadePowerID, FogFadePower);
            //        break;
            //    case FogOfWarFadeType.Smooth:
            //        material.EnableKeyword("FADE_SMOOTH");
            //        break;
            //    case FogOfWarFadeType.Smoother:
            //        material.EnableKeyword("FADE_SMOOTHER");
            //        break;
            //    case FogOfWarFadeType.Smoothstep:
            //        material.EnableKeyword("FADE_SMOOTHSTEP");
            //        break;
            //}
            switch (FogFade)
            {
                case FogOfWarFadeType.Linear:
                    material.SetInt("_fadeType", 0);
                    break;
                case FogOfWarFadeType.Exponential:
                    material.SetInt("_fadeType", 4);
                    material.SetFloat(fadePowerID, FogFadePower);
                    break;
                case FogOfWarFadeType.Smooth:
                    material.SetInt("_fadeType", 1);
                    break;
                case FogOfWarFadeType.Smoother:
                    material.SetInt("_fadeType", 2);
                    break;
                case FogOfWarFadeType.Smoothstep:
                    material.SetInt("_fadeType", 3);
                    break;
            }
            //material.DisableKeyword("BLEND_MAX");
            //material.DisableKeyword("BLEND_ADDITIVE");
            //switch (BlendType)
            //{
            //    case FogOfWarBlendMode.Max:
            //        material.EnableKeyword("BLEND_MAX");
            //        break;
            //    case FogOfWarBlendMode.Addative:
            //        material.EnableKeyword("BLEND_ADDITIVE");
            //        break;
            //}
            material.SetInt("BLEND_MAX", 1);
            switch (BlendType)
            {
                case FogOfWarBlendMode.Max:
                    material.SetInt("BLEND_MAX", 1);
                    break;
                case FogOfWarBlendMode.Addative:
                    material.SetInt("BLEND_MAX", 0);
                    break;
            }

            switch (FogAppearance)
            {
                case FogOfWarAppearance.Solid_Color:
                    break;
                case FogOfWarAppearance.GrayScale:
                    material.SetFloat(saturationStrengthID, SaturationStrength);
                    break;
                case FogOfWarAppearance.Blur:
                    material.SetFloat(blurStrengthID, BlurStrength);
                    material.SetFloat(blurPixelOffsetMinID, Screen.height * (BlurDistanceScreenPercentMin / 100));
                    material.SetFloat(blurPixelOffsetMaxID, Screen.height * (BlurDistanceScreenPercentMax / 100));
                    material.SetInt(blurSamplesID, BlurSamples);
                    material.SetFloat(blurPeriodID, (2 * Mathf.PI) / BlurSamples);    //TAU = 2 * PI
                    break;
                case FogOfWarAppearance.Texture_Sample:
                    material.SetTexture(fowTetureID, FogTexture);
                    material.SetVector(fowTilingID, FogTextureTiling);
                    material.SetFloat(fowSpeedID, FogScrollSpeed);
                    break;
                case FogOfWarAppearance.Outline:
                    material.SetFloat("lineThickness", LineThickness);
                    break;
            }


            material.DisableKeyword("SAMPLE_REALTIME");
            if (FOWSamplingMode == FogSampleMode.Pixel_Perfect || FOWSamplingMode == FogSampleMode.Both)
                material.EnableKeyword("SAMPLE_REALTIME");

            material.DisableKeyword("SAMPLE_TEXTURE");
            material.DisableKeyword("USE_TEXTURE_BLUR");
            if (FOWSamplingMode == FogSampleMode.Texture || FOWSamplingMode == FogSampleMode.Both)
            {
                material.SetTexture("_FowRT", FOW_RT);
                material.EnableKeyword("SAMPLE_TEXTURE");
                
                if (UseConstantBlur)
                {
                    material.EnableKeyword("USE_TEXTURE_BLUR");
                    material.SetFloat("_Sample_Blur_Quality", ConstantTextureBlurQuality);
                    material.SetFloat("_Sample_Blur_Amount", ConstantTextureBlurAmount);
                }
            }

            if (material == FowTextureMaterial)
            {
                //material.SetTexture("_FowRT", FOW_RT);
                //material.SetTexture("_FowRT", FogTexture);
                material.SetFloat("_regrowSpeed", FogRegrowSpeed);
                material.SetFloat("_maxRegrowAmount", MaxFogRegrowAmount);
                material.EnableKeyword("SAMPLE_REALTIME");
                material.DisableKeyword("SAMPLE_TEXTURE");
                material.DisableKeyword("USE_REGROW");
                if (UseRegrow)
                    material.EnableKeyword("USE_REGROW");
            }

            material.DisableKeyword("USE_WORLD_BOUNDS");
            if (UseRegrow)
                material.EnableKeyword("USE_WORLD_BOUNDS");

            //material.DisableKeyword("USE_WORLD_BOUNDS");
            //if (UseWorldBounds)
            //    material.EnableKeyword("USE_WORLD_BOUNDS");
            material.SetFloat("_worldBoundsInfluence", 0);
            if (UseWorldBounds)
            {
                material.SetFloat("_worldBoundsSoftenDistance", WorldBoundsSoftenDistance);
                material.SetFloat("_worldBoundsInfluence", WorldBoundsInfluence);
            }

            SetMaterialBounds();
        }

        public void UpdateWorldBounds(Vector3 center, Vector3 extent)
        {
            WorldBounds.center = center;
            WorldBounds.extents = extent;
            SetMaterialBounds();
        }
        public void UpdateWorldBounds(Bounds newBounds)
        {
            WorldBounds = newBounds;
            SetMaterialBounds();
        }

        void SetMaterialBounds()
        {
            //if (UseWorldBounds && FogOfWarMaterial != null)
            Vector4 boundsVec = GetBoundsVectorForShader();
            if (FogOfWarMaterial != null)
                FogOfWarMaterial.SetVector("_worldBounds", boundsVec);
            if (FowTextureMaterial != null)
                FowTextureMaterial.SetVector("_worldBounds", boundsVec);
        }

        public Vector4 GetBoundsVectorForShader()
        {
            if (is2D)
                return new Vector4(WorldBounds.size.x, WorldBounds.center.x, WorldBounds.size.y, WorldBounds.center.y);

            switch(gamePlane)
            {
                case GamePlane.XZ: return new Vector4(WorldBounds.size.x, WorldBounds.center.x, WorldBounds.size.z, WorldBounds.center.z);
                case GamePlane.XY: return new Vector4(WorldBounds.size.x, WorldBounds.center.x, WorldBounds.size.y, WorldBounds.center.y);
                case GamePlane.ZY: return new Vector4(WorldBounds.size.z, WorldBounds.center.z, WorldBounds.size.z, WorldBounds.center.z);
            }

            return new Vector4(WorldBounds.size.x, WorldBounds.center.x, WorldBounds.size.z, WorldBounds.center.z);
        }

        public Vector2 GetFowPositionFromWorldPosition(Vector3 WorldPosition)
        {
            if (is2D)
                return new Vector2(WorldPosition.x, WorldPosition.y);

            switch (gamePlane)
            {
                case GamePlane.XZ: return new Vector2(WorldPosition.x, WorldPosition.z);
                case GamePlane.XY: return new Vector2(WorldPosition.x, WorldPosition.y);
                case GamePlane.ZY: return new Vector2(WorldPosition.z, WorldPosition.y);
            }

            return new Vector2(WorldPosition.x, WorldPosition.z);
        }

        void SetNumCircles()
        {
            if (FogOfWarMaterial != null)
                FogOfWarMaterial.SetInt(numCirclesID, numRevealers);
            if (FowTextureMaterial != null)
                FowTextureMaterial.SetInt(numCirclesID, numRevealers);
            foreach (PartialHider hider in PartialHiders)
                hider.HiderMaterial.SetInt(numCirclesID, numRevealers);
        }

        public FogOfWarRevealer[] Revealers;
        public List<int> deregisteredIDs = new List<int>();
        int numDeregistered = 0;
        public static List<FogOfWarRevealer> RevealersToRegister = new List<FogOfWarRevealer>();    //just used to prevent script execution order errors
        int[] indiciesDataToSet = new int[1];

        //NativeArray<int> _circleIndicesArray;
        //NativeArray<CircleStruct> _circleArray;
        //NativeArray<ConeEdgeStruct> _angleArray;
        public int RegisterRevealer(FogOfWarRevealer newRevealer)
        {
#if UNITY_EDITOR
            Profiler.BeginSample("Register Revealer");
#endif
            numRevealers++;
            if (!newRevealer.StaticRevealer)
                numDynamicRevealers++;
            SetNumCircles();

            int newID = numRevealers - 1;
            Revealers[newID] = newRevealer;
            if (numDeregistered > 0)
            {
                numDeregistered--;
                newID = deregisteredIDs[0];
                deregisteredIDs.RemoveAt(0);
            }

            newRevealer.IndexID = numRevealers - 1;

            indiciesDataToSet[0] = newID;
            indicesBuffer.SetData(indiciesDataToSet, 0, numRevealers - 1, 1);

            //_circleIndicesArray = indicesBuffer.BeginWrite<int>(numCircles - 1, 1);
            //_circleIndicesArray[0] = newID;

            //indicesBuffer.EndWrite<int>(1);

#if UNITY_EDITOR
            Profiler.EndSample();
#endif
            return newID;
        }
        public void DeRegisterRevealer(FogOfWarRevealer toRemove)
        {
#if UNITY_EDITOR
            Profiler.BeginSample("De-Register Revealer");
#endif
            int index = toRemove.IndexID;

            deregisteredIDs.Add(toRemove.FogOfWarID);
            numDeregistered++;

            numRevealers--;
            if (!toRemove.StaticRevealer)
                numDynamicRevealers--;

            FogOfWarRevealer toSwap = Revealers[numRevealers];

            if (toRemove != toSwap)
            {
                Revealers[index] = toSwap;

                indiciesDataToSet[0] = toSwap.FogOfWarID;
                indicesBuffer.SetData(indiciesDataToSet, 0, index, 1);
                //_circleIndicesArray = indicesBuffer.BeginWrite<int>(index, 1);
                //_circleIndicesArray[0] = toSwap.FogOfWarID;
                toSwap.IndexID = index;

                //indicesBuffer.EndWrite<int>(1);
            }

            SetNumCircles();
#if UNITY_EDITOR
            Profiler.EndSample();
#endif
        }

        public static List<FogOfWarHider> hiders = new List<FogOfWarHider>();
        public static int numHiders;

        CircleStruct[] circleDataToSet = new CircleStruct[1];
        public void UpdateCircle(int id, CircleStruct data, int numHits, ref float[] radii, ref float[] distances, ref bool[] hits)
        {
#if UNITY_EDITOR
            Profiler.BeginSample("write to compute buffers");
#endif
            //setAnglesBuffersJobHandle.Complete();
            data.StartIndex = id * maxPossibleSegmentsPerRevealer;
            circleDataToSet[0] = data;
            circleBuffer.SetData(circleDataToSet, 0, id, 1);
            //_circleArray = circleBuffer.BeginWrite<CircleStruct>(id, 1);
            //_circleArray[0] = data;
            //circleBuffer.EndWrite<CircleStruct>(1);

            if (numHits > maxPossibleSegmentsPerRevealer)
            {
                Debug.LogError($"the revealer is trying to register {numHits} segments. this is more than was set by maxPossibleSegmentsPerRevealer");
                return;
            }
            for (int i = 0; i < numHits; i++)
            {
                anglesArray[i].angle = radii[i];
                anglesArray[i].length = distances[i];
                anglesArray[i].cutShort = hits[i] ? 1 : 0;
                //AnglesNativeArray[i] = anglesArray[i];
            }

            anglesBuffer.SetData(anglesArray, 0, id * maxPossibleSegmentsPerRevealer, numHits);
            //the following lines of code should work in theory, however due to a unity bug, are going to be put on hold for a little bit.
            //_angleArray = anglesBuffer.BeginWrite<ConeEdgeStruct>(id * maxPossibleSegmentsPerRevealer, radii.Length);
            //setAnglesBuffersJob.AnglesArray = _angleArray;
            //setAnglesBuffersJob.Angles = AnglesNativeArray;
            //setAnglesBuffersJobHandle = setAnglesBuffersJob.Schedule(radii.Length, 128);
            //setAnglesBuffersJobHandle.Complete();
            //anglesBuffer.EndWrite<ConeEdgeStruct>(radii.Length);

#if UNITY_EDITOR
            Profiler.EndSample();
#endif
        }

        [BurstCompile(CompileSynchronously = true)]
        private struct SetAnglesBuffersJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<ConeEdgeStruct> Angles;
            [WriteOnly]
            public NativeArray<ConeEdgeStruct> AnglesArray;

            public void Execute(int index)
            {
                AnglesArray[index] = Angles[index];
            }
        }

        /// <summary>
        /// Test if provided point is currently visible.
        /// </summary>
        public static bool TestPointVisibility(Vector3 point)
        {
            for (int i = 0; i < instance.numRevealers; i++)
            {
                if (instance.Revealers[i].TestPoint(point))
                    return true;
            }
            return false;
        }

        public void SetFowAppearance(FogOfWarAppearance AppearanceMode)
        {
            FogAppearance = AppearanceMode;
            if (!Application.isPlaying)
                return;

            enabled = false;
            enabled = true;
        }

        public FogOfWarAppearance GetFowAppearance()
        {
            return FogAppearance;
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(FogOfWarWorld))]
    public class FogOfWarWorldEditor : Editor
    {
        static class Styles
        {
            public static readonly GUIStyle rightLabel = new GUIStyle("RightLabel");
        }
        string[] FowSampleOptions = new string[]
        {
            "Pixel-Perfect", "Texture"
            //"Pixel-Perfect", "Texture", "Both"
        };
        string[] FogTypeOptions = new string[]
        {
            //"No Bleed", "No Bleed Soft", "Hard", "Soft"
            "Hard", "Soft"
        };
        string[] FogAppearanceOptions = new string[]
        {
            "Solid Color", "Gray Scale", "Blur", "Texture Sample", "Outline (BETA)", "None - MiniMap Only"
        };
        string[] FogFadeOptions = new string[]
        {
            "Linear", "Exponential", "Smooth", "Smoother", "Smooth Step"
        };
        string[] FogBlendOptions = new string[]
        {
            "Maximum", "Additive"
        };
        string[] RevealerModeOptions = new string[]
        {
            "Every Frame", "N Per Frame", "Controlled Elsewhere"
        };
        string[] GamePlaneOptions = new string[]
        {
            "XZ", "XY", "ZY"
        };
        private BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle();
        void OnSceneGUI()
        {
            FogOfWarWorld fow = (FogOfWarWorld)target;
            if (fow.UseWorldBounds || fow.UseMiniMap || fow.FOWSamplingMode == FogOfWarWorld.FogSampleMode.Texture || fow.FOWSamplingMode == FogOfWarWorld.FogSampleMode.Both)
            {
                m_BoundsHandle.center = fow.WorldBounds.center;
                m_BoundsHandle.size = fow.WorldBounds.size;

                EditorGUI.BeginChangeCheck();
                m_BoundsHandle.DrawHandle();
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(fow, "Change Bounds");

                    Bounds newBounds = new Bounds();
                    newBounds.center = m_BoundsHandle.center;
                    newBounds.size = m_BoundsHandle.size;
                    fow.UpdateWorldBounds(newBounds);
                    //fow.WorldBounds = newBounds;
                }
            }
        }
        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();
            FogOfWarWorld fow = (FogOfWarWorld)target;

            EditorGUILayout.LabelField("Customization Options:");
            FogOfWarWorld.FogOfWarType fogType = fow.FogType;
            int selected = (int)fogType;
            selected = EditorGUILayout.Popup("Fog Type", selected, FogTypeOptions);
            fogType = (FogOfWarWorld.FogOfWarType)selected;
            if (fow.FogType != fogType)
            {
                fow.FogType = fogType;
                Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                fow.UpdateFogConfig();
            }
            bool AllowBleeding = fow.AllowBleeding;
            bool newAllowBleeding = EditorGUILayout.Toggle("Allow Bleeding?", AllowBleeding);
            if (newAllowBleeding != AllowBleeding)
            {
                fow.AllowBleeding = newAllowBleeding;
                Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                fow.UpdateFogConfig();
            }
            //if (fow.FogType == FogOfWarWorld.FogOfWarType.No_Bleed_Soft || fow.FogType == FogOfWarWorld.FogOfWarType.Soft)
            if (fow.FogType == FogOfWarWorld.FogOfWarType.Soft)
            {
                FogOfWarWorld.FogOfWarFadeType fadeType = fow.FogFade;
                selected = (int)fadeType;
                selected = EditorGUILayout.Popup("Fade Type", selected, FogFadeOptions);
                fadeType = (FogOfWarWorld.FogOfWarFadeType)selected;
                if (fow.FogFade != fadeType)
                {
                    fow.FogFade = fadeType;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.UpdateFogConfig();
                }
                if (fadeType == FogOfWarWorld.FogOfWarFadeType.Exponential)
                {
                    float fadeExp = fow.FogFadePower;
                    float newfadeExp = EditorGUILayout.FloatField("Fade Exponent: ", fadeExp);
                    if (fadeExp != newfadeExp)
                    {
                        fow.FogFadePower = newfadeExp;
                        Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                        fow.UpdateFogConfig();
                    }
                }
                FogOfWarWorld.FogOfWarBlendMode blendType = fow.BlendType;
                selected = (int)blendType;
                selected = EditorGUILayout.Popup("Blend Type", selected, FogBlendOptions);
                blendType = (FogOfWarWorld.FogOfWarBlendMode)selected;
                if (fow.BlendType != blendType)
                {
                    fow.BlendType = blendType;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.UpdateFogConfig();
                }

                //float softenDist = fow.SoftenDistance;
                //float newSoftenDist = EditorGUILayout.FloatField("Soften Distance: ", softenDist);
                //if (newSoftenDist != softenDist)
                //{
                //    fow.SoftenDistance = newSoftenDist;
                //    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                //    fow.UpdateFogConfig();
                //}

                float EdgeSoftenDistance = fow.EdgeSoftenDistance;
                float newEdgeSoftenDist = EditorGUILayout.FloatField("Edge Softening Distance: ", EdgeSoftenDistance);
                if (!Mathf.Approximately(newEdgeSoftenDist, EdgeSoftenDistance))
                {
                    fow.EdgeSoftenDistance = Mathf.Max(0, newEdgeSoftenDist);
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.UpdateFogConfig();
                }

                float unobscuredsoftenDist = fow.UnobscuredSoftenDistance;
                float newUnobscuredSoftenDist = EditorGUILayout.FloatField("Un-Obscured area Soften Distance: ", unobscuredsoftenDist);
                if (!Mathf.Approximately(newUnobscuredSoftenDist, unobscuredsoftenDist))
                {
                    fow.UnobscuredSoftenDistance = Mathf.Max(0, newUnobscuredSoftenDist);
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.UpdateFogConfig();
                }

                bool innerSoften = fow.UseInnerSoften;
                bool newinnerSoften = EditorGUILayout.Toggle("Soften Inner Edge? (BETA!)", innerSoften);
                if (newinnerSoften != innerSoften)
                {
                    fow.UseInnerSoften = newinnerSoften;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.UpdateFogConfig();
                }
                if (newinnerSoften)
                {
                    float softenAng = fow.InnerSoftenAngle;
                    float newSoftenAng = EditorGUILayout.FloatField("Inner Soften Angle: ", softenAng);
                    if (!Mathf.Approximately(newSoftenAng, softenAng))
                    {
                        fow.InnerSoftenAngle = Mathf.Max(0, newSoftenAng);
                        Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                        fow.UpdateFogConfig();
                    }
                }
            }


            float oldExtraSightAmount = fow.SightExtraAmount;
            float newExtraSightAmount = EditorGUILayout.Slider("Revealer Extra Sight Distance: ", oldExtraSightAmount, -.01f, 1);
            if (!Mathf.Approximately(oldExtraSightAmount, newExtraSightAmount))
            {
                fow.SightExtraAmount = newExtraSightAmount;
                Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                fow.UpdateFogConfig();
            }

            float oldMaxDist = fow.MaxFogDistance;
            float newMaxDist = EditorGUILayout.Slider("Max Fog Distance: ", oldMaxDist, 0, 10000);
            if (!Mathf.Approximately(oldMaxDist, newMaxDist))
            {
                fow.MaxFogDistance = newMaxDist;
                Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                fow.UpdateFogConfig();
            }

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("------------------");
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Appearance Options:");

            FogOfWarWorld.FogOfWarAppearance fogAppearance = fow.GetFowAppearance();
            selected = (int)fogAppearance;
            selected = EditorGUILayout.Popup("Fog Appearance", selected, FogAppearanceOptions);
            fogAppearance = (FogOfWarWorld.FogOfWarAppearance)selected;
            if (fow.GetFowAppearance() != fogAppearance)
            {
                //fow.FogAppearance = fogAppearance;
                fow.SetFowAppearance(fogAppearance);
                Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
            }

            if (fow.GetFowAppearance() != FogOfWarWorld.FogOfWarAppearance.None)
            {
                Color unknownColor = fow.UnknownColor;
                Color newColor = EditorGUILayout.ColorField("Unknown Area Color: ", unknownColor);
                if (unknownColor != newColor)
                {
                    fow.UnknownColor = newColor;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.UpdateFogConfig();
                }
            }
            if (fow.GetFowAppearance() == FogOfWarWorld.FogOfWarAppearance.Solid_Color)
            {

            }
            else if (fow.GetFowAppearance() == FogOfWarWorld.FogOfWarAppearance.GrayScale)
            {

                float oldStrength = fow.SaturationStrength;
                float newStrength = EditorGUILayout.Slider("Unknown Area Saturation Strength: ", oldStrength, 0, 1);
                if (!Mathf.Approximately(oldStrength, newStrength))
                {
                    fow.SaturationStrength = newStrength;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.UpdateFogConfig();
                }
            }
            else if (fow.GetFowAppearance() == FogOfWarWorld.FogOfWarAppearance.Blur)
            {
                float oldBlur = fow.BlurStrength;
                float newBlur = EditorGUILayout.Slider("Unknown Area Blur Strength: ", oldBlur, -1, 1);
                if (!Mathf.Approximately(oldBlur, newBlur))
                {
                    fow.BlurStrength = newBlur;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.UpdateFogConfig();
                }

                //float oldBlurOffset = fow.blurPixelOffset;
                //float newBlurOffset = EditorGUILayout.Slider("Unknown Area Blur Pixel Offset: ", oldBlurOffset, 1.5f, 10);
                //if (oldBlurOffset != newBlurOffset)
                //{
                //    fow.blurPixelOffset = newBlurOffset;
                //    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                //    fow.updateFogConfiguration();
                //}
                float oldBlurOffset = fow.BlurDistanceScreenPercentMin;
                float newBlurOffset = EditorGUILayout.Slider("Min Screen Percent: ", oldBlurOffset, 0, 2);
                if (!Mathf.Approximately(oldBlurOffset, newBlurOffset))
                {
                    fow.BlurDistanceScreenPercentMin = newBlurOffset;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.UpdateFogConfig();
                }

                oldBlurOffset = fow.BlurDistanceScreenPercentMax;
                newBlurOffset = EditorGUILayout.Slider("Max Screen Percent: ", oldBlurOffset, 0, 2);
                if (oldBlurOffset != newBlurOffset)
                {
                    fow.BlurDistanceScreenPercentMax = newBlurOffset;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.UpdateFogConfig();
                }

                int oldBlurSamples = fow.BlurSamples;
                int newBlurSamples = EditorGUILayout.IntSlider("Num Blur Samples: ", oldBlurSamples, 6, 18);
                if (oldBlurSamples != newBlurSamples)
                {
                    fow.BlurSamples = newBlurSamples;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.UpdateFogConfig();
                }
            }
            else if (fow.GetFowAppearance() == FogOfWarWorld.FogOfWarAppearance.Texture_Sample)
            {
                Texture2D oldTexture = fow.FogTexture;
                Texture2D newTexture = (Texture2D)EditorGUILayout.ObjectField("Fog Of War Texture: ", oldTexture, typeof(Texture2D), false);
                if (newTexture != oldTexture)
                {
                    fow.FogTexture = newTexture;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.UpdateFogConfig();
                }

                Vector2 oldTiling = fow.FogTextureTiling;
                Vector2 newTiling = EditorGUILayout.Vector2Field("Texture Tiling: ", oldTiling);
                if (oldTiling != newTiling)
                {
                    fow.FogTextureTiling = newTiling;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.UpdateFogConfig();
                }

                float oldSpeed = fow.FogScrollSpeed;
                float newSpeed = EditorGUILayout.FloatField("Texture Scroll Speed: ", oldSpeed);
                if (!Mathf.Approximately(oldSpeed, newSpeed))
                {
                    fow.FogScrollSpeed = newSpeed;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.UpdateFogConfig();
                }
            }
            else if (fow.GetFowAppearance() == FogOfWarWorld.FogOfWarAppearance.Outline)
            {
                float oldThickness = fow.LineThickness;
                float newThickness = EditorGUILayout.FloatField("Outline Thickness: ", oldThickness);
                if (!Mathf.Approximately(oldThickness, newThickness))
                {
                    fow.LineThickness = newThickness;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.UpdateFogConfig();
                }
            }

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("------------------");
            EditorGUILayout.Space(20);

            FogOfWarWorld.FogSampleMode sampleMode = fow.FOWSamplingMode;
            selected = (int)sampleMode;
            selected = EditorGUILayout.Popup("Fog Sample Mode", selected, FowSampleOptions);
            sampleMode = (FogOfWarWorld.FogSampleMode)selected;
            if (fow.FOWSamplingMode != sampleMode)
            {
                fow.FOWSamplingMode = sampleMode;
                Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                fow.UpdateFogConfig();
            }
            if (fow.FOWSamplingMode == FogOfWarWorld.FogSampleMode.Texture || fow.FOWSamplingMode == FogOfWarWorld.FogSampleMode.Both)
            {
                EditorGUILayout.LabelField("---Texture Sampling Mode Options:");
                bool useConstantBlur = fow.UseConstantBlur;
                bool newUseConstantBlur = EditorGUILayout.Toggle("---Use Blur?", useConstantBlur);
                //bool newUseBounds = false;
                if (useConstantBlur != newUseConstantBlur)
                {
                    fow.UseConstantBlur = newUseConstantBlur;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.UpdateFogConfig();
                }
                if (newUseConstantBlur)
                {
                    int oldCBlurQual = fow.ConstantTextureBlurQuality;
                    int newCBlurQual = EditorGUILayout.IntSlider("---Texture Blur Quality: ", oldCBlurQual, 1, 6);
                    if (oldCBlurQual != newCBlurQual)
                    {
                        fow.ConstantTextureBlurQuality = newCBlurQual;
                        Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                        fow.UpdateFogConfig();
                    }
                    float oldCBlurAmmount = fow.ConstantTextureBlurAmount;
                    float newCBlurAmount = EditorGUILayout.Slider("---Texture Blur Amount: ", oldCBlurAmmount, 0, 5);
                    if (!Mathf.Approximately(oldCBlurAmmount, newCBlurAmount))
                    {
                        fow.ConstantTextureBlurAmount = newCBlurAmount;
                        Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                        fow.UpdateFogConfig();
                    }
                }

                EditorGUILayout.Space(20);
            }

            bool useWorldBounds = fow.UseWorldBounds;
            bool newUseBounds = EditorGUILayout.Toggle("Use World Bounds?", useWorldBounds);
            //bool newUseBounds = false;
            if (useWorldBounds != newUseBounds)
            {
                fow.UseWorldBounds = newUseBounds;
                Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                fow.UpdateFogConfig();
            }
            if (newUseBounds)
            {
                float boundSoftendistance = fow.WorldBoundsSoftenDistance;
                float newboundSoftendistance = EditorGUILayout.Slider("World Bounds Soften Distance:", boundSoftendistance, 0, 5);
                if (!Mathf.Approximately(boundSoftendistance, newboundSoftendistance))
                {
                    fow.WorldBoundsSoftenDistance = newboundSoftendistance;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.UpdateFogConfig();
                }

                float boundsInfluence = fow.WorldBoundsInfluence;
                float newboundsInfluence = EditorGUILayout.Slider("World Bounds Influence:", boundsInfluence, 0, 1);
                if (!Mathf.Approximately(boundsInfluence, newboundsInfluence))
                {
                    fow.WorldBoundsInfluence = newboundsInfluence;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.UpdateFogConfig();
                }
            }
            EditorGUILayout.Space(10);
            bool UseMiniMap = fow.UseMiniMap;
            bool newUseMiniMap = EditorGUILayout.Toggle("Enable Mini-Map?", UseMiniMap);
            //bool newUseBounds = false;
            if (UseMiniMap != newUseMiniMap)
            {
                fow.UseMiniMap = newUseMiniMap;
                Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                fow.UpdateFogConfig();
            }
            if (newUseMiniMap)
            {
                Color mapColor = fow.MiniMapColor;
                Color newMapColor = EditorGUILayout.ColorField("MiniMap Color: ", mapColor);
                if (mapColor != newMapColor)
                {
                    fow.MiniMapColor = newMapColor;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.UpdateFogConfig();
                }

                RawImage oldReference = fow.UIImage;
                RawImage newReference = (RawImage)EditorGUILayout.ObjectField("Minimap UI Raw Image Reference: ", oldReference, typeof(RawImage), true);
                if (newReference != oldReference)
                {
                    fow.UIImage = newReference;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.UpdateFogConfig();
                }
            }

            if (newUseBounds || newUseMiniMap || fow.FOWSamplingMode == FogOfWarWorld.FogSampleMode.Texture || fow.FOWSamplingMode == FogOfWarWorld.FogSampleMode.Both)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Bounds:");
                Vector3 WorldBoundsCenter = fow.WorldBounds.center;
                Vector3 newWorldBoundsCenter = EditorGUILayout.Vector3Field("--Center: ", WorldBoundsCenter);
                if (WorldBoundsCenter != newWorldBoundsCenter)
                {
                    fow.WorldBounds.center = newWorldBoundsCenter;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.InitFOWRT();
                    fow.UpdateFogConfig();
                }
                Vector3 WorldBoundsExtents = fow.WorldBounds.extents;
                Vector3 newWorldBoundsExtents = EditorGUILayout.Vector3Field("--Extents: ", WorldBoundsExtents);
                if (WorldBoundsExtents != newWorldBoundsExtents)
                {
                    fow.WorldBounds.extents = newWorldBoundsExtents;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.InitFOWRT();
                    fow.UpdateFogConfig();
                }
            }
            if (newUseMiniMap || fow.FOWSamplingMode == FogOfWarWorld.FogSampleMode.Texture || fow.FOWSamplingMode == FogOfWarWorld.FogSampleMode.Both)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("FOW Texture Options (either cause you are using a minimap, or because your sampling mode uses a texture, or both)");

                int MiniMapResX = fow.FowResX;
                int newMiniMapResX = EditorGUILayout.IntSlider("FOW Res X: ", MiniMapResX, 128, 2048);
                if (MiniMapResX != newMiniMapResX)
                {
                    fow.FowResX = newMiniMapResX;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.InitFOWRT();
                    fow.UpdateFogConfig();
                }
                int MiniMapResY = fow.FowResY;
                int newMiniMapResY = EditorGUILayout.IntSlider("FOW Res Y: ", MiniMapResY, 128, 2048);
                if (MiniMapResY != newMiniMapResY)
                {
                    fow.FowResY = newMiniMapResY;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.InitFOWRT();
                    fow.UpdateFogConfig();
                }
                bool useRegrow = fow.UseRegrow;
                bool newUseRegrow = EditorGUILayout.Toggle("Use Regrow?", useRegrow);
                //bool newUseBounds = false;
                if (useRegrow != newUseRegrow)
                {
                    fow.UseRegrow = newUseRegrow;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                    fow.InitFOWRT();
                    fow.UpdateFogConfig();
                }
                if (newUseRegrow)
                {
                    float oldRegrowSpeed = fow.FogRegrowSpeed;
                    float newRegrowSpeed = EditorGUILayout.Slider("Fog Regrow Speed: ", oldRegrowSpeed, 0, 10);
                    if (!Mathf.Approximately(oldRegrowSpeed, newRegrowSpeed))
                    {
                        fow.FogRegrowSpeed = newRegrowSpeed;
                        Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                        fow.UpdateFogConfig();
                    }
                    float oldInitVal = fow.InitialFogExplorationValue;
                    float newInitVal = EditorGUILayout.Slider("Initial Fog Exploration: ", oldInitVal, 0, 1);
                    if (!Mathf.Approximately(oldInitVal, newInitVal))
                    {
                        fow.InitialFogExplorationValue = newInitVal;
                        Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                        fow.UpdateFogConfig();
                    }
                    float oldRegrowMax = fow.MaxFogRegrowAmount;
                    float newRegrowMax = EditorGUILayout.Slider("Max Fog Regrow Amount: ", oldRegrowMax, 0, 1);
                    if (!Mathf.Approximately(oldRegrowMax, newRegrowMax))
                    {
                        fow.MaxFogRegrowAmount = newRegrowMax;
                        Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                        fow.UpdateFogConfig();
                    }
                }
            }

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("------------------");
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Utility Options (cant be changed at runtime)");

            FogOfWarWorld.RevealerUpdateMode revealerMode = fow.revealerMode;
            selected = (int)revealerMode;
            selected = EditorGUILayout.Popup("Revealer Mode", selected, RevealerModeOptions);
            revealerMode = (FogOfWarWorld.RevealerUpdateMode)selected;
            if (fow.revealerMode != revealerMode)
            {
                fow.revealerMode = revealerMode;
                Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
            }

            if (fow.revealerMode == FogOfWarWorld.RevealerUpdateMode.N_Per_Frame)
            {
                int numRevealersPerFrame = fow.numRevealersPerFrame;
                int newNumRevealersPerFrame = EditorGUILayout.IntField("Num Revealers Per Frame: ", numRevealersPerFrame);
                if (numRevealersPerFrame != newNumRevealersPerFrame)
                {
                    fow.numRevealersPerFrame = newNumRevealersPerFrame;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                }
            }

            int maxNumRevealers = fow.maxPossibleRevealers;
            int newmaxNumRevealers = EditorGUILayout.IntField("Max Num Revealers: ", maxNumRevealers);
            if (maxNumRevealers != newmaxNumRevealers)
            {
                fow.maxPossibleRevealers = newmaxNumRevealers;
                Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
            }

            int maxNumSegments = fow.maxPossibleSegmentsPerRevealer;
            int newmaxNumSegments = EditorGUILayout.IntField("Max Num Segments Per Revealer: ", maxNumSegments);
            if (newmaxNumSegments != maxNumSegments)
            {
                fow.maxPossibleSegmentsPerRevealer = newmaxNumSegments;
                Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
            }

            //bool oldAllowMinDist = fow.AllowMinimumDistance;
            //bool newAllowMinDist = EditorGUILayout.Toggle("Enable Minimum Distance To Revealers? ", oldAllowMinDist);
            //if (oldAllowMinDist != newAllowMinDist)
            //{
            //    fow.AllowMinimumDistance = newAllowMinDist;
            //    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
            //    fow.UpdateFogConfig();
            //}

            bool is2d = fow.is2D;
            bool new2d = EditorGUILayout.Toggle("Is 2D?", is2d);
            if (is2d != new2d)
            {
                fow.is2D = new2d;
                Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                fow.UpdateFogConfig();
            }

            if (!new2d)
            {
                FogOfWarWorld.GamePlane plane = fow.gamePlane;
                selected = (int)plane;
                selected = EditorGUILayout.Popup("Game Plane", selected, GamePlaneOptions);
                plane = (FogOfWarWorld.GamePlane)selected;
                if (fow.gamePlane != plane)
                {
                    fow.gamePlane = plane;
                    Undo.RegisterCompleteObjectUndo(fow, "Change FOW parameters");
                }
            }
        }
    }
#endif
}
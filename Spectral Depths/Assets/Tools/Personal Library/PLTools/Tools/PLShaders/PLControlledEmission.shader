Shader "Universal Render Pipeline/Lit/PLControlledEmission"
{
    Properties
    {
        [Header("Main Texture")]
        _BaseMap("Texture Sample 0", 2D) = "white" {}

        [Header("Main Color")]
        _BaseColor("Base Color", Color) = (1,1,1,1)

        [Header("Emission")]
        _EmissionForce("Emission Force", Float) = 0
        [Toggle(_USEEMISSIONFRESNEL_ON)] _UseEmissionFresnel("Use Emission Fresnel", Float) = 0
        _EmissionFresnelBias("Emission Fresnel Bias", Float) = 1
        _EmissionFresnelScale("Emission Fresnel Scale", Float) = 1
        _EmissionFresnelPower("Emission Fresnel Power", Float) = 1
        _EmissionColor("Emission Color", Color) = (1,1,1,1)

        [Header("Opacity")]
        _Opacity("Opacity", Range(0, 1)) = 1
        [Toggle(_USEOPACITYFRESNEL_ON)] _UseOpacityFresnel("Use Opacity Fresnel", Float) = 0
        [Toggle(_INVERTOPACITYFRESNEL_ON)] _InvertOpacityFresnel("Invert Opacity Fresnel", Float) = 0
        _OpacityFresnelBias("Opacity Fresnel Bias", Float) = 1
        _OpacityFresnelScale("Opacity Fresnel Scale", Float) = 1
        _OpacityFresnelPower("Opacity Fresnel Power", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Overlay"
        }
        LOD 100

        Pass
        {
            Name "UniversalRenderPipeline"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma surface surf Standard fullforwardshadows

            #pragma target 3.0
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #include "Packages/UniversalRP/Runtime/ShaderLibrary/UnityShaderVariables.hlsl"
            #include "Packages/UniversalRP/Runtime/ShaderLibrary/UnityPerMaterial.hlsl"
            #include "Packages/UniversalRP/Runtime/ShaderLibrary/UnityInstancing.hlsl"
            #include "Packages/UniversalRP/Runtime/ShaderLibrary/UnityShaderVariablesLighting.hlsl"

            struct Input
            {
                float2 uv_BaseMap;
                float3 worldPos;
                INTERNAL_DATA
            };

            sampler2D _BaseMap;
            float4 _BaseColor;
            float _EmissionForce;
            float4 _EmissionColor;
            float _UseEmissionFresnel;
            float _EmissionFresnelBias;
            float _EmissionFresnelScale;
            float _EmissionFresnelPower;
            float _Opacity;
            float _UseOpacityFresnel;
            float _InvertOpacityFresnel;
            float _OpacityFresnelBias;
            float _OpacityFresnelScale;
            float _OpacityFresnelPower;

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                float fresnelNdotV8 = dot(IN.worldNormal, UnityWorldSpaceViewDir(IN.worldPos));
                float fresnelNode8 = (_EmissionFresnelBias + _EmissionFresnelScale * pow(1.0 - fresnelNdotV8, _EmissionFresnelPower));

                #ifdef _USEEMISSIONFRESNEL_ON
                float staticSwitch22 = fresnelNode8;
                #else
                float staticSwitch22 = 1.0;
                #endif

                o.Emission = (_EmissionForce * _EmissionColor * staticSwitch22).rgb;

                float fresnelNdotV26 = dot(IN.worldNormal, UnityWorldSpaceViewDir(IN.worldPos));
                float fresnelNode26 = (_OpacityFresnelBias + _OpacityFresnelScale * pow(1.0 - fresnelNdotV26, _OpacityFresnelPower));

                #ifdef _INVERTOPACITYFRESNEL_ON
                float staticSwitch31 = (1.0 - fresnelNode26);
                #else
                float staticSwitch31 = fresnelNode26;
                #endif

                #ifdef _USEOPACITYFRESNEL_ON
                float staticSwitch27 = staticSwitch31;
                #else
                float staticSwitch27 = 1.0;
                #endif

                o.Alpha = (staticSwitch27 * _Opacity * _BaseColor.a);
            }
            ENDCG
        }
    }
}

Shader "Hidden/FullScreen/FOW/TextureSample"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _fowTexture("Texture", 2D) = "white" {}
    }
    HLSLINCLUDE

        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 uv         : TEXCOORD0;
            //UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float2 uv         : TEXCOORD0;
            //UNITY_VERTEX_OUTPUT_STEREO
        };
        
    ENDHLSL

    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma multi_compile_local PLANE_XZ PLANE_XY PLANE_ZY
            #pragma multi_compile_local IS_2D IS_3D

            #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
            //unity 2020 normal texture is VS, not WS. you can just remove this if you care about the extra varients.
            #pragma multi_compile_fragment _ _VS_NORMAL

            #pragma vertex Vert
            #pragma fragment Frag

            #include "FogOfWarLogic.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;
            
            float4x4 _inverseProjectionMatrix;

            float _maxDistance;
            sampler2D _fowTexture;
            float2 _fowTiling;
            float _fowScrollSpeed;
            float4 _unKnownColor;

            float4x4 _camToWorldMatrix;

            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            float4 Frag (Varyings i) : SV_Target
            {
                float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                float2 pos;
                float height;
#if IS_2D
                
                pos = (i.uv * float2(2,2) - float2(1,1)) * _cameraSize * float2(_MainTex_TexelSize.z/ _MainTex_TexelSize.w,1);
                pos+= _cameraPosition;
                Unity_Rotate_Degrees_float(pos, _cameraPosition, -_cameraRotation, pos);
                height = 0;
                float2 uvSample = pos + (_Time * _fowScrollSpeed);
                float4 fog = tex2D(_fowTexture, uvSample * _fowTiling);
#elif IS_3D
                float2 uv = i.uv;

            #if UNITY_REVERSED_Z
                real depth = SampleSceneDepth(i.uv);
            #else
                real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(i.uv));
            #endif

                float3 vpos = ComputeViewSpacePosition(i.uv, depth, UNITY_MATRIX_I_P);
                if (vpos.z > _maxDistance)
                    return color;

                vpos.z*=-1;   //unity can suck my.....
                float4 worldPos = mul(_camToWorldMatrix, float4(vpos, 1));
                //return float4(worldPos.x, worldPos.y, worldPos.z,1);

                float3 normal = SampleSceneNormals(uv);
            #if _VS_NORMAL
                //this was required in unity 2020.3.28. when updating to 2020.3.48, its no longer required. not sure what version fixes it exactly.
                normal.z*=-1;   //unity can suck my.....
                normal = mul((float3x3)_camToWorldMatrix, normal);
                return float4(1, 1, 1, 1);
            #endif
                //return float4(normal.x, normal.y, normal.z, 1);

                float3 powResult = pow(abs(normal), 8);
                float dotResult = dot(powResult, float3(1, 1, 1));
                //float3 lerpVals = round(powResult / dotResult);
                float3 lerpVals = (powResult / dotResult);
                //uvSample = lerp(lerp(worldPos.xz, worldPos.yz, lerpVals.x), worldPos.xy, lerpVals.z) + (_Time * _fowScrollSpeed);
                float2 uvSample1 = worldPos.yz + (_Time * _fowScrollSpeed);
                float2 uvSample2 = worldPos.xz + (_Time * _fowScrollSpeed);
                float2 uvSample3 = worldPos.xy + (_Time * _fowScrollSpeed);
                float4 fog = tex2D(_fowTexture, uvSample1 * _fowTiling) * lerpVals.x;
                fog += tex2D(_fowTexture, uvSample2 * _fowTiling) * lerpVals.y;
                fog += tex2D(_fowTexture, uvSample3 * _fowTiling) * lerpVals.z;
    #if PLANE_XZ
                pos = worldPos.xz;
                height = worldPos.y;
    #elif PLANE_XY
                pos = worldPos.xy;
                height = worldPos.z;
    #elif PLANE_ZY
                pos = worldPos.zy;            
                height = worldPos.x;            
    #endif

#endif

                float coneCheckOut;
#if HARD
                FOW_Hard_float(pos, height, coneCheckOut);
#elif SOFT
                FOW_Soft_float(pos, height, coneCheckOut);
#endif
                CustomCurve_float(coneCheckOut, coneCheckOut);
                TextureSample(pos, coneCheckOut);

                //float4 fog = tex2D(_fowTexture, uvSample * _fowTiling) * lerpVals.x;
                fog = lerp(color, fog, _unKnownColor.w);
                OutOfBoundsCheck(pos, color);
                OutOfBoundsCheck(pos, fog);
                return float4(lerp(fog.rgb * _unKnownColor, color.rgb, coneCheckOut), color.a);
            }
            ENDHLSL
        }
    }
}
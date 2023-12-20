Shader "Hidden/FullScreen/FOW/FOW_RT"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma multi_compile_local _ USE_REGROW
            #pragma multi_compile_local _ IGNORE_HEIGHT

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "FogOfWarLogic.hlsl"
            //#include "../FogOfWarLogic.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;

            float4x4 _camToWorldMatrix;
            float4x4 _inverseProjectionMatrix;
			
            float4 _unKnownColor;


            fixed4 frag (v2f i) : SV_Target
            {
                //fixed4 color = tex2D(_MainTex, i.uv);

                float coneCheckOut;
				float2 pos = float2(((i.uv.x-.5) * _worldBounds.x) + _worldBounds.y, ((i.uv.y-.5) * _worldBounds.z) + _worldBounds.w);

                //return float4(pos.x, pos.y, 0,1);
#if HARD
                FOW_Hard_float(pos, 0, coneCheckOut);
#elif SOFT
                FOW_Soft_float(pos, 0, coneCheckOut);
#endif

                CustomCurve_float(coneCheckOut, coneCheckOut);
#if USE_REGROW
                //return tex2D(_MainTex, i.uv);
                float opacitySample = tex2D(_MainTex, i.uv).w;
                //return opacitySample;
                //opacitySample -= unity_DeltaTime.x;
                coneCheckOut = max(coneCheckOut, opacitySample);
                //coneCheckOut = opacitySample;
                coneCheckOut = clamp(coneCheckOut, 0, 1);
                //coneCheckOut = .5;
#endif

				//return float4(coneCheckOut,coneCheckOut,coneCheckOut,1);
                //return float4(lerp(color.rgb * _unKnownColor, color.rgb, coneCheckOut), color.a);
                return float4(_unKnownColor.rgb, (1 - coneCheckOut));
                return float4(_unKnownColor.rgb, (1 - coneCheckOut) * _unKnownColor.a);
            }
            ENDCG
        }
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _regrowSpeed;
            float _maxRegrowAmount;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);
                color.w = 1 - color.w;
                //color.w-= unity_DeltaTime.z;
                if (color.w > _maxRegrowAmount)
                {
                    color.w -= unity_DeltaTime.x * _regrowSpeed;
                    color.w = clamp(color.w, _maxRegrowAmount, 1);
                }
                
                return color;
            }
            ENDCG
        }
    }
}

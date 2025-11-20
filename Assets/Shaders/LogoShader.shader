Shader "Unlit/LogoShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("NoiseTexture", 2D) = "white" {}
        _ScrollSpeedY ("ScrollSpeedY", float) = 0.5
        _DistortionStrength ("DistortionStrength", float) = 0.5
        _ShineStrength ("ShineStrength", float) = 0.5
        _Color ("_Color", Color) = (1,1,1,1)
        _IsShine ("_IsShine", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            float _ScrollSpeedY;
            float _DistortionStrength;
            float _ShineStrength;
            float _IsShine;
            float4 _Color;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float2 NoiseUV = uv * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
                NoiseUV.y += _Time.y * _ScrollSpeedY;
                float noise = tex2D(_NoiseTex,NoiseUV).r;
                float noiseOffset = noise - 0.5f;

                float2 distortion = noiseOffset * _DistortionStrength;
                float2 distortionUV = uv + distortion;

                // sample the texture
                fixed4 col = tex2D(_MainTex, distortionUV);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                fixed4 finalColor =  col * _Color ;
                finalColor.a *= col.r;
                float sineWave = sin(_Time.y *_ShineStrength) * 0.5 + 0.5;
                float shineAlpha = sineWave * 0.6 + 0.4;
                if(_IsShine >= 1.0f)
                {
                    finalColor.a *=shineAlpha;
                }
                return finalColor;
            }
            ENDCG
        }
    }
}

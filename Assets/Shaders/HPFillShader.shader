Shader "Unlit/HPFillShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTexture ("NoiseTexture", 2D) = "white" {}
    
        _FillColor ("FillColor", Color) = (1,1,1,1)
        _NoiseColor ("NoiseColor", Color) = (1,1,1,1)
        _Intesity ("Intesity", float) = 1
    
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
            sampler2D _NoiseTexture;
            float4 _NoiseTexture_ST;

            float4 _FillColor;
            float4 _NoiseColor;
            float _Intesity;


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
                
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col.a = col.r *  col.a; 


                float2 noiseUV1 = i.uv + float2(1 - (_Time.y * 0.05f),0);
                float2 noiseUV2 = i.uv + float2(0,1 - (_Time.y * 0.1f));
                float noise1 = tex2D(_NoiseTexture, noiseUV1).r;
                float noise2 = tex2D(_NoiseTexture, noiseUV2).r;
                float noise = noise1* noise2;

                fixed4 finalCol = 0;
                finalCol.rgb = (_FillColor.rgb + noise * _NoiseColor*  _Intesity) * (col.b);
                finalCol.a = col.a;

                
                return finalCol;
            }
            ENDCG
        }
    }
}

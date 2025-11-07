Shader "Unlit/HPFillShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTexture ("NoiseTexture", 2D) = "white" {}
    
        _FillColor ("FillColor", Color) = (1,1,1,1)
        _NoiseColor ("NoiseColor", Color) = (1,1,1,1)
        _NoiseIntesity ("NoiseIntesity", float) = 1

        _HPPercent ("_HPPercent", float) = 0.5
        _HPOffset ("_HPOffset", float) = 0.1
        _FillEdgeColor ("_FillEdgeColor", Color) = (1,1,1,1)
        _EdgeIntesity ("_EdgeIntesity", float) = 1
        _EdgeTextureTiling ("_EdgeTextureTiling", Vector) = (1,1,0,0)

        _BloodTex ("_BloodTex", 2D) = "white" {}
        _HPDifPercent ("_HPDifPercent", float) = 1
        _BloodTexTiling ("_BloodTexTiling", Vector) = (1,1,0,0)
        _BloodColor ("_BloodColor", Color) = (1,1,1,1)




    
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
            sampler2D _BloodTex;
            float4 _BloodTex_ST;

            float4 _FillColor;
            float4 _NoiseColor;
            float _NoiseIntesity;

            float _HPPercent;
            float _HPOffset;
            float4 _FillEdgeColor;
            float _EdgeIntesity;
            float4 _EdgeTextureTiling;

            float _HPDifPercent;
            float4 _BloodTexTiling;
            float4 _BloodColor;


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



                float2 dissolveNoiseUV = i.uv * _EdgeTextureTiling.rg + float2(1 - (_Time.y * 0.5f),0);
                float dissolveNoise = tex2D(_NoiseTexture, dissolveNoiseUV).r;

                float HPDissolve =  smoothstep(_HPPercent + _HPOffset,_HPPercent,i.uv.r);
                float HPDissolveEdge = smoothstep(_HPPercent ,_HPPercent + 0.25f,i.uv.r);
                float dissolveAlpha = 1- step(HPDissolve,dissolveNoise);

                float3 Edge = HPDissolveEdge * dissolveAlpha * _FillEdgeColor.rgb * _EdgeIntesity;

                float2 BloodUV = i.uv * _EdgeTextureTiling.rg;
                float BloodCol = tex2D(_NoiseTexture, BloodUV).r ;
                float BloodDissolve = (1 - step(BloodCol,smoothstep(_HPPercent,_HPPercent + _HPDifPercent,i.uv.r))) * 1- dissolveAlpha;

                fixed4 HPCol = 0;
                noise * dissolveAlpha;
                HPCol.rgb = _FillColor.rgb + noise * _NoiseColor * _NoiseIntesity;
                HPCol.a = col.a;

                HPCol.rgb = (HPCol.rgb + Edge ) * (col.b);
                HPCol.rgb -= BloodDissolve;

                HPCol.a =HPCol.a * (dissolveAlpha + BloodDissolve);
                return HPCol;
                //return fixed4(BloodDissolve,BloodDissolve,BloodDissolve,BloodDissolve);
            }
            ENDCG
        }
    }
}

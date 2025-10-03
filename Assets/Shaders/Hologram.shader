Shader "Unlit/Hologram"
{
    Properties
    {
        _MainTex ("MainTexture", 2D) = "white" {}
        _DissolveTex ("DissloveTexture", 2D) = "white" {}
        _AdjustDissolve ("Adjust Dissolve", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
            sampler2D _DissolveTex;
            half _AdjustDissolve;
            float4 _MainTex_ST;


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
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                half dissolveValue = tex2D(_DissolveTex,i.uv).r;
                clip(dissolveValue - _AdjustDissolve);

                col.rgb = col.rgb * dissolveValue;
                return col ;
            }
            ENDCG
            ]
        }
    }
}

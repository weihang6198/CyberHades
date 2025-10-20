Shader "Unlit/PostEffects"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScreenTint ("Screen Tint", Color) = (1,1,1,1)
    }
    CGINCLUDE
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

        sampler2D _MainTex;
        float4 _MainTex_ST;
        fixed4 _ScreenTint;

        v2f vert (appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            return o;
        }
    ENDCG
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass //0
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 ScreenTint = _ScreenTint;

                fixed4 col = tex2D(_MainTex, i.uv);
                    
                return col * ScreenTint;
            }
            ENDCG
        }
    }
}

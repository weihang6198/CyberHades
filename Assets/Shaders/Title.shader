Shader "Unlit/Title"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlendMode ("Blend Mode", Float) = 0 // 0 = Normal, 1 = Multiply
        _MaskOpacity ("Mask Opacity", Float) = 0 
        _ShineColor ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanvasModulateColor"="True"
        }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

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
            float4 _MainTex_ST;
            float _BlendMode;
            float _MaskOpacity;
            fixed4 _ShineColor;

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
                
                fixed4 blended;
                
               blended.rgb = col.g ;
               float t = sin(_Time.y * 2.0); // oscillates smoothly
               float shine = saturate((_MaskOpacity * t) * 0.5 + 0.5); //
               blended.rgb += col.r * _ShineColor.rgb * shine;
               blended.a = col.a;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, blended);
                return fixed4(blended);
            }
            ENDCG
        }
    }
}

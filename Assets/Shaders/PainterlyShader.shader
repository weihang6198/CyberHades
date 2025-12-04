Shader "Custom/PainterlyShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        [HDR]_SpecularColor("Specular color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [Normal]_Normal("Normal", 2D) = "bump" {}
        _NormalStrength("Normal strength", Range(-2, 2)) = 1

        _OMR ("OcclusionMetallicRoughness (OMR)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _Emissive ("Emissive", 2D) = "black" {}
        _EmissiveStrength ("EmissiveStrength", Range(0,10)) = 1.0

        _ShadingGradient("Shading gradient", 2D) = "white" {}
        _PainterlyGuide("Painterly guide", 2D) = "white" {}
        _PainterlySmoothness("Painterly smoothness", Range(0, 1)) = 0.1
    }

    SubShader
    {
        Tags 
        { 
            "RenderPipeline"="UniversalPipeline" 
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl" // <-- NEW: For URP light functions
            
            // NOTE: You can remove the TEXTURE2D and SAMPLER macros and use 
            // the new URP format: TEXTURE2D_X(_MainTex, sampler_MainTex);
            // I'll stick to your current format for minimal changes.

            TEXTURE2D(_MainTex);            SAMPLER(sampler_MainTex);
            TEXTURE2D(_Normal);             SAMPLER(sampler_Normal);
            TEXTURE2D(_ShadingGradient);    SAMPLER(sampler_ShadingGradient);
            TEXTURE2D(_PainterlyGuide);     SAMPLER(sampler_PainterlyGuide);
            TEXTURE2D(_OMR);     SAMPLER(sampler_OMR);
            TEXTURE2D(_Emissive);     SAMPLER(sampler_Emissive);

            float4 _MainTex_ST;
            float4 _Normal_ST;
            float4 _PainterlyGuide_ST;
            float4 _ShadingGradient_ST;


            float4 _Color;
            float4 _SpecularColor;
            float _NormalStrength;
            float _Glossiness;
            float _Metallic;
            float _PainterlySmoothness;
            float _EmissiveStrength;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 tangentOS  : TANGENT;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 worldPos   : TEXCOORD1;
                // Using TANGENT_TO_WORLD_SPACE for correct URP convention
                float3 t          : TEXCOORD2;
                float3 b          : TEXCOORD3;
                float3 n          : TEXCOORD4;
                float3 viewDirWS  : TEXCOORD5; // View direction in world space
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                // URP/Core.hlsl provides these transformation macros/functions
                // They handle the coordinate system changes correctly.
                //VertexPositionInputs vertexInput = Get=>WorldSpacePosition(IN.positionOS.xyz);
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz); // <-- Correct URP helper function call
                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                float3 tangentWS = TransformObjectToWorldDir(IN.tangentOS.xyz);
                float tangentSign = IN.tangentOS.w;

                // Recalculate bitangent/normalize (good practice)
                tangentWS = normalize(tangentWS - normalWS * dot(normalWS, tangentWS));
                float3 bitangentWS = cross(normalWS, tangentWS) * tangentSign;

                OUT.worldPos = vertexInput.positionWS; // Get world position from URP helper
                
                OUT.t = tangentWS;
                OUT.b = bitangentWS;
                OUT.n = normalize(normalWS);

                OUT.uv = IN.uv;
                OUT.positionCS = vertexInput.positionCS; // Get clip position from URP helper

                // Calculate view direction using URP's _WorldSpaceCameraPos substitute
                // The URP version of view direction calculation is more complex due to SRP Batcher,
                // but we can use the position of the camera in world space for simplicity here.
                // URP stores camera position in _WorldSpaceCameraPos in an optimized way.
                float3 viewDir = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);
                OUT.viewDirWS = viewDir;
                
                return OUT;
            }
            
            float3 UnpackNormalWithScaleTex(float4 packed, float scale)
            {
                float3 n;
                n.xy = packed.xy * 2.0 - 1.0;
                n.xy *= scale;
                n.z = sqrt(saturate(1.0 - dot(n.xy, n.xy)));
                return normalize(n);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 albedoSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color;
                float3 albedo = albedoSample.rgb;

                // --- URP LIGHTING START ---
                // Fetch the main light data (Directional Light)
                Light mainLight = GetMainLight();
                float3 lightDirWS = mainLight.direction;
                float3 lightColor = mainLight.color;
                // --- URP LIGHTING END ---
                
                float painterGuide = SAMPLE_TEXTURE2D(_PainterlyGuide, sampler_PainterlyGuide,_PainterlyGuide_ST.zw +_PainterlyGuide_ST.xy * IN.uv).r;

                float4 nSample = SAMPLE_TEXTURE2D(_Normal, sampler_Normal, IN.uv);
                float3 nTS = UnpackNormalWithScaleTex(nSample, _NormalStrength);

                float3 normalWS = normalize(
                    nTS.x * IN.t +
                    nTS.y * IN.b +
                    nTS.z * IN.n
                );

                float3 viewDir = normalize(IN.viewDirWS);

                float nDotL = saturate(dot(normalWS, lightDirWS) + 0.2);

                float diff = smoothstep(
                    painterGuide - _PainterlySmoothness,
                    painterGuide + _PainterlySmoothness,
                    nDotL
                );

                float3 refl = reflect(-lightDirWS, normalWS);
                float vDotRefl = saturate(dot(viewDir, refl));

                float specThreshold = painterGuide + _Glossiness;
                float specFactor = smoothstep(
                    specThreshold - _PainterlySmoothness,
                    specThreshold + _PainterlySmoothness,
                    vDotRefl
                );

                // Use URP's lightColor instead of legacy _LightColor0
                float3 specular = _SpecularColor.rgb * lightColor * specFactor * _Glossiness;

                float atten = smoothstep(
                    painterGuide - _PainterlySmoothness,
                    painterGuide + _PainterlySmoothness,
                    1.0
                );

                float2 gradUV = float2(diff, 0.5);
                float3 gradCol = SAMPLE_TEXTURE2D(_ShadingGradient, sampler_ShadingGradient, gradUV).rgb;
                float3 emissive = SAMPLE_TEXTURE2D(_Emissive, sampler_Emissive, IN.uv).rgb;

                // Use URP's lightColor instead of legacy _LightColor0
                float3 litColor = (albedo * gradCol * lightColor + specular + emissive* _EmissiveStrength) * atten;

                return float4(litColor, albedoSample.a);
            }

            ENDHLSL
        }   
    }
}
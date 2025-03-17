Shader "Custom/URP_Lit_Terrain"
{
    Properties
    {
        _BaseMap ("Base Texture", 2D) = "white" {}
        _HeightMap ("Height Map", 2D) = "black" {}
        _ExtrusionAmount ("Extrusion Amount", Range(0, 1)) = 0.1
        _Steps ("Step Layers", Int) = 10
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalRenderPipeline" }
        LOD 300

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
            };

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D(_HeightMap); SAMPLER(sampler_HeightMap);
            float _ExtrusionAmount;
            int _Steps;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float height = SAMPLE_TEXTURE2D(_HeightMap, sampler_HeightMap, IN.uv).r;
                float stepHeight = round(height * _Steps) / _Steps; // Step-based extrusion
                IN.positionOS.y += stepHeight * _ExtrusionAmount;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                Light mainLight = GetMainLight();
                half3 lighting = max(0, dot(normalize(float3(0, 1, 0)), mainLight.direction)) * mainLight.color;
                return half4(baseColor.rgb * lighting, baseColor.a);
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

            HLSLPROGRAM
            #pragma vertex vertShadow
            #pragma fragment fragShadow
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            TEXTURE2D(_HeightMap); SAMPLER(sampler_HeightMap);
            float _ExtrusionAmount;
            int _Steps;

            Varyings vertShadow(Attributes IN)
            {
                Varyings OUT;
                float height = SAMPLE_TEXTURE2D(_HeightMap, sampler_HeightMap, IN.uv).r;
                float stepHeight = round(height * _Steps) / _Steps;
                IN.positionOS.y += stepHeight * _ExtrusionAmount;
                OUT.positionHCS = TransformWorldToHClip(TransformObjectToWorld(IN.positionOS.xyz));
                return OUT;
            }

            half4 fragShadow(Varyings IN) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
    }
}
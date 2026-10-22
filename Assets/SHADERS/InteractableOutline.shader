
Shader "GEAR UP/Interactable Outline"
{
    Properties
    {
        _OutlineColor ("Cor do Contorno", Color) = (1, 0.8, 0.05, 1)
        _OutlineThickness ("Espessura em Pixels", Float) = 3
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry+1"
        }

        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            Cull Front
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineThickness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;

                float3 positionWS =
                    TransformObjectToWorld(input.positionOS.xyz);

                float3 normalWS =
                    TransformObjectToWorldNormal(input.normalOS);

                float3 normalVS =
                    TransformWorldToViewDir(normalWS, true);

                float4 positionCS =
                    TransformWorldToHClip(positionWS);

                float3 normalCS =
                    mul((float3x3)UNITY_MATRIX_P, normalVS);

                float2 direction = normalCS.xy;
                float len = length(direction);

                if (len > 0.0001)
                    direction /= len;
                else
                    direction = float2(0, 0);

                positionCS.xy +=
                    direction *
                    (2.0 / _ScreenParams.xy) *
                    _OutlineThickness *
                    positionCS.w;

                output.positionCS = positionCS;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}

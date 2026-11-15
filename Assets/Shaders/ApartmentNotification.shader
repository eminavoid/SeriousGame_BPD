// Pulso corto de notificación sobre la pantalla del celular. Sin bloom propio: el brillo pasa el umbral del volumen que ya tiene la casa.
Shader "Apartment/Notification"
{
    Properties
    {
        [MainTexture] _BaseMap("Screen", 2D) = "black" {}
        _IdleColor("Quiet tint", Color) = (1.15, 1.2, 1.15, 1)
        [HDR] _GlowColor("Notification", Color) = (0.62, 1, 0.78, 1)
        _Strength("Glow strength", Range(0, 4)) = 2.6
        _Speed("Pulse speed", Range(0, 8)) = 3.2
        _Notify("Notification on", Range(0, 1)) = 1
        _PlanarSize("Object size XZ", Vector) = (0.08, 0.165, 0, 0)
        _BannerCenter("Banner center", Vector) = (0.5, 0.82, 0, 0)
        _BannerSize("Banner size", Vector) = (0.32, 0.09, 0, 0)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _IdleColor;
                half4 _GlowColor;
                half _Strength;
                half _Speed;
                half _Notify;
                float4 _PlanarSize;
                float4 _BannerCenter;
                float4 _BannerSize;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionOS : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionOS = input.positionOS.xyz;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float2 size = max(_PlanarSize.xy, float2(0.001, 0.001));
                float2 uv = input.positionOS.xz / size + 0.5;
                half3 screen = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv).rgb;

                half wave = 0.4h + 0.6h * sin(_Time.y * _Speed);
                half notify = saturate(_Notify) * wave;

                float2 bannerUv = (uv - _BannerCenter.xy) / max(_BannerSize.xy, float2(0.001, 0.001));
                half banner = saturate(1.0h - (half)dot(bannerUv, bannerUv));
                banner = smoothstep(0.2h, 0.65h, banner);

                half3 color = screen * _IdleColor.rgb * (1.0h + notify * 0.22h);
                color += _GlowColor.rgb * _Strength * banner * notify;
                return half4(color, 1);
            }
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            ZWrite On
            ColorMask R
            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex DepthVert
            #pragma fragment DepthFrag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings { float4 positionCS : SV_POSITION; };
            Varyings DepthVert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }
            half DepthFrag(Varyings input) : SV_Target { return input.positionCS.z; }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Unlit"
}

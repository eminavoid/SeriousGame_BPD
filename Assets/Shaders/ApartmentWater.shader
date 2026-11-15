// Lámina de agua: transparente, fría, con una onda suave y un reflejo de la luz. Sin normal map.
Shader "Apartment/Water"
{
    Properties
    {
        [MainColor] _DeepColor("Deep", Color) = (0.18, 0.40, 0.48, 1)
        _ShallowColor("Shallow", Color) = (0.55, 0.78, 0.82, 1)
        _ReflectColor("Reflection", Color) = (0.72, 0.82, 0.86, 1)
        _SpecColor("Specular", Color) = (0.42, 0.48, 0.5, 1)
        _Opacity("Opacity", Range(0, 1)) = 0.62
        _Smoothness("Smoothness", Range(0, 1)) = 0.86
        _Frequency("Waves per meter", Range(1, 20)) = 8
        _Ripple("Ripple", Range(0, 0.15)) = 0.035
        _Speed("Speed", Range(0, 3)) = 0.85
        _Fresnel("Reflection", Range(0, 1)) = 0.55
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "UniversalMaterialType" = "SimpleLit"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _CLUSTER_LIGHT_LOOP
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_instancing
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Fog.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _DeepColor;
                half4 _ShallowColor;
                half4 _ReflectColor;
                half4 _SpecColor;
                half _Opacity;
                half _Smoothness;
                half _Frequency;
                half _Ripple;
                half _Speed;
                half _Fresnel;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                half3 normalWS : TEXCOORD1;
                half fogFactor : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                VertexPositionInputs vertex = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertex.positionCS;
                output.positionWS = vertex.positionWS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
#if defined(_FOG_FRAGMENT)
                output.fogFactor = 0;
#else
                output.fogFactor = ComputeFogFactor(vertex.positionCS.z);
#endif
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float3 viewDir = GetWorldSpaceNormalizeViewDir(input.positionWS);
                float2 p = input.positionWS.xz;
                float t = _Time.y * _Speed;
                float f = max(_Frequency, 0.001);

                float s1 = sin(p.x * f * 1.6 + t);
                float s2 = sin(p.y * f * 1.25 - t * 0.85);
                float s3 = sin((p.x + p.y) * f * 0.85 + t * 0.55);
                float h = (s1 + s2 + s3) * 0.333;

                float dx = cos(p.x * f * 1.6 + t) * f * 1.6 + cos((p.x + p.y) * f * 0.85 + t * 0.55) * f * 0.85;
                float dz = cos(p.y * f * 1.25 - t * 0.85) * f * 1.25 + cos((p.x + p.y) * f * 0.85 + t * 0.55) * f * 0.85;
                float3 face = normalize(float3(-dx * _Ripple, 1.0, -dz * _Ripple));
                face = normalize(lerp(normalize(input.normalWS), face, 0.85));

                half fresnel = pow(1.0h - saturate((half)dot(face, viewDir)), 4.0h);
                half crest = saturate(h * 0.5h + 0.5h);
                half3 albedo = lerp(_DeepColor.rgb, _ShallowColor.rgb, crest);
                albedo = lerp(albedo, _ReflectColor.rgb, fresnel * _Fresnel);
                albedo *= lerp(0.9h, 1.08h, crest);

                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;
                inputData.positionCS = input.positionCS;
                inputData.normalWS = face;
                inputData.viewDirectionWS = viewDir;
#if defined(MAIN_LIGHT_CALCULATE_SHADOWS)
                inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
#else
                inputData.shadowCoord = float4(0, 0, 0, 0);
#endif
                inputData.fogCoord = InitializeInputDataFog(float4(input.positionWS, 1), input.fogFactor);
                inputData.bakedGI = SampleSH(face);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
                inputData.shadowMask = half4(1, 1, 1, 1);
                inputData.tangentToWorld = half3x3(half3(1, 0, 0), half3(0, 1, 0), half3(0, 0, 1));

                half alpha = lerp(_Opacity, saturate(_Opacity + 0.28h), fresnel);

                SurfaceData surface = (SurfaceData)0;
                surface.albedo = albedo;
                surface.specular = _SpecColor.rgb;
                surface.smoothness = _Smoothness;
                surface.normalTS = half3(0, 0, 1);
                surface.occlusion = 1;
                surface.alpha = alpha;
                surface.clearCoatSmoothness = 1;

                half4 color = UniversalFragmentBlinnPhong(inputData, surface);
                color.rgb = MixFog(color.rgb, inputData.fogCoord);
                return half4(color.rgb, alpha);
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Simple Lit"
}

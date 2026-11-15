// Vidrio transparente con un borde de Fresnel. No es un cristal limpio: deja ver poco, para que no se abra un hueco al cielo.
Shader "Apartment/Clear"
{
    Properties
    {
        [MainColor] _BaseColor("Tint", Color) = (0.7, 0.82, 0.88, 1)
        _ReflectColor("Reflection", Color) = (0.78, 0.86, 0.9, 1)
        _SpecColor("Specular", Color) = (0.16, 0.18, 0.2, 1)
        _Opacity("Opacity", Range(0, 1)) = 0.42
        _Smoothness("Smoothness", Range(0, 1)) = 0.9
        _Fresnel("Edge", Range(0, 1)) = 0.7
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Float) = 2
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
            Cull [_Cull]

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
                half4 _BaseColor;
                half4 _ReflectColor;
                half4 _SpecColor;
                half _Opacity;
                half _Smoothness;
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
                float3 face = normalize(input.normalWS);
                if (dot(face, viewDir) < 0)
                    face = -face;

                half fresnel = pow(1.0h - saturate((half)dot(face, viewDir)), 3.5h);
                float3 reflection = reflect(-viewDir, face);
                half sky = saturate(reflection.y * 0.5h + 0.5h);
                half3 room = lerp(half3(0.45, 0.5, 0.54), _ReflectColor.rgb, sky);
                half3 albedo = lerp(_BaseColor.rgb, room, fresnel * _Fresnel);

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

                half alpha = lerp(_Opacity, saturate(_Opacity + 0.4h), fresnel);

                SurfaceData surface = (SurfaceData)0;
                surface.albedo = albedo;
                surface.specular = _SpecColor.rgb;
                surface.smoothness = lerp(_Smoothness, 1.0h, fresnel * _Fresnel);
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

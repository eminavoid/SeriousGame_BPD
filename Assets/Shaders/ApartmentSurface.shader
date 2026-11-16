Shader "Apartment/Surface"
{
    Properties
    {
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1, 1, 1, 1)
        [Normal] _BumpMap("Normal", 2D) = "bump" {}
        _BumpScale("Normal strength", Range(0, 2)) = 1
        _Tile("World tiles per meter", Float) = 0.8
        _Blend("Triplanar blend", Range(1, 12)) = 6
        _Mapping("Mapping (0 world, 1 object)", Float) = 0
        _PlanarScale("Object UV scale/offset", Vector) = (1, 1, 0.5, 0.5)
        _HardNormals("Flat faces", Range(0, 1)) = 1
        _Smoothness("Smoothness", Range(0, 1)) = 0.3
        _Metallic("Metallic", Range(0, 1)) = 0
        _Macro("Large breakup", Range(0, 1)) = 0.25
        _SpecColor("Specular", Color) = (0.04, 0.04, 0.04, 1)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "UniversalMaterialType" = "SimpleLit"
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

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half4 _SpecColor;
                half _BumpScale;
                half _Tile;
                half _Blend;
                half _Mapping;
                float4 _PlanarScale;
                half _HardNormals;
                half _Smoothness;
                half _Metallic;
                half _Macro;
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
                float3 positionOS : TEXCOORD1;
                half3 normalWS : TEXCOORD2;
                half fogFactor : TEXCOORD3;
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
                output.positionOS = input.positionOS.xyz;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
#if defined(_FOG_FRAGMENT)
                output.fogFactor = 0;
#else
                output.fogFactor = ComputeFogFactor(vertex.positionCS.z);
#endif
                return output;
            }

            // El derivativo implícito se rompe en el triplanar y, en diagonal, deja líneas.
            void StableGrad(float2 uv, out float2 dx, out float2 dy)
            {
                dx = ddx(uv);
                dy = ddy(uv);
                float spread = max(length(dx), length(dy));
                float soften = max(spread, 0.004) * 1.65;
                float scale = soften / max(spread, 1e-5);
                dx *= scale;
                dy *= scale;
            }

            half3 SampleAlbedoGrad(float2 uv, float2 dx, float2 dy)
            {
                return SAMPLE_TEXTURE2D_GRAD(_BaseMap, sampler_BaseMap, uv, dx, dy).rgb;
            }

            half3 SampleAlbedo(float2 uv)
            {
                float2 dx, dy;
                StableGrad(uv, dx, dy);
                return SampleAlbedoGrad(uv, dx, dy);
            }

            half3 SampleTangentGrad(float2 uv, float2 dx, float2 dy)
            {
                half4 packed = SAMPLE_TEXTURE2D_GRAD(_BumpMap, sampler_BumpMap, uv, dx, dy);
                return UnpackNormalScale(packed, _BumpScale);
            }

            // Un peso en cero no aporta. El derivativo se calcula siempre, afuera del if,
            // para no volver a dibujar la línea entre triángulos.
            float3 TriplanarNormal(float3 posWS, float3 n, float3 blend)
            {
                float2 uvX = posWS.zy * _Tile;
                float2 uvY = posWS.xz * _Tile;
                float2 uvZ = posWS.xy * _Tile;
                float2 dxX, dyX, dxY, dyY, dxZ, dyZ;
                StableGrad(uvX, dxX, dyX);
                StableGrad(uvY, dxY, dyY);
                StableGrad(uvZ, dxZ, dyZ);

                float3 tx = 0;
                float3 ty = 0;
                float3 tz = 0;
                [branch] if (blend.x > 0)
                {
                    float3 t = SampleTangentGrad(uvX, dxX, dyX);
                    tx = float3(t.xy + n.zy, abs(t.z) * n.x);
                }
                [branch] if (blend.y > 0)
                {
                    float3 t = SampleTangentGrad(uvY, dxY, dyY);
                    ty = float3(t.xy + n.xz, abs(t.z) * n.y);
                }
                [branch] if (blend.z > 0)
                {
                    float3 t = SampleTangentGrad(uvZ, dxZ, dyZ);
                    tz = float3(t.xy + n.xy, abs(t.z) * n.z);
                }
                return normalize(tx.zyx * blend.x + ty.xzy * blend.y + tz.xyz * blend.z);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                float3 viewDir = GetWorldSpaceNormalizeViewDir(input.positionWS);
                // La normal de la malla. Reconstruirla con ddx/ddy dibuja una línea en cada triángulo al cambiar el ángulo.
                float3 baseNormal = normalize(input.normalWS);

                half3 albedo;
                half luma;
                float3 normalWS;

                if (_Mapping > 0.5)
                {
                    float3 nOS = abs(normalize(mul((float3x3)unity_WorldToObject, baseNormal)));
                    float2 uv = nOS.y >= nOS.x && nOS.y >= nOS.z
                        ? input.positionOS.xz
                        : (nOS.x >= nOS.z ? input.positionOS.zy : input.positionOS.xy);
                    uv = uv * _PlanarScale.xy + _PlanarScale.zw;
                    half3 tex = SampleAlbedo(uv);
                    albedo = tex * _BaseColor.rgb;
                    luma = dot(tex, half3(0.3, 0.5, 0.2));
                    normalWS = baseNormal;
                }
                else
                {
                    float3 blend = pow(abs(baseNormal), _Blend);
                    blend /= max(dot(blend, 1.0), 1e-4);
                    float2 uvX = input.positionWS.zy * _Tile;
                    float2 uvY = input.positionWS.xz * _Tile;
                    float2 uvZ = input.positionWS.xy * _Tile;
                    float2 dxX, dyX, dxY, dyY, dxZ, dyZ;
                    StableGrad(uvX, dxX, dyX);
                    StableGrad(uvY, dxY, dyY);
                    StableGrad(uvZ, dxZ, dyZ);
                    half3 tex = 0;
                    [branch] if (blend.x > 0) tex += (half3)(SampleAlbedoGrad(uvX, dxX, dyX) * blend.x);
                    [branch] if (blend.y > 0) tex += (half3)(SampleAlbedoGrad(uvY, dxY, dyY) * blend.y);
                    [branch] if (blend.z > 0) tex += (half3)(SampleAlbedoGrad(uvZ, dxZ, dyZ) * blend.z);
                    half broad = dot(SampleAlbedo(uvY * 0.27), half3(0.3, 0.5, 0.2));
                    tex *= lerp(1.0, broad * 1.15, _Macro);
                    albedo = tex * _BaseColor.rgb;
                    luma = dot(tex, half3(0.3, 0.5, 0.2));
                    float3 detailN = TriplanarNormal(input.positionWS, baseNormal, blend);
                    half facing = saturate(abs(dot(baseNormal, viewDir)));
                    normalWS = normalize(lerp(baseNormal, detailN, facing));
                }

                half smooth = _Smoothness * lerp(0.7h, 1.0h, saturate(luma));
                half3 specular = lerp(_SpecColor.rgb, albedo, _Metallic);

                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;
                inputData.positionCS = input.positionCS;
                inputData.normalWS = normalWS;
                inputData.viewDirectionWS = viewDir;
#if defined(MAIN_LIGHT_CALCULATE_SHADOWS)
                inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
#else
                inputData.shadowCoord = float4(0, 0, 0, 0);
#endif
                inputData.fogCoord = InitializeInputDataFog(float4(input.positionWS, 1), input.fogFactor);
                inputData.vertexLighting = half3(0, 0, 0);
                inputData.bakedGI = SampleSH(normalWS);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
                inputData.shadowMask = half4(1, 1, 1, 1);
                inputData.tangentToWorld = half3x3(half3(1, 0, 0), half3(0, 1, 0), half3(0, 0, 1));

                SurfaceData surface = (SurfaceData)0;
                surface.albedo = albedo;
                surface.specular = specular;
                surface.metallic = _Metallic;
                surface.smoothness = smooth;
                surface.normalTS = half3(0, 0, 1);
                surface.emission = half3(0, 0, 0);
                surface.occlusion = 1;
                surface.alpha = 1;
                surface.clearCoatMask = 0;
                surface.clearCoatSmoothness = 1;

                half4 color = UniversalFragmentBlinnPhong(inputData, surface);
                color.rgb = MixFog(color.rgb, inputData.fogCoord);
                return half4(color.rgb, 1);
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite On
            ZTest LEqual
            ColorMask 0

            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex ShadowVert
            #pragma fragment ShadowFrag
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            float3 _LightDirection;
            float3 _LightPosition;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings ShadowVert(Attributes input)
            {
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
#if _CASTING_PUNCTUAL_LIGHT_SHADOW
                float3 lightDirectionWS = normalize(_LightPosition - positionWS);
#else
                float3 lightDirectionWS = _LightDirection;
#endif
                Varyings output;
                output.positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
                output.positionCS = ApplyShadowClamping(output.positionCS);
                return output;
            }

            half4 ShadowFrag(Varyings input) : SV_Target
            {
                return 0;
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

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings DepthVert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half DepthFrag(Varyings input) : SV_Target
            {
                return input.positionCS.z;
            }
            ENDHLSL
        }

        Pass
        {
            Name "DepthNormals"
            Tags { "LightMode" = "DepthNormals" }
            ZWrite On

            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex DepthNVert
            #pragma fragment DepthNFrag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
            };

            Varyings DepthNVert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half4 DepthNFrag(Varyings input) : SV_Target
            {
                return half4(normalize(input.normalWS), 0);
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Simple Lit"
}

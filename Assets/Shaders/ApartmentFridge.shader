Shader "Apartment/Fridge"
{
    Properties
    {
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1, 1, 1, 1)
        [Normal] _BumpMap("Normal", 2D) = "bump" {}
        _DoorMap("Door", 2D) = "white" {}
        [Normal] _DoorBump("Door normal", 2D) = "bump" {}
        _BumpScale("Normal strength", Range(0, 2)) = 0.4
        _Tile("World tiles per meter", Float) = 2
        _Blend("Triplanar blend", Range(1, 12)) = 8
        _Smoothness("Smoothness", Range(0, 1)) = 0.46
        _Metallic("Metallic", Range(0, 1)) = 0
        _Macro("Large breakup", Range(0, 1)) = 0.05
        _SpecColor("Specular", Color) = (0.055, 0.054, 0.05, 1)
        _GasketColor("Gasket", Color) = (0.16, 0.155, 0.15, 1)
        _UseDoor("Front doors", Float) = 1
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
            TEXTURE2D(_DoorMap);
            SAMPLER(sampler_DoorMap);
            TEXTURE2D(_DoorBump);
            SAMPLER(sampler_DoorBump);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _DoorMap_ST;
                half4 _BaseColor;
                half4 _SpecColor;
                half4 _GasketColor;
                half _BumpScale;
                half _Tile;
                half _Blend;
                half _Smoothness;
                half _Metallic;
                half _Macro;
                half _UseDoor;
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
                float3 normalOS : TEXCOORD4;
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
                output.normalOS = input.normalOS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
#if defined(_FOG_FRAGMENT)
                output.fogFactor = 0;
#else
                output.fogFactor = ComputeFogFactor(vertex.positionCS.z);
#endif
                return output;
            }

            // El piso del derivativo evita la línea diagonal del triplanar. Se llama siempre
            // afuera de los if, para que el salto entre triángulos no vuelva.
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

            half3 SampleDoorGrad(float2 uv, float2 dx, float2 dy)
            {
                return SAMPLE_TEXTURE2D_GRAD(_DoorMap, sampler_DoorMap, uv, dx, dy).rgb;
            }

            half3 SampleTangentGrad(float2 uv, float2 dx, float2 dy)
            {
                half4 packed = SAMPLE_TEXTURE2D_GRAD(_BumpMap, sampler_BumpMap, uv, dx, dy);
                return UnpackNormalScale(packed, _BumpScale);
            }

            half3 SampleDoorTangent(float2 uv, float2 dx, float2 dy)
            {
                half4 packed = SAMPLE_TEXTURE2D_GRAD(_DoorBump, sampler_DoorBump, uv, dx, dy);
                return UnpackNormalScale(packed, _BumpScale);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                float3 viewDir = GetWorldSpaceNormalizeViewDir(input.positionWS);
                float3 baseNormal = normalize(input.normalWS);
                float3 nOS = normalize(input.normalOS);
                float3 p = input.positionOS;

                float3 blend = pow(abs(baseNormal), _Blend);
                blend /= max(dot(blend, 1.0), 1e-4);
                float2 uvX = input.positionWS.zy * _Tile;
                float2 uvY = input.positionWS.xz * _Tile;
                float2 uvZ = input.positionWS.xy * _Tile;
                float2 dxX, dyX, dxY, dyY, dxZ, dyZ;
                StableGrad(uvX, dxX, dyX);
                StableGrad(uvY, dxY, dyY);
                StableGrad(uvZ, dxZ, dyZ);
                float2 uvBroad = uvY * 0.27;
                float2 dxB, dyB;
                StableGrad(uvBroad, dxB, dyB);

                // La puerta mira a -X. u crece con +Z, v con +Y. El burlete está en la textura.
                float2 doorUV = float2((p.z + 0.35) * (1.0 / 0.70), (p.y + 0.90) * (1.0 / 1.80));
                float2 doorDx = ddx(doorUV);
                float2 doorDy = ddy(doorUV);

                // Hoja plana en x = -0.35. El chaflán queda justo detrás, y el cuerpo en x = -0.32.
                bool cabinet = _UseDoor > 0.5;
                bool panel = cabinet && nOS.x < -0.92 && p.x < -0.343;
                bool lip = cabinet && !panel && p.x < -0.338 && nOS.x < -0.45;

                half3 albedo;
                half luma;
                float3 normalWS;
                half metal = _Metallic;

                if (panel)
                {
                    half3 tex = SampleDoorGrad(doorUV, doorDx, doorDy);
                    albedo = tex * _BaseColor.rgb;
                    luma = dot(tex, half3(0.3, 0.5, 0.2));
                    half3 ts = SampleDoorTangent(doorUV, doorDx, doorDy);
                    float3 nPertOS = normalize(float3(0, 0, 1) * ts.x + float3(0, 1, 0) * ts.y + float3(-1, 0, 0) * ts.z);
                    float3 detailN = TransformObjectToWorldNormal(nPertOS);
                    half facing = saturate(abs(dot(baseNormal, viewDir)));
                    normalWS = normalize(lerp(baseNormal, detailN, facing));
                    metal = 0;
                }
                else
                {
                    half3 tex = 0;
                    float3 tx = 0;
                    float3 ty = 0;
                    float3 tz = 0;
                    [branch] if (blend.x > 0)
                    {
                        tex += (half3)(SampleAlbedoGrad(uvX, dxX, dyX) * blend.x);
                        float3 t = SampleTangentGrad(uvX, dxX, dyX);
                        tx = float3(t.xy + baseNormal.zy, abs(t.z) * baseNormal.x);
                    }
                    [branch] if (blend.y > 0)
                    {
                        tex += (half3)(SampleAlbedoGrad(uvY, dxY, dyY) * blend.y);
                        float3 t = SampleTangentGrad(uvY, dxY, dyY);
                        ty = float3(t.xy + baseNormal.xz, abs(t.z) * baseNormal.y);
                    }
                    [branch] if (blend.z > 0)
                    {
                        tex += (half3)(SampleAlbedoGrad(uvZ, dxZ, dyZ) * blend.z);
                        float3 t = SampleTangentGrad(uvZ, dxZ, dyZ);
                        tz = float3(t.xy + baseNormal.xy, abs(t.z) * baseNormal.z);
                    }
                    half broad = dot(SampleAlbedoGrad(uvBroad, dxB, dyB), half3(0.3, 0.5, 0.2));
                    tex *= lerp(1.0, broad * 1.15, _Macro);
                    albedo = tex * _BaseColor.rgb;
                    luma = dot(tex, half3(0.3, 0.5, 0.2));
                    float3 detailN = normalize(tx.zyx * blend.x + ty.xzy * blend.y + tz.xyz * blend.z);
                    half facing = saturate(abs(dot(baseNormal, viewDir)));
                    normalWS = normalize(lerp(baseNormal, detailN, facing));
                }

                // Burlete del chaflán, y la junta entre el freezer y la heladera (y ≈ 0.27).
                half gasket = 0;
                if (lip)
                    gasket = 1;
                else if (cabinet && nOS.x < -0.92 && p.x > -0.330 && abs(p.z) < 0.336)
                    gasket = 1.0 - smoothstep(0.004, 0.012, abs(p.y - 0.270));

                albedo = lerp(albedo, _GasketColor.rgb, gasket);
                metal = lerp(metal, 0, gasket);

                half smooth = _Smoothness * lerp(0.7h, 1.0h, saturate(luma));
                smooth = lerp(smooth, min(smooth, 0.22h), gasket);
                half3 specular = lerp(_SpecColor.rgb, albedo, metal);

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
                surface.metallic = metal;
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

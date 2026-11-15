using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SeriousGameBPD.World
{
    /// <summary>
    /// Compila, al entrar a la casa, la variante que usa el pipeline de PC.
    /// Si no, el primer vistazo a la ventana, el espejo, la bacha o el celu
    /// compila el shader en el medio del frame.
    /// </summary>
    public static class ApartmentShaderWarmup
    {
        static readonly string[] LitShaders =
        {
            "Apartment/Surface",
            "Apartment/Glass",
            "Apartment/Clear",
            "Apartment/Mirror",
            "Apartment/Water",
        };

        static readonly string[] UnlitShaders =
        {
            "Apartment/Screen",
            "Apartment/Notification",
        };

        // Keywords que prende PC_RPAsset: Forward+, cascadas, sombras soft high, SSAO.
        static readonly string[] ForwardKeywords =
        {
            "_CLUSTER_LIGHT_LOOP",
            "_MAIN_LIGHT_SHADOWS_CASCADE",
            "_SCREEN_SPACE_OCCLUSION",
            "_SHADOWS_SOFT",
            "_SHADOWS_SOFT_HIGH",
        };

        static bool _done;

        public static void Warm()
        {
            if (_done) return;
            _done = true;

            for (int i = 0; i < LitShaders.Length; i++)
                WarmShader(LitShaders[i], true);
            for (int i = 0; i < UnlitShaders.Length; i++)
                WarmShader(UnlitShaders[i], false);
        }

        static void WarmShader(string shaderName, bool lighting)
        {
            var shader = Shader.Find(shaderName);
            if (shader == null)
                return;

            var collection = new ShaderVariantCollection();
            if (lighting)
                TryAdd(collection, shader, PassType.ScriptableRenderPipeline, ForwardKeywords);
            TryAdd(collection, shader, PassType.ScriptableRenderPipeline, null);
            TryAdd(collection, shader, PassType.ShadowCaster, null);
            if (collection.variantCount > 0)
                collection.WarmUp();
        }

        static void TryAdd(ShaderVariantCollection collection, Shader shader, PassType pass, string[] keywords)
        {
            try
            {
                var variant = keywords == null
                    ? new ShaderVariantCollection.ShaderVariant(shader, pass)
                    : new ShaderVariantCollection.ShaderVariant(shader, pass, keywords);
                collection.Add(variant);
            }
            catch (ArgumentException)
            {
            }
        }
    }
}

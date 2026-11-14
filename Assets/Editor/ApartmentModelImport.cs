#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SeriousGameBPD.EditorTools
{
    /// <summary>
    /// Los FBX salen de Blender con emisión en blanco. Acá se reemplazan por materiales URP
    /// con el color que trae cada uno, sin esa emisión.
    /// </summary>
    public class ApartmentModelImport : AssetPostprocessor
    {
        const string ModelRoot = "Assets/Models/Apartment/";
        const string MaterialRoot = "Assets/Materials/Apartment/Fbx";

        // r, g, b en lineal (como los exporta Blender), smoothness, metallic, emisión de pantalla.
        static readonly Dictionary<string, float[]> Surfaces = new Dictionary<string, float[]>
        {
            { "M_Alfombra", new[] { 0.3613f, 0.1590f, 0.0976f, 0.08f, 0f, 0f } },
            { "M_Ceramica_Blanca", new[] { 0.8963f, 0.8963f, 0.8796f, 0.55f, 0f, 0f } },
            { "M_Metal_Gastado", new[] { 0.2705f, 0.2874f, 0.3095f, 0.45f, 0.72f, 0f } },
            { "M_Plastico_Negro", new[] { 0.0137f, 0.0144f, 0.0168f, 0.40f, 0f, 0f } },
            { "M_Vidrio", new[] { 0.5776f, 0.7758f, 0.8714f, 0.88f, 0.04f, 0f } },
            { "M_Madera", new[] { 0.3712f, 0.1946f, 0.0844f, 0.22f, 0f, 0f } },
            { "M_Lino", new[] { 0.8388f, 0.7991f, 0.7231f, 0.08f, 0f, 0f } },
            { "M_Lino_Manta", new[] { 0.2918f, 0.3564f, 0.4287f, 0.08f, 0f, 0f } },
            { "M_Celular_Funda", new[] { 0.6584f, 0.1022f, 0.0452f, 0.28f, 0f, 0f } },
            { "M_Pantalla", new[] { 0.0060f, 0.0103f, 0.0176f, 0.60f, 0f, 1f } },
            { "M_Madera_Oscura", new[] { 0.1470f, 0.0723f, 0.0343f, 0.22f, 0f, 0f } },
            { "M_Cuaderno", new[] { 0.0497f, 0.1559f, 0.2623f, 0.12f, 0f, 0f } },
            { "M_Papel", new[] { 0.9047f, 0.8796f, 0.8070f, 0.08f, 0f, 0f } },
            { "M_Marco_Blanco", new[] { 0.7991f, 0.7913f, 0.7529f, 0.30f, 0f, 0f } },
            { "M_Espejo", new[] { 0.7231f, 0.7605f, 0.7913f, 0.86f, 0.85f, 0f } },
            { "M_Aluminio", new[] { 0.4851f, 0.5029f, 0.5333f, 0.50f, 0.75f, 0f } },
            { "M_Pared_Bano", new[] { 0.3663f, 0.4397f, 0.5029f, 0.12f, 0f, 0f } },
            { "M_Pared_Estar", new[] { 0.3813f, 0.4397f, 0.3467f, 0.08f, 0f, 0f } },
            { "M_Pared_Dormitorio", new[] { 0.7913f, 0.6514f, 0.5029f, 0.08f, 0f, 0f } },
            { "M_Piso_Ceramico", new[] { 0.5520f, 0.5841f, 0.6038f, 0.40f, 0.02f, 0f } },
            { "M_Piso_Porcelanato", new[] { 0.6724f, 0.6308f, 0.5647f, 0.42f, 0.02f, 0f } },
            { "M_Piso_Madera", new[] { 0.2542f, 0.1356f, 0.0648f, 0.22f, 0f, 0f } },
            { "M_Tela_Oscura", new[] { 0.0423f, 0.0452f, 0.0578f, 0.06f, 0f, 0f } },
            { "M_Taza", new[] { 0.6939f, 0.3712f, 0.0529f, 0.45f, 0f, 0f } },
            { "M_Mesada", new[] { 0.1559f, 0.1470f, 0.1384f, 0.40f, 0f, 0f } },
        };

        void OnPreprocessModel()
        {
            if (!assetPath.Replace('\\', '/').StartsWith(ModelRoot))
                return;

            var importer = (ModelImporter)assetImporter;
            importer.globalScale = 1f;
            importer.useFileScale = true;
            importer.bakeAxisConversion = true;
            importer.importAnimation = false;
            importer.animationType = ModelImporterAnimationType.None;
            importer.importCameras = false;
            importer.importLights = false;
            importer.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
            importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
            importer.materialSearch = ModelImporterMaterialSearch.Everywhere;
        }

        Material OnAssignMaterialModel(Material material, Renderer renderer)
        {
            if (!assetPath.Replace('\\', '/').StartsWith(ModelRoot) || material == null)
                return null;

            var key = Clean(material.name);
            if (!Surfaces.TryGetValue(key, out var spec))
            {
                Debug.LogWarning("Apartment FBX: material sin tabla " + material.name + " en " + assetPath);
                return null;
            }

            return GetOrCreate(key, spec);
        }

        static Material GetOrCreate(string key, float[] spec)
        {
            EnsureFolder();
            var path = MaterialRoot + "/" + key + ".mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit")
                             ?? Shader.Find("Universal Render Pipeline/Simple Lit");
                if (shader == null)
                    return null;

                mat = new Material(shader) { name = key };
                AssetDatabase.CreateAsset(mat, path);
            }

            var color = new Color(spec[0], spec[1], spec[2], 1f).gamma;
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", color);
            if (mat.HasProperty("_Smoothness"))
                mat.SetFloat("_Smoothness", spec[3]);
            if (mat.HasProperty("_Metallic"))
                mat.SetFloat("_Metallic", spec[4]);

            var screen = spec[5] > 0.5f;
            var emission = screen ? new Color(0.16f, 0.24f, 0.2f) : Color.black;
            if (mat.HasProperty("_EmissionColor"))
            {
                mat.SetColor("_EmissionColor", emission);
                if (screen)
                    mat.EnableKeyword("_EMISSION");
                else
                    mat.DisableKeyword("_EMISSION");
            }

            if (mat.HasProperty("_Cull"))
                mat.SetFloat("_Cull", 2f);

            EditorUtility.SetDirty(mat);
            return mat;
        }

        static void EnsureFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Materials"))
                AssetDatabase.CreateFolder("Assets", "Materials");
            if (!AssetDatabase.IsValidFolder("Assets/Materials/Apartment"))
                AssetDatabase.CreateFolder("Assets/Materials", "Apartment");
            if (!AssetDatabase.IsValidFolder(MaterialRoot))
                AssetDatabase.CreateFolder("Assets/Materials/Apartment", "Fbx");
        }

        static string Clean(string name)
        {
            var cut = name.IndexOf('\0');
            if (cut >= 0)
                name = name.Substring(0, cut);
            const string instance = " (Instance)";
            cut = name.IndexOf(instance, System.StringComparison.Ordinal);
            if (cut >= 0)
                name = name.Substring(0, cut);
            return name.Trim();
        }
    }
}
#endif

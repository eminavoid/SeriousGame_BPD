#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SeriousGameBPD.EditorTools
{
    /// <summary>
    /// Al importar un FBX del departamento, usa el material compartido con textura y shader.
    /// </summary>
    public class ApartmentModelImport : AssetPostprocessor
    {
        const string ModelRoot = "Assets/Models/Apartment/";

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
            var styled = ApartmentVisuals.GetSharedMaterial(key);
            if (styled == null)
                Debug.LogWarning("Apartment FBX: material sin tabla " + material.name + " en " + assetPath);
            return styled;
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

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
            var door = DoorMaterial(key);
            if (door != null)
                return door;

            var fridge = FridgeMaterial(key);
            if (fridge != null)
                return fridge;

            var styled = ApartmentVisuals.GetSharedMaterial(key);
            if (styled == null)
                Debug.LogWarning("Apartment FBX: material sin tabla " + material.name + " en " + assetPath);
            return styled;
        }

        Material DoorMaterial(string key)
        {
            var path = assetPath.Replace('\\', '/');
            if (!path.EndsWith("/Puerta.fbx"))
                return null;

            string asset = null;
            if (key == "M_Madera_Oscura")
                asset = "Assets/Materials/Apartment/Fbx/M_Puerta_Marco.mat";
            else if (key == "M_Madera")
                asset = "Assets/Materials/Apartment/Fbx/M_Puerta_Hoja.mat";
            if (asset == null)
                return null;

            var mat = AssetDatabase.LoadAssetAtPath<Material>(asset);
            if (mat == null)
                Debug.LogWarning("Apartment FBX: falta " + asset);
            return mat;
        }

        Material FridgeMaterial(string key)
        {
            var path = assetPath.Replace('\\', '/');
            if (!path.EndsWith("/Heladera.fbx"))
                return null;

            string dedicated = null;
            if (key == "M_Plastico_Negro")
                dedicated = "M_Heladera_Manija";
            else if (key == "M_Metal_Gastado")
                dedicated = "M_Heladera";
            if (dedicated == null)
                return null;

            return AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Apartment/Fbx/" + dedicated + ".mat");
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

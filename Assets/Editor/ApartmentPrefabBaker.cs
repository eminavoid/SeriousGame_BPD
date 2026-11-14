#if UNITY_EDITOR
using System.IO;
using SeriousGameBPD.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SeriousGameBPD.EditorTools
{
    public static class ApartmentPrefabBaker
    {
        const string PrefabPath = "Assets/Prefabs/Apartment.prefab";
        const string MaterialFolder = "Assets/Materials/Apartment";
        const string HouseScenePath = "Assets/Scenes/House.unity";

        static bool _started;

        [InitializeOnLoadMethod]
        static void Schedule()
        {
            EditorApplication.delayCall += Tick;
        }

        public static void BakeBatch()
        {
            Ensure(overwritePrefab: false);
        }

        static void Tick()
        {
            if (Application.isBatchMode)
                return;

            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += Tick;
                return;
            }

            if (_started || EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            _started = true;
            try
            {
                Ensure(overwritePrefab: false);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        [MenuItem("SeriousGame BPD/Bake Apartment Prefab")]
        public static void BakeFromMenu()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogWarning("Salí de Play antes de hornear el departamento.");
                return;
            }

            bool exists = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) != null;
            if (exists && !EditorUtility.DisplayDialog(
                    "Hornear departamento",
                    "Esto vuelve a crear Apartment.prefab con la geometría del código y reemplaza la instancia en la escena House. Los materiales que ya recoloriste se conservan. Los movimientos que hayas hecho en el prefab se pierden.",
                    "Hornear de nuevo",
                    "Cancelar"))
                return;

            Ensure(overwritePrefab: true);
        }

        public static void Ensure(bool overwritePrefab)
        {
            EnsureFolders();
            bool missing = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) == null;
            if (missing || overwritePrefab)
                CreatePrefab();

            if (overwritePrefab || !SceneFileReferencesPrefab())
                PlaceInHouseScene(overwritePrefab);

            AssetDatabase.SaveAssets();
            Debug.Log("SeriousGame BPD: Apartment.prefab está en la escena House. Mové los cubos desde la Hierarchy.");
        }

        static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Materials"))
                AssetDatabase.CreateFolder("Assets", "Materials");
            if (!AssetDatabase.IsValidFolder(MaterialFolder))
                AssetDatabase.CreateFolder("Assets/Materials", "Apartment");
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        static void CreatePrefab()
        {
            var palette = HouseBuilder.CreatePalette(CreateMaterialAsset);
            var preview = EditorSceneManager.NewPreviewScene();
            try
            {
                var temp = new GameObject("BakeRoot");
                EditorSceneManager.MoveGameObjectToScene(temp, preview);
                var built = HouseBuilder.Build(temp.transform, palette);

                var spawn = new GameObject("Spawn");
                spawn.transform.SetParent(built.Root, false);
                spawn.transform.localPosition = built.Spawn;
                spawn.transform.localRotation = Quaternion.Euler(0f, built.SpawnYaw, 0f);

                var layout = built.Root.gameObject.AddComponent<HouseLayout>();
                layout.SpawnPoint = spawn.transform;
                layout.Sun = built.Sun;
                layout.Phone = built.Phone;
                layout.Door = built.Door;
                layout.Mirror = built.Mirror;
                layout.Sink = built.Sink;
                layout.Bed = built.Bed;
                layout.Laptop = built.Laptop;
                layout.Window = built.Window;
                layout.Couch = built.Couch;
                layout.Fridge = built.Fridge;
                layout.Coffee = built.Coffee;

                var saved = PrefabUtility.SaveAsPrefabAsset(built.Root.gameObject, PrefabPath);
                if (saved == null)
                    throw new System.InvalidOperationException("No se pudo guardar " + PrefabPath);
            }
            finally
            {
                EditorSceneManager.ClosePreviewScene(preview);
            }

            AssetDatabase.ImportAsset(PrefabPath);
        }

        static Material CreateMaterialAsset(string name, Color color)
        {
            var path = $"{MaterialFolder}/{name}.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
                return existing;

            var shader = Shader.Find("Universal Render Pipeline/Lit")
                         ?? Shader.Find("Universal Render Pipeline/Simple Lit")
                         ?? Shader.Find("Standard");
            if (shader == null)
                throw new System.InvalidOperationException("No se encontró un shader Lit para los materiales del departamento.");

            var mat = new Material(shader) { name = name };
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", color);
            if (mat.HasProperty("_Smoothness"))
                mat.SetFloat("_Smoothness", 0.2f);
            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        static bool SceneFileReferencesPrefab()
        {
            var guid = AssetDatabase.AssetPathToGUID(PrefabPath);
            if (string.IsNullOrEmpty(guid))
                return false;

            var fullPath = Path.Combine(Application.dataPath, "Scenes", "House.unity");
            if (!File.Exists(fullPath))
                return false;

            return File.ReadAllText(fullPath).Contains(guid);
        }

        static void PlaceInHouseScene(bool replaceExisting)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (prefab == null)
                throw new System.InvalidOperationException("Falta el prefab " + PrefabPath);

            var scene = EditorSceneManager.GetSceneByPath(HouseScenePath);
            bool opened = false;
            if (!scene.IsValid() || !scene.isLoaded)
            {
                scene = EditorSceneManager.OpenScene(HouseScenePath, OpenSceneMode.Additive);
                opened = true;
            }

            try
            {
                if (replaceExisting)
                {
                    foreach (var root in scene.GetRootGameObjects())
                    {
                        if (root.GetComponentInChildren<HouseLayout>(true) != null)
                            Object.DestroyImmediate(root);
                    }
                }
                else
                {
                    foreach (var root in scene.GetRootGameObjects())
                    {
                        if (root.GetComponentInChildren<HouseLayout>(true) != null)
                            return;
                    }
                }

                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
                instance.name = "Apartment";
                instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

                foreach (var root in scene.GetRootGameObjects())
                {
                    if (root.name == "Directional Light")
                        root.SetActive(false);
                }

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
            finally
            {
                if (opened)
                    EditorSceneManager.CloseScene(scene, true);
            }
        }
    }
}
#endif

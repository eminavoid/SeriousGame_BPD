#if UNITY_EDITOR
using System.IO;
using System.Text.RegularExpressions;
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

        public static void BakeBatchOverwrite()
        {
            Ensure(overwritePrefab: true);
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

        static Texture2D _wood;
        static Texture2D _plaster;
        static Texture2D _fabric;
        static Texture2D _metal;
        static Texture2D _screen;
        static bool _texturesReady;

        static Material CreateMaterialAsset(string name, Color color)
        {
            EnsureTextures();
            var path = $"{MaterialFolder}/{name}.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            Material mat;
            if (existing != null)
            {
                mat = existing;
            }
            else
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit")
                             ?? Shader.Find("Universal Render Pipeline/Simple Lit")
                             ?? Shader.Find("Standard");
                if (shader == null)
                    throw new System.InvalidOperationException("No se encontró un shader Lit para los materiales del departamento.");

                mat = new Material(shader) { name = name };
                AssetDatabase.CreateAsset(mat, path);
            }

            StyleMaterial(mat, name, color);
            EditorUtility.SetDirty(mat);
            return mat;
        }

        static void StyleMaterial(Material mat, string name, Color color)
        {
            float smoothness = 0.16f;
            float metallic = 0f;
            Texture2D tex = null;
            var scale = Vector2.one;
            var emission = Color.black;

            switch (name)
            {
                case "Floor":
                    tex = _wood; smoothness = 0.28f; scale = new Vector2(4f, 5f); break;
                case "Wood":
                case "WoodDark":
                    tex = _wood; smoothness = 0.24f; scale = new Vector2(1.4f, 1.4f); break;
                case "Ceiling":
                case "BedWall":
                case "LiveWall":
                case "BathWall":
                    tex = _plaster; smoothness = 0.06f; scale = new Vector2(2.2f, 2.2f); break;
                case "Linen":
                case "Pillow":
                case "Blanket":
                case "Couch":
                case "Rug":
                    tex = _fabric; smoothness = 0.1f; scale = new Vector2(2f, 2f); break;
                case "Metal":
                    tex = _metal; smoothness = 0.58f; metallic = 0.72f; scale = new Vector2(2f, 1f); break;
                case "Glass":
                    smoothness = 0.84f; metallic = 0.04f; break;
                case "Phone":
                case "Dark":
                    smoothness = 0.42f; break;
                case "Screen":
                    tex = _screen; smoothness = 0.55f; color = Color.white;
                    emission = new Color(0.22f, 0.28f, 0.26f);
                    scale = Vector2.one;
                    break;
            }

            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", color);
            if (mat.HasProperty("_Smoothness"))
                mat.SetFloat("_Smoothness", smoothness);
            if (mat.HasProperty("_Metallic"))
                mat.SetFloat("_Metallic", metallic);
            if (mat.HasProperty("_BaseMap"))
            {
                mat.SetTexture("_BaseMap", tex);
                mat.SetTextureScale("_BaseMap", scale);
            }
            if (mat.HasProperty("_MainTex"))
            {
                mat.SetTexture("_MainTex", tex);
                mat.SetTextureScale("_MainTex", scale);
            }
            if (mat.HasProperty("_EmissionColor"))
            {
                mat.SetColor("_EmissionColor", emission);
                if (emission.maxColorComponent > 0.001f)
                    mat.EnableKeyword("_EMISSION");
                else
                    mat.DisableKeyword("_EMISSION");
            }
        }

        static void EnsureTextures()
        {
            if (_texturesReady)
                return;

            if (!AssetDatabase.IsValidFolder(MaterialFolder + "/Textures"))
                AssetDatabase.CreateFolder(MaterialFolder, "Textures");

            _wood = SaveTexture("Wood", MakeWood(), true);
            _plaster = SaveTexture("Plaster", MakePlaster(), true);
            _fabric = SaveTexture("Fabric", MakeFabric(), true);
            _metal = SaveTexture("Metal", MakeMetal(), true);
            _screen = SaveTexture("Screen", MakeScreen(), false);
            _texturesReady = true;
        }

        static Texture2D SaveTexture(string name, Texture2D tex, bool repeat)
        {
            var path = $"{MaterialFolder}/Textures/{name}.png";
            File.WriteAllBytes(path, tex.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Default;
            importer.sRGBTexture = true;
            importer.mipmapEnabled = true;
            importer.wrapMode = repeat ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        static Texture2D MakeWood()
        {
            const int w = 256;
            const int h = 256;
            var tex = new Texture2D(w, h, TextureFormat.RGB24, false);
            var pixels = new Color[w * h];
            for (int y = 0; y < h; y++)
            {
                int plank = y / 40;
                float seam = (y % 40) <= 1 ? 0.42f : 1f;
                float plankTone = 0.78f + (plank % 3) * 0.07f;
                for (int x = 0; x < w; x++)
                {
                    float grain = Mathf.PerlinNoise(x * 0.09f, plank * 1.7f + y * 0.01f);
                    float knot = Mathf.PerlinNoise(x * 0.03f + 4f, y * 0.03f);
                    float v = plankTone * seam * Mathf.Lerp(0.82f, 1f, grain);
                    if (knot > 0.82f)
                        v *= 0.75f;
                    pixels[y * w + x] = new Color(v, v, v, 1f);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        static Texture2D MakePlaster()
        {
            const int w = 128;
            const int h = 128;
            var tex = new Texture2D(w, h, TextureFormat.RGB24, false);
            var pixels = new Color[w * h];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float n = Mathf.PerlinNoise(x * 0.08f, y * 0.08f);
                    float speckle = Mathf.PerlinNoise(x * 0.45f + 8f, y * 0.45f);
                    float v = Mathf.Lerp(0.9f, 1f, n) * Mathf.Lerp(0.96f, 1f, speckle);
                    pixels[y * w + x] = new Color(v, v, v, 1f);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        static Texture2D MakeFabric()
        {
            const int w = 128;
            const int h = 128;
            var tex = new Texture2D(w, h, TextureFormat.RGB24, false);
            var pixels = new Color[w * h];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    bool weave = ((x / 2) + (y / 2)) % 2 == 0;
                    float n = Mathf.PerlinNoise(x * 0.12f, y * 0.12f);
                    float v = (weave ? 0.88f : 1f) * Mathf.Lerp(0.92f, 1f, n);
                    pixels[y * w + x] = new Color(v, v, v, 1f);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        static Texture2D MakeMetal()
        {
            const int w = 128;
            const int h = 128;
            var tex = new Texture2D(w, h, TextureFormat.RGB24, false);
            var pixels = new Color[w * h];
            for (int y = 0; y < h; y++)
            {
                float streak = Mathf.PerlinNoise(2.2f, y * 0.35f);
                for (int x = 0; x < w; x++)
                {
                    float fine = Mathf.PerlinNoise(x * 0.8f, y * 0.05f);
                    float v = Mathf.Lerp(0.78f, 1f, streak) * Mathf.Lerp(0.92f, 1f, fine);
                    pixels[y * w + x] = new Color(v, v, v, 1f);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        static Texture2D MakeScreen()
        {
            const int w = 64;
            const int h = 128;
            var tex = new Texture2D(w, h, TextureFormat.RGB24, false);
            var bg = new Color(0.07f, 0.1f, 0.13f);
            var pixels = new Color[w * h];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = bg;

            void Fill(int x0, int y0, int x1, int y1, Color c)
            {
                for (int y = y0; y < y1; y++)
                for (int x = x0; x < x1; x++)
                {
                    if (x < 0 || y < 0 || x >= w || y >= h) continue;
                    pixels[y * w + x] = c;
                }
            }

            Fill(0, h - 16, w, h, new Color(0.12f, 0.16f, 0.2f));
            Fill(6, h - 12, 22, h - 6, new Color(0.85f, 0.9f, 0.88f));
            Fill(8, 78, 40, 108, new Color(0.22f, 0.45f, 0.4f));
            Fill(24, 48, 56, 72, new Color(0.25f, 0.28f, 0.32f));
            Fill(8, 22, 36, 40, new Color(0.22f, 0.45f, 0.4f));
            Fill(10, 8, 54, 16, new Color(0.16f, 0.2f, 0.24f));

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
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
                RepairPrefabRootInScene();
            }
            finally
            {
                if (opened)
                    EditorSceneManager.CloseScene(scene, true);
            }
        }

        static void RepairPrefabRootInScene()
        {
            var scenePath = Path.Combine(Application.dataPath, "Scenes", "House.unity");
            var prefabPath = Path.Combine(Application.dataPath, "Prefabs", "Apartment.prefab");
            if (!File.Exists(scenePath) || !File.Exists(prefabPath))
                return;

            var scene = File.ReadAllText(scenePath);
            if (scene.Contains(" stripped"))
                return;

            var prefab = File.ReadAllText(prefabPath);
            var rootTransform = Regex.Match(
                prefab,
                @"- component: \{fileID: (\d+)\}\s+- component: \{fileID: \d+\}\s+m_Layer: 0\s+m_Name: Apartment");
            var instance = Regex.Match(scene, @"--- !u!1001 &(\d+)\s+PrefabInstance:");
            if (!rootTransform.Success || !instance.Success)
                return;

            var guid = AssetDatabase.AssetPathToGUID(PrefabPath);
            var strippedId = (long.Parse(instance.Groups[1].Value) + 1).ToString();
            var block =
                $"--- !u!4 &{strippedId} stripped\n" +
                "Transform:\n" +
                $"  m_CorrespondingSourceObject: {{fileID: {rootTransform.Groups[1].Value}, guid: {guid}, type: 3}}\n" +
                $"  m_PrefabInstance: {{fileID: {instance.Groups[1].Value}}}\n" +
                "  m_PrefabAsset: {fileID: 0}\n";

            scene = Regex.Replace(scene, @"--- !u!1660057539 &9223372036854775807\r?\n", block + "--- !u!1660057539 &9223372036854775807\n");
            scene = scene.Replace(
                "  - {fileID: " + instance.Groups[1].Value + "}\n",
                "  - {fileID: " + strippedId + "}\n");
            File.WriteAllText(scenePath, scene.Replace("\r\n", "\n"));
        }
    }
}
#endif

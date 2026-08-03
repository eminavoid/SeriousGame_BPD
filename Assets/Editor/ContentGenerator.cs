#if UNITY_EDITOR
using SeriousGameBPD.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SeriousGameBPD.EditorTools
{
    public static class ContentGenerator
    {
        const string ContentFolder = "Assets/Resources/Content";
        const string ScenesFolder = "Assets/Scenes";
        const string SessionKey = "SeriousGameBPD_AutoSetup_v2";

        [InitializeOnLoadMethod]
        static void AutoSetupOnce()
        {
            if (SessionState.GetBool(SessionKey, false))
                return;

            EditorApplication.delayCall += () =>
            {
                if (SessionState.GetBool(SessionKey, false))
                    return;

                try
                {
                    var plan = Resources.Load<DayPlan>("Content/DayPlan_MVP");
                    bool needsContent = plan == null || plan.events == null || plan.events.Count == 0;
                    bool needsBuildScenes = EditorBuildSettings.scenes == null ||
                                           EditorBuildSettings.scenes.Length < 3 ||
                                           EditorBuildSettings.scenes[0].path != $"{ScenesFolder}/Boot.unity";

                    if (needsContent || needsBuildScenes)
                    {
                        SetupAll();
                        Debug.Log("SeriousGame BPD: auto-setup ejecutado (contenido + escenas + build settings).");
                    }

                    SessionState.SetBool(SessionKey, true);
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning("SeriousGame BPD auto-setup deferred: " + ex.Message);
                }
            };
        }

        [MenuItem("SeriousGame BPD/Generate MVP Content Assets")]
        public static void GenerateContent()
        {
            EnsureFolders();
            var plan = ContentFactory.BuildEmbeddedDayPlan();

            for (int i = 0; i < plan.skills.Count; i++)
            {
                var skill = plan.skills[i];
                var path = $"{ContentFolder}/Skill_{skill.id}.asset";
                SaveAsset(skill, path);
                plan.skills[i] = AssetDatabase.LoadAssetAtPath<SkillDef>(path);
            }

            for (int i = 0; i < plan.glossary.Count; i++)
            {
                var g = plan.glossary[i];
                var path = $"{ContentFolder}/Glossary_{i}.asset";
                SaveAsset(g, path);
                plan.glossary[i] = AssetDatabase.LoadAssetAtPath<GlossaryEntry>(path);
            }

            for (int i = 0; i < plan.events.Count; i++)
            {
                var e = plan.events[i];
                var path = $"{ContentFolder}/Event_{e.eventId}.asset";
                SaveAsset(e, path);
                plan.events[i] = AssetDatabase.LoadAssetAtPath<DayEvent>(path);
            }

            var planPath = $"{ContentFolder}/DayPlan_MVP.asset";
            SaveAsset(plan, planPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("SeriousGame BPD: MVP content assets generated at " + ContentFolder);
        }

        [MenuItem("SeriousGame BPD/Create Boot Day Debrief Scenes")]
        public static void CreateScenes()
        {
            EnsureFolders();
            if (!System.IO.File.Exists($"{ScenesFolder}/Boot.unity"))
                CreateEmptyScene($"{ScenesFolder}/Boot.unity");
            if (!System.IO.File.Exists($"{ScenesFolder}/Day.unity"))
                CreateEmptyScene($"{ScenesFolder}/Day.unity");
            if (!System.IO.File.Exists($"{ScenesFolder}/Debrief.unity"))
                CreateEmptyScene($"{ScenesFolder}/Debrief.unity");

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene($"{ScenesFolder}/Boot.unity", true),
                new EditorBuildSettingsScene($"{ScenesFolder}/Day.unity", true),
                new EditorBuildSettingsScene($"{ScenesFolder}/Debrief.unity", true),
            };
            AssetDatabase.SaveAssets();
            Debug.Log("SeriousGame BPD: Boot/Day/Debrief scenes ready in Build Settings.");
        }

        [MenuItem("SeriousGame BPD/Setup MVP (Content + Scenes + Build)")]
        public static void SetupAll()
        {
            GenerateContent();
            CreateScenes();
            PlayerSettings.productName = "Un Dia Con Emociones - TLP";
            PlayerSettings.companyName = "SeriousGame BPD";
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
            SessionState.SetBool(SessionKey, true);
            Debug.Log("SeriousGame BPD: MVP setup complete. Open Boot scene and Press Play.");
        }

        static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder(ContentFolder))
                AssetDatabase.CreateFolder("Assets/Resources", "Content");
            if (!AssetDatabase.IsValidFolder(ScenesFolder))
                AssetDatabase.CreateFolder("Assets", "Scenes");
            if (!AssetDatabase.IsValidFolder("Assets/Art"))
                AssetDatabase.CreateFolder("Assets", "Art");
            if (!AssetDatabase.IsValidFolder("Assets/Docs"))
                AssetDatabase.CreateFolder("Assets", "Docs");
        }

        static void SaveAsset(Object asset, string path)
        {
            var existing = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (existing != null)
                AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(asset, path);
        }

        static void CreateEmptyScene(string path)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, path);
        }
    }
}
#endif

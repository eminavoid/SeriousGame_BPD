#if UNITY_EDITOR
using SeriousGameBPD.Data;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace SeriousGameBPD.EditorTools
{
    public static class BuildWindows
    {
        const string OutputPath = "Builds/Windows/UnDiaConEmocionesTLP.exe";

        [MenuItem("SeriousGame BPD/Build Standalone Windows 64")]
        public static void Build()
        {
            // Avoid SwitchActiveBuildTarget in batchmode; generate content + scenes only.
            ContentGenerator.GenerateContent();
            ContentGenerator.CreateScenes();
            PlayerSettings.productName = "Un Dia Con Emociones - TLP";
            PlayerSettings.companyName = "SeriousGame BPD";

            var options = new BuildPlayerOptions
            {
                scenes = new[]
                {
                    "Assets/Scenes/Boot.unity",
                    "Assets/Scenes/House.unity",
                    "Assets/Scenes/Day.unity",
                    "Assets/Scenes/Debrief.unity"
                },
                locationPathName = OutputPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result == BuildResult.Succeeded)
                Debug.Log($"SeriousGame BPD: Windows build OK -> {OutputPath}");
            else
                Debug.LogError($"SeriousGame BPD: Windows build failed ({report.summary.result})");
        }

        [MenuItem("SeriousGame BPD/Validate MVP Content")]
        public static void Validate()
        {
            var plan = ContentFactory.BuildEmbeddedDayPlan();
            Debug.Assert(plan.events != null && plan.events.Count >= 4, "Need 4+ events");
            Debug.Assert(plan.skills != null && plan.skills.Count == 3, "Need 3 skills");
            Debug.Assert(plan.glossary != null && plan.glossary.Count >= 4, "Need 4 glossary entries");
            foreach (var e in plan.events)
            {
                Debug.Assert(e != null && e.options != null && e.options.Count > 0, "Event missing options: " + e);
            }
            Debug.Log($"SeriousGame BPD: validation OK — {plan.events.Count} events, {plan.skills.Count} skills, {plan.glossary.Count} glossary.");
        }
    }
}
#endif

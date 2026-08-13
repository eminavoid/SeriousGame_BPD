using UnityEngine;
using UnityEngine.SceneManagement;

namespace SeriousGameBPD.Core
{
    public static class SceneFlow
    {
        public const string BootScene = "Boot";
        public const string HouseScene = "House";
        public const string DayScene = "Day";
        public const string DebriefScene = "Debrief";

        public static void LoadBoot() => SceneManager.LoadScene(BootScene);
        public static void LoadHouse() => SceneManager.LoadScene(HouseScene);
        public static void LoadDay() => SceneManager.LoadScene(DayScene);
        public static void LoadDebrief() => SceneManager.LoadScene(DebriefScene);
    }
}

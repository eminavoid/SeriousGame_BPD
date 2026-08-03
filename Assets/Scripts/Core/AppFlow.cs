using SeriousGameBPD.Data;
using SeriousGameBPD.Narrative;
using SeriousGameBPD.Presentation;
using SeriousGameBPD.Safety;
using UnityEngine;

namespace SeriousGameBPD.Core
{
    /// <summary>
    /// Single persistent flow: Boot warning -> Day -> Debrief (no scene loads required).
    /// </summary>
    public class AppFlow : MonoBehaviour
    {
        public static AppFlow Instance { get; private set; }

        Transform _root;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoStart()
        {
            if (Instance != null) return;

            // Start from any entry scene (Boot / SampleScene / first loaded).
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            bool isEntry = scene.name == SceneFlow.BootScene
                           || scene.name == "SampleScene"
                           || scene.buildIndex == 0;
            if (!isEntry) return;

            var go = new GameObject("AppFlow");
            DontDestroyOnLoad(go);
            go.AddComponent<AppFlow>();
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            EnsureSession();
            UITheme.EnsureEventSystem();
            var canvas = UITheme.CreateCanvas("AppCanvas");
            DontDestroyOnLoad(canvas.gameObject);
            _root = canvas.transform;
            ShowBoot();
        }

        static void EnsureSession()
        {
            if (GameSession.Instance != null) return;
            var go = new GameObject("GameSession");
            DontDestroyOnLoad(go);
            go.AddComponent<GameSession>();
        }

        public void ShowBoot()
        {
            ClearRoot();
            var warning = _root.gameObject.AddComponent<ContentWarningUI>();
            warning.Build(_root, OnAccepted);
        }

        void OnAccepted()
        {
            GameSession.Instance.AcceptedWarning = true;
            GameSession.Instance.BeginNewDay(ContentFactory.LoadOrCreateDayPlan());
            ShowDay();
        }

        public void ShowDay()
        {
            ClearRoot();
            if (GameSession.Instance.DayPlan == null)
                GameSession.Instance.BeginNewDay(ContentFactory.LoadOrCreateDayPlan());

            var day = _root.gameObject.AddComponent<DayController>();
            day.BuildInto(_root, ShowDebrief);
        }

        public void ShowDebrief()
        {
            ClearRoot();
            var debrief = _root.gameObject.AddComponent<DebriefController>();
            debrief.BuildInto(_root, ShowDay, ShowBoot);
        }

        void ClearRoot()
        {
            if (_root == null) return;

            for (int i = _root.childCount - 1; i >= 0; i--)
                Destroy(_root.GetChild(i).gameObject);

            var toRemove = _root.GetComponents<MonoBehaviour>();
            foreach (var behaviour in toRemove)
            {
                if (behaviour == null) continue;
                if (behaviour is ContentWarningUI || behaviour is DayController || behaviour is DebriefController)
                    Destroy(behaviour);
            }
        }
    }
}

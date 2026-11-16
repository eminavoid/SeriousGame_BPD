using System.Collections;
using SeriousGameBPD.Data;
using SeriousGameBPD.Narrative;
using SeriousGameBPD.Presentation;
using SeriousGameBPD.Safety;
using SeriousGameBPD.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SeriousGameBPD.Core
{
    /// <summary>
    /// Persistent flow: start menu -> content warning -> House (3D day) -> Debrief.
    /// </summary>
    public class AppFlow : MonoBehaviour
    {
        public static AppFlow Instance { get; private set; }

        Transform _root;
        bool _pendingBootUi;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoStart()
        {
            if (Instance != null) return;

            var scene = SceneManager.GetActiveScene();
            bool isEntry = scene.name == SceneFlow.BootScene
                           || scene.name == "SampleScene"
                           || scene.buildIndex == 0;
            if (!isEntry) return;
            if (scene.name == SceneFlow.HouseScene) return;

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
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
            SceneManager.sceneLoaded -= OnSceneLoaded;
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

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_pendingBootUi && scene.name == SceneFlow.BootScene)
            {
                _pendingBootUi = false;
                ShowBoot();
                return;
            }

            if (scene.name == SceneFlow.HouseScene)
                HouseBootstrap.Ensure();
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
            PlayerLock.Reset();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (SceneManager.GetActiveScene().name != SceneFlow.BootScene
                && SceneManager.GetActiveScene().name != "SampleScene")
            {
                _pendingBootUi = true;
                ClearRoot();
                SceneFlow.LoadBoot();
                return;
            }

            ClearRoot();
            var menu = _root.gameObject.AddComponent<StartMenu>();
            menu.Build(_root, ShowWarning);
        }

        void ShowWarning()
        {
            ClearRoot();
            var warning = _root.gameObject.AddComponent<ContentWarningUI>();
            warning.Build(_root, OnAccepted);
        }

        void OnAccepted()
        {
            GameSession.Instance.AcceptedWarning = true;
            GameSession.Instance.BeginNewDay(ContentFactory.LoadOrCreateDayPlan());
            ShowHouse();
        }

        public void ShowHouse()
        {
            ClearRoot();
            if (GameSession.Instance.DayPlan == null)
                GameSession.Instance.BeginNewDay(ContentFactory.LoadOrCreateDayPlan());
            StopAllCoroutines();
            StartCoroutine(EnterHouse());
        }

        IEnumerator EnterHouse()
        {
            if (HouseSceneIsInBuild())
            {
                var load = SceneManager.LoadSceneAsync(SceneFlow.HouseScene);
                if (load != null)
                    yield return load;
            }

            if (SceneManager.GetActiveScene().name != SceneFlow.HouseScene)
            {
                var existing = FindFirstObjectByType<HouseBootstrap>();
                if (existing != null)
                    Destroy(existing.gameObject);
                yield return null;
            }

            HouseBootstrap.Ensure();
        }

        static bool HouseSceneIsInBuild()
        {
            int count = SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < count; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                if (string.IsNullOrEmpty(path)) continue;
                string name = System.IO.Path.GetFileNameWithoutExtension(path);
                if (name == SceneFlow.HouseScene)
                    return true;
            }

            return false;
        }

        public void OnHouseReady(HouseContext ctx)
        {
            if (ctx == null) return;
            ClearRoot();
            var bootstrap = FindFirstObjectByType<HouseBootstrap>();
            if (bootstrap == null) return;
            bootstrap.AttachHud(_root, ShowDebrief);
        }

        public void ShowDay()
        {
            ShowHouse();
        }

        public void ShowDebrief()
        {
            ClearRoot();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            var debrief = _root.gameObject.AddComponent<DebriefController>();
            debrief.BuildInto(_root, PlayAgain, ShowBoot);
        }

        void PlayAgain()
        {
            PlayerLock.Reset();
            GameSession.Instance.BeginNewDay(ContentFactory.LoadOrCreateDayPlan());
            ShowHouse();
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
                if (behaviour is StartMenu || behaviour is ContentWarningUI || behaviour is DayController || behaviour is DebriefController || behaviour is HouseHud
                    || behaviour is EventRunner || behaviour is PhoneChatUI || behaviour is ThoughtOverlay || behaviour is Affect.AffectMeterUI)
                    Destroy(behaviour);
            }
        }
    }
}

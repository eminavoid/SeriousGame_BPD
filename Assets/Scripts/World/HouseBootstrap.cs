using SeriousGameBPD.Core;
using SeriousGameBPD.Data;
using SeriousGameBPD.Presentation;
using SeriousGameBPD.Skills;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SeriousGameBPD.World
{
    public class HouseBootstrap : MonoBehaviour
    {
        HouseContext _ctx;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoStart()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene.name != SceneFlow.HouseScene) return;
            if (FindFirstObjectByType<HouseBootstrap>() != null) return;
            new GameObject("HouseBootstrap").AddComponent<HouseBootstrap>();
        }

        public static HouseBootstrap Ensure()
        {
            var existing = FindFirstObjectByType<HouseBootstrap>();
            if (existing != null)
            {
                if (existing._ctx == null)
                    existing.Boot();
                else if (AppFlow.Instance != null && existing._ctx.Hud == null)
                    AppFlow.Instance.OnHouseReady(existing._ctx);
                return existing;
            }

            var go = new GameObject("HouseBootstrap");
            var created = go.AddComponent<HouseBootstrap>();
            created.Boot();
            return created;
        }

        void Start()
        {
            Boot();
        }

        void Boot()
        {
            if (_ctx != null) return;

            PlayerLock.Reset();
            EnsureSession();
            BuildWorld();

            if (AppFlow.Instance != null)
                AppFlow.Instance.OnHouseReady(_ctx);
            else
                BuildStandaloneHud();
        }

        static void EnsureSession()
        {
            if (GameSession.Instance == null)
            {
                var go = new GameObject("GameSession");
                go.AddComponent<GameSession>();
            }

            if (GameSession.Instance.DayPlan == null)
                GameSession.Instance.BeginNewDay(ContentFactory.LoadOrCreateDayPlan());
        }

        void BuildWorld()
        {
            var existingCam = Camera.main;
            foreach (var cam in FindObjectsByType<Camera>(FindObjectsSortMode.None))
            {
                if (cam != existingCam)
                    cam.enabled = false;
            }

            var sceneLight = FindFirstObjectByType<Light>();
            if (sceneLight != null)
                sceneLight.enabled = false;

            var built = HouseBuilder.Build(transform);

            var playerGo = new GameObject("Player");
            var fps = playerGo.AddComponent<FPSController>();
            Camera eye = existingCam;
            if (eye == null)
            {
                var camGo = new GameObject("Eye", typeof(Camera), typeof(AudioListener));
                eye = camGo.GetComponent<Camera>();
                eye.tag = "MainCamera";
                if (camGo.GetComponent<UniversalAdditionalCameraData>() == null)
                    camGo.AddComponent<UniversalAdditionalCameraData>();
            }

            fps.Possess(eye, built.Spawn, built.SpawnYaw);

            var phone = built.PhoneVisual.gameObject.AddComponent<PhoneController>();
            phone.Setup(built.Phone);

            var door = built.DoorVisual.gameObject.AddComponent<DoorController>();
            door.Setup(built.Door);

            _ctx = gameObject.AddComponent<HouseContext>();
            _ctx.Clock = gameObject.AddComponent<WorldClock>();
            _ctx.Player = fps;
            _ctx.Phone = phone;
            _ctx.Door = door;
            _ctx.Sun = built.Sun;
            _ctx.Laptop = built.Laptop;
            _ctx.Skills = new SkillController(GameSession.Instance.DayPlan.skills);
        }

        void BuildStandaloneHud()
        {
            UITheme.EnsureEventSystem();
            var canvas = UITheme.CreateCanvas("HouseCanvas");
            AttachHud(canvas.transform, () =>
            {
                GameSession.Instance.BeginNewDay(ContentFactory.LoadOrCreateDayPlan());
                SceneFlow.LoadDebrief();
            });
        }

        public void AttachHud(Transform canvasRoot, System.Action onDayComplete)
        {
            var hud = canvasRoot.gameObject.AddComponent<HouseHud>();
            hud.Build(canvasRoot, _ctx.Skills, onDayComplete);
            _ctx.Hud = hud;
            _ctx.Thoughts = hud.Thoughts;
            _ctx.Events = hud.Events;
            _ctx.PhoneChat = hud.PhoneChat;

            var fx = _ctx.Player.gameObject.AddComponent<AffectPresentation>();
            fx.Setup(_ctx.Player.Eye, GameSession.Instance.Affect, canvasRoot, _ctx.Sun);
            _ctx.AffectFx = fx;

            var interactor = _ctx.Player.gameObject.AddComponent<PlayerInteractor>();
            interactor.Setup(_ctx.Player.Eye, hud);

            _ctx.Director = gameObject.AddComponent<DayDirector>();
            _ctx.Director.Setup(_ctx);
        }
    }
}

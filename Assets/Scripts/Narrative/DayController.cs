using SeriousGameBPD.Affect;
using SeriousGameBPD.Core;
using SeriousGameBPD.Data;
using SeriousGameBPD.Skills;
using SeriousGameBPD.Presentation;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace SeriousGameBPD.Narrative
{
    public class DayController : MonoBehaviour
    {
        int _index;
        EventRunner _runner;
        SkillController _skills;
        Text _progress;
        Action _onDayComplete;
        Transform _host;

        /// <summary>Used by AppFlow (same-scene). Scene-based AutoStart kept as fallback.</summary>
        public void BuildInto(Transform parent, Action onDayComplete)
        {
            _host = parent;
            _onDayComplete = onDayComplete;
            BuildUiAndStart();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoStart()
        {
            if (AppFlow.Instance != null) return;
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene.name != SceneFlow.DayScene) return;
            if (FindFirstObjectByType<DayController>() != null) return;
            new GameObject("DayController").AddComponent<DayController>();
        }

        void Start()
        {
            // Scene-only fallback path (no AppFlow BuildInto call).
            if (_host != null) return;
            if (AppFlow.Instance != null) return;

            if (GameSession.Instance == null)
            {
                var sessionGo = new GameObject("GameSession");
                sessionGo.AddComponent<GameSession>();
            }

            if (GameSession.Instance.DayPlan == null)
                GameSession.Instance.BeginNewDay(ContentFactory.LoadOrCreateDayPlan());

            UITheme.EnsureEventSystem();
            var canvas = UITheme.CreateCanvas("DayCanvas");
            _host = canvas.transform;
            _onDayComplete ??= SceneFlow.LoadDebrief;
            BuildUiAndStart();
        }

        void BuildUiAndStart()
        {
            var bg = UITheme.Panel(_host, "BG",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, UITheme.BgDeep);

            var plan = GameSession.Instance.DayPlan;
            var intro = UITheme.Text(bg, "DayTitle", plan != null ? plan.dayTitle : "Un día", 42, UITheme.TextPrimary, TextAnchor.MiddleLeft, FontStyle.Bold);
            UITheme.Stretch(intro.rectTransform, new Vector2(0.08f, 0.88f), new Vector2(0.70f, 0.98f), Vector2.zero, Vector2.zero);

            _progress = UITheme.Text(bg, "Progress", "", 22, UITheme.TextMuted, TextAnchor.MiddleRight);
            UITheme.Stretch(_progress.rectTransform, new Vector2(0.70f, 0.88f), new Vector2(0.92f, 0.98f), Vector2.zero, Vector2.zero);

            var meterHost = bg.gameObject.AddComponent<AffectMeterUI>();
            meterHost.Build(bg, GameSession.Instance.Affect);

            _skills = new SkillController(plan != null ? plan.skills : null);
            _runner = bg.gameObject.AddComponent<EventRunner>();
            _runner.Build(bg, _skills);

            _index = 0;
            PlayCurrent();
        }

        void PlayCurrent()
        {
            var plan = GameSession.Instance.DayPlan;
            if (plan == null || plan.events == null || _index >= plan.events.Count)
            {
                _onDayComplete?.Invoke();
                return;
            }

            // Skip null event assets from broken Resources references.
            while (_index < plan.events.Count && plan.events[_index] == null)
                _index++;

            if (_index >= plan.events.Count)
            {
                _onDayComplete?.Invoke();
                return;
            }

            _progress.text = $"Momento {_index + 1} / {plan.events.Count}";
            _runner.Play(plan.events[_index], OnEventFinished);
        }

        void OnEventFinished()
        {
            _index++;
            PlayCurrent();
        }
    }
}

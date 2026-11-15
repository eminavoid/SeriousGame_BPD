using System;
using SeriousGameBPD.Affect;
using SeriousGameBPD.Core;
using SeriousGameBPD.Data;
using SeriousGameBPD.Narrative;
using SeriousGameBPD.Presentation;
using SeriousGameBPD.Skills;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SeriousGameBPD.World
{
    public class HouseHud : MonoBehaviour
    {
        Transform _root;
        Text _clock;
        Text _prompt;
        Text _skillsHint;
        EventRunner _events;
        PhoneChatUI _phoneChat;
        ThoughtOverlay _thoughts;
        SkillController _skills;
        Action _onDayComplete;
        float _skillCooldown;
        const float SkillCooldownSeconds = 50f;
        bool _hintCooling;
        int _hintSecond = int.MinValue;
        string _clockBlock;
        string _clockTime;

        public EventRunner Events => _events;
        public PhoneChatUI PhoneChat => _phoneChat;
        public ThoughtOverlay Thoughts => _thoughts;
        public bool OverlayOpen =>
            (_events != null && _events.IsPlaying) ||
            (_phoneChat != null && _phoneChat.IsPlaying);

        public void Build(Transform parent, SkillController skills, Action onDayComplete)
        {
            _root = parent;
            _skills = skills;
            _onDayComplete = onDayComplete;

            var meters = parent.gameObject.AddComponent<AffectMeterUI>();
            meters.Build(parent, GameSession.Instance.Affect);

            _clock = UITheme.Text(parent, "Clock", "Mañana 07:30", 22, UITheme.TextMuted, TextAnchor.MiddleLeft);
            UITheme.Stretch(_clock.rectTransform, new Vector2(0.02f, 0.78f), new Vector2(0.28f, 0.86f), Vector2.zero, Vector2.zero);

            _prompt = UITheme.Text(parent, "Prompt", "", 28, UITheme.Accent, TextAnchor.MiddleCenter, FontStyle.Bold);
            UITheme.Stretch(_prompt.rectTransform, new Vector2(0.2f, 0.10f), new Vector2(0.8f, 0.18f), Vector2.zero, Vector2.zero);

            _skillsHint = UITheme.Text(parent, "SkillsHint",
                "Herramientas: 1 STOP · 2 TIPP · 3 Acción opuesta",
                18, UITheme.TextMuted, TextAnchor.MiddleLeft);
            UITheme.Stretch(_skillsHint.rectTransform, new Vector2(0.02f, 0.02f), new Vector2(0.55f, 0.08f), Vector2.zero, Vector2.zero);

            var cross = UITheme.Text(parent, "Crosshair", "+", 28, new Color(1f, 1f, 1f, 0.45f), TextAnchor.MiddleCenter);
            UITheme.Stretch(cross.rectTransform, new Vector2(0.48f, 0.48f), new Vector2(0.52f, 0.52f), Vector2.zero, Vector2.zero);

            _thoughts = parent.gameObject.AddComponent<ThoughtOverlay>();
            _thoughts.Build(parent);

            _events = parent.gameObject.AddComponent<EventRunner>();
            _events.Build(parent, skills);

            _phoneChat = parent.gameObject.AddComponent<PhoneChatUI>();
            _phoneChat.Build(parent, skills);
        }

        public void SetPrompt(string text)
        {
            if (_prompt != null)
                _prompt.text = text ?? "";
        }

        public void SetClock(string block, string time)
        {
            if (_clock == null) return;
            if (block == _clockBlock && time == _clockTime) return;
            _clockBlock = block;
            _clockTime = time;
            _clock.text = block + "  " + time;
        }

        public void CompleteDay()
        {
            PlayerLock.Push();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _onDayComplete?.Invoke();
        }

        void Update()
        {
            if (_skillsHint != null)
            {
                bool cooling = _skillCooldown > 0f;
                int second = cooling ? Mathf.RoundToInt(_skillCooldown) : 0;
                if (cooling != _hintCooling || second != _hintSecond)
                {
                    _hintCooling = cooling;
                    _hintSecond = second;
                    _skillsHint.text = cooling
                        ? "Herramientas en cooldown (" + second + "s) — 1 STOP · 2 TIPP · 3 Acción opuesta"
                        : "Herramientas: 1 STOP · 2 TIPP · 3 Acción opuesta  (libres, con cooldown)";
                }
            }

            if (_skillCooldown > 0f)
                _skillCooldown -= Time.unscaledDeltaTime;

            if (PlayerLock.IsLocked || OverlayOpen || _skillCooldown > 0f)
                return;

            SkillId id = SkillId.None;
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            if (kb == null) return;
            if (kb.digit1Key.wasPressedThisFrame || kb.numpad1Key.wasPressedThisFrame) id = SkillId.Stop;
            else if (kb.digit2Key.wasPressedThisFrame || kb.numpad2Key.wasPressedThisFrame) id = SkillId.Tipp;
            else if (kb.digit3Key.wasPressedThisFrame || kb.numpad3Key.wasPressedThisFrame) id = SkillId.OppositeAction;
#else
            if (Input.GetKeyDown(KeyCode.Alpha1)) id = SkillId.Stop;
            else if (Input.GetKeyDown(KeyCode.Alpha2)) id = SkillId.Tipp;
            else if (Input.GetKeyDown(KeyCode.Alpha3)) id = SkillId.OppositeAction;
#endif
            if (id != SkillId.None)
                UseFreeSkill(id);
        }

        public void UseFreeSkill(SkillId id)
        {
            if (_skills == null || !_skills.TryGet(id, out var skill)) return;
            if (PlayerLock.IsLocked || OverlayOpen || _skillCooldown > 0f) return;

            PlayerLock.Push();
            skill.Present(_root, _ =>
            {
                _skillCooldown = SkillCooldownSeconds;
                PlayerLock.Pop();
            });
        }
    }
}

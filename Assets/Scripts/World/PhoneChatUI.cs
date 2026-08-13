using System;
using System.Collections;
using SeriousGameBPD.Core;
using SeriousGameBPD.Data;
using SeriousGameBPD.Presentation;
using SeriousGameBPD.Skills;
using UnityEngine;
using UnityEngine.UI;

namespace SeriousGameBPD.World
{
    public class PhoneChatUI : MonoBehaviour
    {
        Transform _hudRoot;
        SkillController _skills;
        GameObject _blocker;
        RectTransform _bubbles;
        RectTransform _optionsRoot;
        Text _status;
        Action _onFinished;
        bool _playing;

        public bool IsPlaying => _playing;

        public void Build(Transform parent, SkillController skills)
        {
            _hudRoot = parent;
            _skills = skills;
            _blocker = UITheme.Panel(parent, "PhoneBlocker",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
                new Color(0f, 0f, 0f, 0.55f)).gameObject;

            var phone = UITheme.Panel(_blocker.transform, "PhoneFrame",
                new Vector2(0.32f, 0.06f), new Vector2(0.68f, 0.92f),
                Vector2.zero, Vector2.zero, new Color(0.08f, 0.09f, 0.12f, 0.98f));

            var header = UITheme.Text(phone, "Contact", "Alguien importante", 28, UITheme.TextPrimary, TextAnchor.MiddleCenter, FontStyle.Bold);
            UITheme.Stretch(header.rectTransform, new Vector2(0.06f, 0.90f), new Vector2(0.94f, 0.98f), Vector2.zero, Vector2.zero);

            _status = UITheme.Text(phone, "Status", "", 18, UITheme.TextMuted, TextAnchor.MiddleCenter);
            UITheme.Stretch(_status.rectTransform, new Vector2(0.06f, 0.86f), new Vector2(0.94f, 0.90f), Vector2.zero, Vector2.zero);

            var scroll = UITheme.ScrollArea(phone, "Thread", out _bubbles);
            UITheme.Stretch(scroll.GetComponent<RectTransform>(), new Vector2(0.06f, 0.34f), new Vector2(0.94f, 0.85f), Vector2.zero, Vector2.zero);

            var optionsGo = new GameObject("ChatOptions", typeof(RectTransform), typeof(VerticalLayoutGroup));
            _optionsRoot = optionsGo.GetComponent<RectTransform>();
            _optionsRoot.SetParent(phone, false);
            UITheme.Stretch(_optionsRoot, new Vector2(0.06f, 0.04f), new Vector2(0.94f, 0.32f), Vector2.zero, Vector2.zero);
            var layout = optionsGo.GetComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            Hide();
        }

        public void Hide()
        {
            _playing = false;
            if (_blocker != null)
                _blocker.SetActive(false);
        }

        public void Play(DayEvent evt, Action onFinished)
        {
            _onFinished = onFinished;
            _playing = true;
            _blocker.SetActive(true);
            Clear(_bubbles);
            Clear(_optionsRoot);
            StartCoroutine(Thread(evt));
        }

        IEnumerator Thread(DayEvent evt)
        {
            GameSession.Instance.RegisterTags(evt.tags);
            GameSession.Instance.Affect.Apply(evt.enterDelta);

            if (evt.eventId == "manana_mensaje")
            {
                _status.text = "en línea";
                AddBubble("Hey, cómo estás? Tengo un rato libre", me: true);
                yield return new WaitForSecondsRealtime(0.6f);
                _status.text = "visto · escribiendo…";
                yield return new WaitForSecondsRealtime(1.6f);
                _status.text = "visto";
                AddBubble("Se detuvo. El silencio pesa más que un no.", me: false, thought: true);
                AddBubble(evt.body, me: false, thought: true);
            }
            else if (evt.eventId == "mediodia_cafe")
            {
                _status.text = "en línea";
                AddBubble("hoy no puedo, perdón", me: false);
                yield return new WaitForSecondsRealtime(0.5f);
                AddBubble(evt.body, me: false, thought: true);
            }
            else
            {
                _status.text = "";
                AddBubble(evt.body, me: false, thought: true);
            }

            ShowOptions(evt);
        }

        void ShowOptions(DayEvent evt)
        {
            Clear(_optionsRoot);
            if (evt.options == null || evt.options.Count == 0)
            {
                var cont = UITheme.Button(_optionsRoot, "Continue", "Guardar el celu", UITheme.Accent, new Vector2(400, 56));
                cont.gameObject.AddComponent<LayoutElement>().preferredHeight = 56;
                cont.onClick.AddListener(Finish);
                return;
            }

            for (int i = 0; i < evt.options.Count; i++)
            {
                var opt = evt.options[i];
                var btn = UITheme.Button(_optionsRoot, "Opt" + i, opt.label, UITheme.MeterTrack, new Vector2(520, 64));
                btn.gameObject.AddComponent<LayoutElement>().preferredHeight = 64;
                var captured = opt;
                btn.onClick.AddListener(() => OnOption(captured));
            }
        }

        void OnOption(EventOption opt)
        {
            Clear(_optionsRoot);
            AddBubble(opt.label, me: true);

            if (opt.requiresSkillOffer && opt.suggestedSkill != SkillId.None && _skills.TryGet(opt.suggestedSkill, out _))
            {
                OfferSkill(opt);
                return;
            }

            Resolve(opt, false);
        }

        void OfferSkill(EventOption opt)
        {
            var def = _skills.GetDef(opt.suggestedSkill);
            string skillName = def != null ? def.displayName : opt.suggestedSkill.ToString();
            var prompt = UITheme.Text(_optionsRoot, "SkillPrompt",
                $"Antes de mandar: ¿usar «{skillName}»?",
                22, UITheme.AccentWarm, TextAnchor.MiddleCenter);
            prompt.gameObject.AddComponent<LayoutElement>().preferredHeight = 48;

            var use = UITheme.Button(_optionsRoot, "UseSkill", $"Usar {skillName}", UITheme.Good, new Vector2(420, 56));
            use.gameObject.AddComponent<LayoutElement>().preferredHeight = 56;
            use.onClick.AddListener(() =>
            {
                Clear(_optionsRoot);
                if (_skills.TryGet(opt.suggestedSkill, out var skill))
                    skill.Present(_hudRoot, success => Resolve(opt, success));
                else
                    Resolve(opt, false);
            });

            var skip = UITheme.Button(_optionsRoot, "SkipSkill", "Mandar igual", UITheme.MeterTrack, new Vector2(420, 56));
            skip.gameObject.AddComponent<LayoutElement>().preferredHeight = 56;
            skip.onClick.AddListener(() =>
            {
                Clear(_optionsRoot);
                Resolve(opt, false);
            });
        }

        void Resolve(EventOption opt, bool usedSkill)
        {
            GameSession.Instance.Affect.Apply(opt.delta);
            if (usedSkill)
                GameSession.Instance.Affect.Apply(opt.skillBonusDelta);

            string outcome = usedSkill && !string.IsNullOrWhiteSpace(opt.outcomeWithSkill)
                ? opt.outcomeWithSkill
                : opt.outcomeWithoutSkill;
            GameSession.Instance.LogOutcome(outcome);
            AddBubble(outcome, me: false, thought: true);

            var next = UITheme.Button(_optionsRoot, "Next", "Guardar el celu", UITheme.Accent, new Vector2(360, 56));
            next.gameObject.AddComponent<LayoutElement>().preferredHeight = 56;
            next.onClick.AddListener(Finish);
        }

        void Finish()
        {
            var done = _onFinished;
            _onFinished = null;
            Hide();
            done?.Invoke();
        }

        void AddBubble(string text, bool me, bool thought = false)
        {
            var color = thought ? new Color(0.18f, 0.2f, 0.26f, 1f)
                : me ? new Color(0.22f, 0.38f, 0.42f, 1f)
                : new Color(0.24f, 0.24f, 0.3f, 1f);
            var panel = UITheme.Panel(_bubbles, "Bubble", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, color);
            var le = panel.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = thought ? 110 : 72;
            le.minHeight = 56;
            var label = UITheme.Text(panel, "T", text, 20, UITheme.TextPrimary, TextAnchor.UpperLeft, thought ? FontStyle.Italic : FontStyle.Normal);
            UITheme.Stretch(label.rectTransform, new Vector2(0.04f, 0.08f), new Vector2(0.96f, 0.92f), Vector2.zero, Vector2.zero);
        }

        static void Clear(Transform root)
        {
            if (root == null) return;
            for (int i = root.childCount - 1; i >= 0; i--)
                Destroy(root.GetChild(i).gameObject);
        }
    }
}

using System;
using System.Collections.Generic;
using SeriousGameBPD.Core;
using SeriousGameBPD.Data;
using SeriousGameBPD.Skills;
using SeriousGameBPD.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace SeriousGameBPD.Narrative
{
    public class EventRunner : MonoBehaviour
    {
        Transform _root;
        SkillController _skills;
        DayEvent _current;
        EventOption _pendingOption;
        Action _onFinishedEvent;

        Text _blockLabel;
        Text _title;
        Text _body;
        Text _tags;
        RectTransform _optionsRoot;
        GameObject _outcomePanel;
        Text _outcomeText;

        public void Build(Transform parent, SkillController skills)
        {
            _skills = skills;
            _root = UITheme.Panel(parent, "EventPanel",
                new Vector2(0.08f, 0.06f), new Vector2(0.92f, 0.78f),
                Vector2.zero, Vector2.zero, UITheme.BgPanel);

            _blockLabel = UITheme.Text(_root, "Block", "", 22, UITheme.Accent, TextAnchor.UpperLeft);
            UITheme.Stretch(_blockLabel.rectTransform, new Vector2(0.04f, 0.90f), new Vector2(0.96f, 0.98f), Vector2.zero, Vector2.zero);

            _title = UITheme.Text(_root, "Title", "", 36, UITheme.TextPrimary, TextAnchor.UpperLeft, FontStyle.Bold);
            UITheme.Stretch(_title.rectTransform, new Vector2(0.04f, 0.78f), new Vector2(0.96f, 0.90f), Vector2.zero, Vector2.zero);

            _body = UITheme.Text(_root, "Body", "", 26, UITheme.TextPrimary, TextAnchor.UpperLeft);
            UITheme.Stretch(_body.rectTransform, new Vector2(0.04f, 0.48f), new Vector2(0.96f, 0.78f), Vector2.zero, Vector2.zero);

            _tags = UITheme.Text(_root, "Tags", "", 20, UITheme.TextMuted, TextAnchor.UpperLeft);
            UITheme.Stretch(_tags.rectTransform, new Vector2(0.04f, 0.40f), new Vector2(0.96f, 0.48f), Vector2.zero, Vector2.zero);

            var optionsGo = new GameObject("Options", typeof(RectTransform), typeof(VerticalLayoutGroup));
            _optionsRoot = optionsGo.GetComponent<RectTransform>();
            _optionsRoot.SetParent(_root, false);
            UITheme.Stretch(_optionsRoot, new Vector2(0.04f, 0.04f), new Vector2(0.96f, 0.38f), Vector2.zero, Vector2.zero);
            var layout = optionsGo.GetComponent<VerticalLayoutGroup>();
            layout.spacing = 14;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
        }

        public void Play(DayEvent evt, Action onFinished)
        {
            _current = evt;
            _onFinishedEvent = onFinished;
            ClearOptions();
            HideOutcome();

            if (evt == null)
            {
                onFinished?.Invoke();
                return;
            }

            GameSession.Instance.RegisterTags(evt.tags);
            GameSession.Instance.Affect.Apply(evt.enterDelta);

            _blockLabel.text = BlockLabel(evt.block);
            _title.text = evt.title;
            _body.text = evt.body;
            _tags.text = FormatTags(evt.tags);

            if (evt.options == null || evt.options.Count == 0)
            {
                var cont = UITheme.Button(_optionsRoot, "Continue", "Continuar", UITheme.Accent, new Vector2(400, 64));
                cont.gameObject.AddComponent<LayoutElement>().preferredHeight = 64;
                cont.onClick.AddListener(() => _onFinishedEvent?.Invoke());
                return;
            }

            for (int i = 0; i < evt.options.Count; i++)
            {
                var opt = evt.options[i];
                var btn = UITheme.Button(_optionsRoot, "Opt" + i, opt.label, UITheme.MeterTrack, new Vector2(800, 70));
                btn.gameObject.AddComponent<LayoutElement>().preferredHeight = 72;
                var captured = opt;
                btn.onClick.AddListener(() => OnOptionChosen(captured));
            }
        }

        void OnOptionChosen(EventOption opt)
        {
            _pendingOption = opt;
            ClearOptions();

            if (opt.requiresSkillOffer && opt.suggestedSkill != SkillId.None && _skills.TryGet(opt.suggestedSkill, out _))
            {
                OfferSkill(opt);
                return;
            }

            Resolve(opt, usedSkill: false);
        }

        void OfferSkill(EventOption opt)
        {
            var def = _skills.GetDef(opt.suggestedSkill);
            string skillName = def != null ? def.displayName : opt.suggestedSkill.ToString();

            var prompt = UITheme.Text(_optionsRoot, "SkillPrompt",
                $"La impulsividad / intensidad está alta. ¿Querés usar la herramienta «{skillName}» antes de resolver?",
                24, UITheme.AccentWarm, TextAnchor.MiddleCenter);
            prompt.gameObject.AddComponent<LayoutElement>().preferredHeight = 70;

            var use = UITheme.Button(_optionsRoot, "UseSkill", $"Usar {skillName}", UITheme.Good, new Vector2(500, 68));
            use.gameObject.AddComponent<LayoutElement>().preferredHeight = 68;
            use.onClick.AddListener(() =>
            {
                ClearOptions();
                if (_skills.TryGet(opt.suggestedSkill, out var skill))
                {
                    skill.Present(_root, success =>
                    {
                        Resolve(opt, usedSkill: success);
                    });
                }
                else
                {
                    Resolve(opt, usedSkill: false);
                }
            });

            var skip = UITheme.Button(_optionsRoot, "SkipSkill", "Seguir sin la herramienta", UITheme.MeterTrack, new Vector2(500, 68));
            skip.gameObject.AddComponent<LayoutElement>().preferredHeight = 68;
            skip.onClick.AddListener(() =>
            {
                ClearOptions();
                Resolve(opt, usedSkill: false);
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
            ShowOutcome(outcome);
        }

        void ShowOutcome(string text)
        {
            HideOutcome();
            _outcomePanel = UITheme.Panel(_root, "Outcome",
                new Vector2(0.04f, 0.04f), new Vector2(0.96f, 0.38f),
                Vector2.zero, Vector2.zero, new Color(0.12f, 0.16f, 0.20f, 1f)).gameObject;

            _outcomeText = UITheme.Text(_outcomePanel.transform, "OutcomeText", text, 26, UITheme.TextPrimary, TextAnchor.UpperLeft);
            UITheme.Stretch(_outcomeText.rectTransform, new Vector2(0.04f, 0.28f), new Vector2(0.96f, 0.92f), Vector2.zero, Vector2.zero);

            var next = UITheme.Button(_outcomePanel.transform, "Next", "Seguir", UITheme.Accent, new Vector2(280, 60));
            UITheme.Stretch(next.GetComponent<RectTransform>(), new Vector2(0.5f, 0.08f), new Vector2(0.5f, 0.08f), new Vector2(-140, -30), new Vector2(140, 30));
            next.onClick.AddListener(() =>
            {
                HideOutcome();
                _onFinishedEvent?.Invoke();
            });
        }

        void HideOutcome()
        {
            if (_outcomePanel != null)
            {
                Destroy(_outcomePanel);
                _outcomePanel = null;
            }
        }

        void ClearOptions()
        {
            if (_optionsRoot == null) return;
            for (int i = _optionsRoot.childCount - 1; i >= 0; i--)
                Destroy(_optionsRoot.GetChild(i).gameObject);
        }

        static string BlockLabel(DayBlock block) => block switch
        {
            DayBlock.Manana => "Bloque: Mañana",
            DayBlock.Mediodia => "Bloque: Mediodía",
            DayBlock.Tarde => "Bloque: Tarde",
            DayBlock.Noche => "Bloque: Noche",
            _ => ""
        };

        static string FormatTags(List<EducationalTag> tags)
        {
            if (tags == null || tags.Count == 0) return "";
            var parts = new List<string>();
            foreach (var t in tags)
                parts.Add(TagLabel(t));
            return "Temas: " + string.Join(" · ", parts);
        }

        public static string TagLabel(EducationalTag tag) => tag switch
        {
            EducationalTag.Abandono => "miedo al abandono",
            EducationalTag.BlancoNegro => "pensamiento blanco/negro",
            EducationalTag.Vacio => "vacío",
            EducationalTag.Ira => "ira intensa",
            EducationalTag.Idealizacion => "idealización / devaluación",
            EducationalTag.Desregulacion => "desregulación emocional",
            EducationalTag.Identidad => "identidad inestable",
            _ => tag.ToString()
        };
    }
}

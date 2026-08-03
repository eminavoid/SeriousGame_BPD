using System;
using System.Collections;
using SeriousGameBPD.Data;
using SeriousGameBPD.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace SeriousGameBPD.Skills
{
    public class OppositeActionSkill : SkillBase
    {
        public OppositeActionSkill(SkillDef def) : base(def) { }

        public override void Present(Transform uiParent, Action<bool> onComplete)
        {
            var host = uiParent.gameObject.AddComponent<OppositeActionRunner>();
            host.Run(Def, () =>
            {
                ApplySuccess();
                onComplete?.Invoke(true);
            }, () => onComplete?.Invoke(false));
        }
    }

    class OppositeActionRunner : MonoBehaviour
    {
        public void Run(SkillDef def, Action onDone, Action onCancel)
        {
            StartCoroutine(Flow(def, onDone, onCancel));
        }

        IEnumerator Flow(SkillDef def, Action onDone, Action onCancel)
        {
            var overlay = UITheme.Panel(transform, "OppositeOverlay",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
                new Color(0.06f, 0.07f, 0.11f, 0.94f));

            var title = UITheme.Text(overlay, "Title", "Chequear hechos + Acción opuesta", 36, UITheme.Accent, TextAnchor.MiddleCenter, FontStyle.Bold);
            UITheme.Stretch(title.rectTransform, new Vector2(0.08f, 0.86f), new Vector2(0.92f, 0.96f), Vector2.zero, Vector2.zero);

            var prompt = UITheme.Text(overlay, "Prompt",
                "Separá HECHOS de INTERPRETACIONES. Después elegí si el impulso encaja… o si conviene la acción opuesta.",
                24, UITheme.TextPrimary, TextAnchor.UpperCenter);
            UITheme.Stretch(prompt.rectTransform, new Vector2(0.1f, 0.72f), new Vector2(0.9f, 0.85f), Vector2.zero, Vector2.zero);

            var facts = UITheme.Text(overlay, "Facts",
                "Hechos (ejemplo): alguien canceló / no respondió aún / dio un feedback.\n" +
                "Interpretación típica: 'no le importo', 'me abandona', 'soy un fracaso'.",
                24, UITheme.TextMuted, TextAnchor.UpperLeft);
            UITheme.Stretch(facts.rectTransform, new Vector2(0.12f, 0.48f), new Vector2(0.88f, 0.70f), Vector2.zero, Vector2.zero);

            bool decided = false;
            bool success = false;

            var use = UITheme.Button(overlay, "Use",
                "Elegir acción opuesta al impulso (probar el gris)",
                UITheme.Good, new Vector2(720, 80));
            UITheme.Stretch(use.GetComponent<RectTransform>(), new Vector2(0.5f, 0.30f), new Vector2(0.5f, 0.30f), new Vector2(-360, -40), new Vector2(360, 40));
            use.onClick.AddListener(() => { success = true; decided = true; });

            var cancel = UITheme.Button(overlay, "Cancel", "Seguir con el impulso original", UITheme.MeterTrack, new Vector2(520, 70));
            UITheme.Stretch(cancel.GetComponent<RectTransform>(), new Vector2(0.5f, 0.14f), new Vector2(0.5f, 0.14f), new Vector2(-260, -35), new Vector2(260, 35));
            cancel.onClick.AddListener(() => { success = false; decided = true; });

            var tip = UITheme.Text(overlay, "Tip", def != null ? def.howToUse : "", 20, UITheme.TextMuted, TextAnchor.UpperCenter);
            UITheme.Stretch(tip.rectTransform, new Vector2(0.15f, 0.02f), new Vector2(0.85f, 0.10f), Vector2.zero, Vector2.zero);

            while (!decided) yield return null;
            Destroy(overlay.gameObject);
            if (success) onDone?.Invoke();
            else onCancel?.Invoke();
            Destroy(this);
        }
    }
}

using System;
using System.Collections;
using SeriousGameBPD.Data;
using SeriousGameBPD.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace SeriousGameBPD.Skills
{
    public class StopSkill : SkillBase
    {
        public StopSkill(SkillDef def) : base(def) { }

        public override void Present(Transform uiParent, Action<bool> onComplete)
        {
            var host = uiParent.gameObject.AddComponent<StopSkillRunner>();
            host.Run(Def, () =>
            {
                ApplySuccess();
                onComplete?.Invoke(true);
            }, () => onComplete?.Invoke(false));
        }
    }

    class StopSkillRunner : MonoBehaviour
    {
        public void Run(SkillDef def, Action onDone, Action onCancel)
        {
            StartCoroutine(Flow(def, onDone, onCancel));
        }

        IEnumerator Flow(SkillDef def, Action onDone, Action onCancel)
        {
            var overlay = UITheme.Panel(transform, "StopOverlay",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
                new Color(0.05f, 0.07f, 0.1f, 0.92f));

            var title = UITheme.Text(overlay, "Title", "Herramienta: STOP", 40, UITheme.Accent, TextAnchor.MiddleCenter, FontStyle.Bold);
            UITheme.Stretch(title.rectTransform, new Vector2(0.1f, 0.82f), new Vector2(0.9f, 0.95f), Vector2.zero, Vector2.zero);

            var step = UITheme.Text(overlay, "Step", "", 32, UITheme.TextPrimary, TextAnchor.MiddleCenter);
            UITheme.Stretch(step.rectTransform, new Vector2(0.1f, 0.45f), new Vector2(0.9f, 0.78f), Vector2.zero, Vector2.zero);

            var hint = UITheme.Text(overlay, "Hint", def != null ? def.howToUse : "", 22, UITheme.TextMuted, TextAnchor.UpperCenter);
            UITheme.Stretch(hint.rectTransform, new Vector2(0.15f, 0.28f), new Vector2(0.85f, 0.44f), Vector2.zero, Vector2.zero);

            string[] steps =
            {
                "S — STOP: Paré. No actúes todavía.",
                "T — Take a step back: Un paso atrás interno.",
                "O — Observe: ¿Qué siento en el cuerpo? ¿Qué historia me cuento?",
                "P — Proceed: Elegí el siguiente paso con un poco más de claridad."
            };

            foreach (var s in steps)
            {
                step.text = s;
                yield return new WaitForSecondsRealtime(1.35f);
            }

            bool decided = false;
            bool success = false;
            var ok = UITheme.Button(overlay, "Ok", "Usar STOP y continuar", UITheme.Good, new Vector2(420, 70));
            UITheme.Stretch(ok.GetComponent<RectTransform>(), new Vector2(0.5f, 0.12f), new Vector2(0.5f, 0.12f), new Vector2(-450, -35), new Vector2(-30, 35));
            ok.onClick.AddListener(() => { success = true; decided = true; });

            var skip = UITheme.Button(overlay, "Skip", "Volver sin usarla", UITheme.MeterTrack, new Vector2(320, 70));
            UITheme.Stretch(skip.GetComponent<RectTransform>(), new Vector2(0.5f, 0.12f), new Vector2(0.5f, 0.12f), new Vector2(30, -35), new Vector2(450, 35));
            skip.onClick.AddListener(() => { success = false; decided = true; });

            while (!decided) yield return null;

            Destroy(overlay.gameObject);
            if (success) onDone?.Invoke();
            else onCancel?.Invoke();
            Destroy(this);
        }
    }
}

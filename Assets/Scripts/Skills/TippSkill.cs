using System;
using System.Collections;
using SeriousGameBPD.Data;
using SeriousGameBPD.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace SeriousGameBPD.Skills
{
    public class TippSkill : SkillBase
    {
        public TippSkill(SkillDef def) : base(def) { }

        public override void Present(Transform uiParent, Action<bool> onComplete)
        {
            var host = uiParent.gameObject.AddComponent<TippSkillRunner>();
            host.Run(Def, () =>
            {
                ApplySuccess();
                onComplete?.Invoke(true);
            }, () => onComplete?.Invoke(false));
        }
    }

    class TippSkillRunner : MonoBehaviour
    {
        public void Run(SkillDef def, Action onDone, Action onCancel)
        {
            StartCoroutine(Flow(def, onDone, onCancel));
        }

        IEnumerator Flow(SkillDef def, Action onDone, Action onCancel)
        {
            var overlay = UITheme.Panel(transform, "TippOverlay",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
                new Color(0.05f, 0.08f, 0.12f, 0.92f));

            var title = UITheme.Text(overlay, "Title", "Herramienta: TIPP (lite)", 40, UITheme.Accent, TextAnchor.MiddleCenter, FontStyle.Bold);
            UITheme.Stretch(title.rectTransform, new Vector2(0.1f, 0.84f), new Vector2(0.9f, 0.95f), Vector2.zero, Vector2.zero);

            var body = UITheme.Text(overlay, "Body",
                "Cuando la urgencia es alta, el cuerpo manda. TIPP busca bajar esa temperatura fisiológica un poco.",
                26, UITheme.TextPrimary, TextAnchor.MiddleCenter);
            UITheme.Stretch(body.rectTransform, new Vector2(0.12f, 0.68f), new Vector2(0.88f, 0.82f), Vector2.zero, Vector2.zero);

            var step = UITheme.Text(overlay, "Step", "", 30, UITheme.AccentWarm, TextAnchor.MiddleCenter);
            UITheme.Stretch(step.rectTransform, new Vector2(0.1f, 0.40f), new Vector2(0.9f, 0.66f), Vector2.zero, Vector2.zero);

            var barTrack = UITheme.Panel(overlay, "BreathTrack",
                new Vector2(0.2f, 0.28f), new Vector2(0.8f, 0.34f), Vector2.zero, Vector2.zero, UITheme.MeterTrack);
            var barFill = UITheme.Panel(barTrack, "Fill",
                Vector2.zero, new Vector2(0f, 1f), Vector2.zero, Vector2.zero, UITheme.Accent);

            string[] steps =
            {
                "T — Temperatura: imaginá agua fría en la cara / nuca.",
                "I — Intensidad breve: tensá y soltá manos, brazos, mandíbula.",
                "P — Respiración pausada: inhalá 4 · sostené 2 · exhalá 6.",
                "P — Relajación: soltá hombros. La urgencia puede bajar sin 'arreglar' todo."
            };

            for (int i = 0; i < steps.Length; i++)
            {
                step.text = steps[i];
                float t = 0f;
                float dur = i == 2 ? 4.5f : 2.2f;
                while (t < dur)
                {
                    t += Time.unscaledDeltaTime;
                    float a = Mathf.SmoothStep(0f, 1f, t / dur);
                    barFill.anchorMax = new Vector2(a, 1f);
                    yield return null;
                }
            }

            bool decided = false;
            bool success = false;
            var ok = UITheme.Button(overlay, "Ok", "Completar TIPP", UITheme.Good, new Vector2(360, 70));
            UITheme.Stretch(ok.GetComponent<RectTransform>(), new Vector2(0.5f, 0.12f), new Vector2(0.5f, 0.12f), new Vector2(-420, -35), new Vector2(-40, 35));
            ok.onClick.AddListener(() => { success = true; decided = true; });

            var skip = UITheme.Button(overlay, "Skip", "Cancelar", UITheme.MeterTrack, new Vector2(280, 70));
            UITheme.Stretch(skip.GetComponent<RectTransform>(), new Vector2(0.5f, 0.12f), new Vector2(0.5f, 0.12f), new Vector2(40, -35), new Vector2(420, 35));
            skip.onClick.AddListener(() => { success = false; decided = true; });

            while (!decided) yield return null;
            Destroy(overlay.gameObject);
            if (success) onDone?.Invoke();
            else onCancel?.Invoke();
            Destroy(this);
        }
    }
}

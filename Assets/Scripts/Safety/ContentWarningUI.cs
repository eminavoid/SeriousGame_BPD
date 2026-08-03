using System;
using SeriousGameBPD.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace SeriousGameBPD.Safety
{
    public class ContentWarningUI : MonoBehaviour
    {
        public void Build(Transform parent, Action onContinue)
        {
            var bg = UITheme.Panel(parent, "WarningBG",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, UITheme.BgDeep);

            var panel = UITheme.Panel(bg, "WarningPanel",
                new Vector2(0.15f, 0.12f), new Vector2(0.85f, 0.88f),
                Vector2.zero, Vector2.zero, UITheme.BgPanel);

            var title = UITheme.Text(panel, "Title", "Aviso de contenido", 48, UITheme.AccentWarm, TextAnchor.MiddleCenter, FontStyle.Bold);
            UITheme.Stretch(title.rectTransform, new Vector2(0.05f, 0.82f), new Vector2(0.95f, 0.95f), Vector2.zero, Vector2.zero);

            var body = UITheme.Text(panel, "Body",
                "Este serious game habla de emociones intensas, miedo al abandono, vacío e impulsividad " +
                "asociados al Trastorno Límite de la Personalidad (TLP).\n\n" +
                "Puede resultar sensibilizante si estás atravesando una crisis.\n\n" +
                "Es un recurso educativo y de concientización. NO diagnostica, NO reemplaza terapia ni tratamiento profesional.\n\n" +
                "Si estás en peligro o pensás en lastimarte, pedí ayuda ahora a un profesional de la salud o a un servicio de crisis local.",
                26, UITheme.TextPrimary, TextAnchor.UpperLeft);
            UITheme.Stretch(body.rectTransform, new Vector2(0.08f, 0.28f), new Vector2(0.92f, 0.80f), Vector2.zero, Vector2.zero);

            var disclaimer = UITheme.Text(panel, "Disclaimer",
                "Al continuar aceptás que jugás bajo tu propio cuidado emocional.",
                20, UITheme.TextMuted, TextAnchor.MiddleCenter);
            UITheme.Stretch(disclaimer.rectTransform, new Vector2(0.1f, 0.18f), new Vector2(0.9f, 0.26f), Vector2.zero, Vector2.zero);

            var btn = UITheme.Button(panel, "Continue", "Entiendo — Continuar", UITheme.Accent, new Vector2(420, 72));
            UITheme.Stretch(btn.GetComponent<RectTransform>(), new Vector2(0.5f, 0.08f), new Vector2(0.5f, 0.08f), new Vector2(-210, -36), new Vector2(210, 36));
            btn.onClick.AddListener(() => onContinue?.Invoke());
        }
    }
}

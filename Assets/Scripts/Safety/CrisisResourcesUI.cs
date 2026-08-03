using SeriousGameBPD.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace SeriousGameBPD.Safety
{
    public static class CrisisResources
    {
        public const string Title = "Recursos de ayuda";

        public const string Body =
            "Si estás en crisis o pensás en lastimarte, buscá ayuda inmediata.\n\n" +
            "• Argentina — Línea de prevención del suicidio: 135 (CABA y GBA) / 0800-345-1435\n" +
            "• Emergencias: 911\n" +
            "• Hablá con alguien de confianza y/o un profesional de salud mental\n\n" +
            "Recursos educativos (no reemplazan atención):\n" +
            "• Información sobre TLP / BPD de asociaciones y guías clínicas locales\n" +
            "• Terapias con evidencia para TLP incluyen DBT y otros abordajes especializados\n\n" +
            "Este juego no es un servicio de emergencia ni un diagnóstico.";
    }

    public class CrisisResourcesUI
    {
        public static RectTransform BuildSection(Transform parent)
        {
            var box = UITheme.Panel(parent, "CrisisBox",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
                new Color(0.20f, 0.14f, 0.14f, 1f));

            var title = UITheme.Text(box, "CrisisTitle", CrisisResources.Title, 28, UITheme.AccentWarm, TextAnchor.UpperLeft, FontStyle.Bold);
            UITheme.Stretch(title.rectTransform, new Vector2(0.03f, 0.82f), new Vector2(0.97f, 0.98f), Vector2.zero, Vector2.zero);

            var body = UITheme.Text(box, "CrisisBody", CrisisResources.Body, 20, UITheme.TextPrimary, TextAnchor.UpperLeft);
            UITheme.Stretch(body.rectTransform, new Vector2(0.03f, 0.04f), new Vector2(0.97f, 0.82f), Vector2.zero, Vector2.zero);
            return box;
        }
    }
}

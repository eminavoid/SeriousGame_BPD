using System;
using System.Text;
using SeriousGameBPD.Core;
using SeriousGameBPD.Data;
using SeriousGameBPD.Narrative;
using SeriousGameBPD.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace SeriousGameBPD.Safety
{
    public class DebriefController : MonoBehaviour
    {
        Transform _host;
        Action _onPlayAgain;
        Action _onBackToBoot;
        bool _built;

        public void BuildInto(Transform parent, Action onPlayAgain, Action onBackToBoot)
        {
            _host = parent;
            _onPlayAgain = onPlayAgain;
            _onBackToBoot = onBackToBoot;
            BuildUi();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoStart()
        {
            if (AppFlow.Instance != null) return;
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene.name != SceneFlow.DebriefScene) return;
            if (FindFirstObjectByType<DebriefController>() != null) return;
            var go = new GameObject("DebriefController");
            go.AddComponent<DebriefController>();
        }

        void Start()
        {
            if (_built || AppFlow.Instance != null) return;

            if (GameSession.Instance == null)
            {
                var go = new GameObject("GameSession");
                go.AddComponent<GameSession>();
                GameSession.Instance.BeginNewDay(ContentFactory.LoadOrCreateDayPlan());
            }

            UITheme.EnsureEventSystem();
            var canvas = UITheme.CreateCanvas("DebriefCanvas");
            _host = canvas.transform;
            _onPlayAgain = () =>
            {
                GameSession.Instance.BeginNewDay(ContentFactory.LoadOrCreateDayPlan());
                SceneFlow.LoadDay();
            };
            _onBackToBoot = SceneFlow.LoadBoot;
            BuildUi();
        }

        void BuildUi()
        {
            _built = true;
            var bg = UITheme.Panel(_host, "BG",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, UITheme.BgDeep);

            var title = UITheme.Text(bg, "Title", "Cierre del día — ¿Qué viviste?", 40, UITheme.TextPrimary, TextAnchor.MiddleLeft, FontStyle.Bold);
            UITheme.Stretch(title.rectTransform, new Vector2(0.06f, 0.90f), new Vector2(0.70f, 0.98f), Vector2.zero, Vector2.zero);

            var disclaimer = UITheme.Text(bg, "Disclaimer",
                "Este juego es educativo y de concientización. No diagnostica ni reemplaza tratamiento. Si estás en crisis, pedí ayuda profesional.",
                18, UITheme.AccentWarm, TextAnchor.MiddleLeft);
            UITheme.Stretch(disclaimer.rectTransform, new Vector2(0.06f, 0.84f), new Vector2(0.94f, 0.90f), Vector2.zero, Vector2.zero);

            var scroll = UITheme.ScrollArea(bg, "Scroll", out var content);
            UITheme.Stretch(scroll.GetComponent<RectTransform>(), new Vector2(0.06f, 0.22f), new Vector2(0.58f, 0.82f), Vector2.zero, Vector2.zero);

            AddSection(content, "Temas que aparecieron hoy", BuildTagsSummary());
            AddSection(content, "Cómo cerraron algunas escenas", BuildOutcomesSummary());
            AddSection(content, "Herramientas que usaste", BuildSkillsSummary());
            AddGlossary(content);

            var crisisHost = UITheme.Panel(bg, "CrisisHost",
                new Vector2(0.60f, 0.22f), new Vector2(0.94f, 0.82f),
                Vector2.zero, Vector2.zero, new Color(0, 0, 0, 0));
            CrisisResourcesUI.BuildSection(crisisHost);

            var again = UITheme.Button(bg, "Again", "Jugar de nuevo", UITheme.Accent, new Vector2(280, 64));
            UITheme.Stretch(again.GetComponent<RectTransform>(), new Vector2(0.06f, 0.06f), new Vector2(0.06f, 0.06f), new Vector2(0, -32), new Vector2(280, 32));
            again.onClick.AddListener(() =>
            {
                GameSession.Instance.BeginNewDay(ContentFactory.LoadOrCreateDayPlan());
                if (_onPlayAgain != null) _onPlayAgain.Invoke();
                else SceneFlow.LoadDay();
            });

            var quit = UITheme.Button(bg, "Quit", "Salir al inicio", UITheme.MeterTrack, new Vector2(280, 64));
            UITheme.Stretch(quit.GetComponent<RectTransform>(), new Vector2(0.28f, 0.06f), new Vector2(0.28f, 0.06f), new Vector2(0, -32), new Vector2(280, 32));
            quit.onClick.AddListener(() =>
            {
                if (_onBackToBoot != null) _onBackToBoot.Invoke();
                else SceneFlow.LoadBoot();
            });
        }

        static void AddSection(RectTransform parent, string heading, string body)
        {
            var h = UITheme.Text(parent, "H", heading, 26, UITheme.Accent, TextAnchor.UpperLeft, FontStyle.Bold);
            h.gameObject.AddComponent<LayoutElement>().preferredHeight = 36;
            var b = UITheme.Text(parent, "B", body, 22, UITheme.TextPrimary, TextAnchor.UpperLeft);
            var le = b.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = Mathf.Clamp(40 + body.Length * 0.35f, 80, 420);
        }

        void AddGlossary(RectTransform parent)
        {
            var plan = GameSession.Instance.DayPlan;
            if (plan == null || plan.glossary == null) return;

            var h = UITheme.Text(parent, "GlossH", "Glosario breve", 26, UITheme.Accent, TextAnchor.UpperLeft, FontStyle.Bold);
            h.gameObject.AddComponent<LayoutElement>().preferredHeight = 36;

            foreach (var entry in plan.glossary)
            {
                if (entry == null) continue;
                var block = $"{entry.title}\n{entry.body}";
                var t = UITheme.Text(parent, entry.title, block, 20, UITheme.TextMuted, TextAnchor.UpperLeft);
                t.gameObject.AddComponent<LayoutElement>().preferredHeight = Mathf.Clamp(60 + entry.body.Length * 0.4f, 100, 280);
            }
        }

        string BuildTagsSummary()
        {
            var tags = GameSession.Instance.ExperiencedTags;
            if (tags == null || tags.Count == 0)
                return "Hoy no se registraron temas (¿saliste muy temprano?).";

            var sb = new StringBuilder();
            sb.AppendLine("Estas experiencias aparecen a menudo cuando se habla de TLP. No significan un diagnóstico:");
            foreach (var tag in tags)
            {
                sb.Append("• ");
                sb.Append(EventRunner.TagLabel(tag));
                sb.Append(" — ");
                sb.AppendLine(TagExplainer(tag));
            }
            return sb.ToString();
        }

        string BuildOutcomesSummary()
        {
            var log = GameSession.Instance.OutcomeLog;
            if (log == null || log.Count == 0)
                return "No hay cierres registrados.";

            var sb = new StringBuilder();
            int show = Mathf.Min(log.Count, 4);
            for (int i = 0; i < show; i++)
            {
                sb.Append("• ");
                sb.AppendLine(log[i]);
            }
            return sb.ToString();
        }

        string BuildSkillsSummary()
        {
            var used = GameSession.Instance.UsedSkills;
            if (used == null || used.Count == 0)
                return "No usaste herramientas DBT hoy. Está bien: a veces solo notar la ola ya es un paso. Podés reintentar el día y probar STOP, TIPP o Acción opuesta.";

            var sb = new StringBuilder();
            sb.AppendLine("Practicaste:");
            foreach (var id in used)
            {
                sb.Append("• ");
                sb.AppendLine(id switch
                {
                    SkillId.Stop => "STOP — frenar antes de actuar",
                    SkillId.Tipp => "TIPP — bajar la urgencia del cuerpo",
                    SkillId.OppositeAction => "Acción opuesta / chequear hechos",
                    _ => id.ToString()
                });
            }
            sb.AppendLine();
            sb.Append("En la vida real estas habilidades se entrenan con práctica y, idealmente, con acompañamiento profesional.");
            return sb.ToString();
        }

        static string TagExplainer(EducationalTag tag) => tag switch
        {
            EducationalTag.Abandono => "la alarma de 'me dejan' puede sonar más fuerte que los hechos.",
            EducationalTag.BlancoNegro => "todo o nada deja poco espacio para el gris de las relaciones.",
            EducationalTag.Vacio => "una sensación de hueco o niebla que pide alivio ya.",
            EducationalTag.Ira => "enojo que sube rápido y puede confundirse con rechazo.",
            EducationalTag.Idealizacion => "subir a alguien a un pedestal y bajarlo de golpe.",
            EducationalTag.Desregulacion => "emociones más altas, más rápidas y más lentas para bajar.",
            EducationalTag.Identidad => "dudas sobre quién soy según el momento o la relación.",
            _ => ""
        };
    }
}

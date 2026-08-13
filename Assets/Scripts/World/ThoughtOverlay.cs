using System.Collections;
using SeriousGameBPD.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace SeriousGameBPD.World
{
    public class ThoughtOverlay : MonoBehaviour
    {
        Text _text;
        CanvasGroup _group;
        Coroutine _fade;

        public void Build(Transform parent)
        {
            var panel = UITheme.Panel(parent, "Thought",
                new Vector2(0.18f, 0.22f), new Vector2(0.82f, 0.38f),
                Vector2.zero, Vector2.zero, new Color(0.08f, 0.09f, 0.12f, 0.0f));
            _group = panel.gameObject.AddComponent<CanvasGroup>();
            _group.alpha = 0f;
            _group.blocksRaycasts = false;
            _group.interactable = false;

            _text = UITheme.Text(panel, "Line", "", 26, UITheme.TextPrimary, TextAnchor.MiddleCenter, FontStyle.Italic);
            UITheme.Stretch(_text.rectTransform, new Vector2(0.04f, 0.1f), new Vector2(0.96f, 0.9f), Vector2.zero, Vector2.zero);
        }

        public void Show(string line, float hold = 3.4f)
        {
            if (_text == null || string.IsNullOrWhiteSpace(line)) return;
            _text.text = line;
            if (_fade != null)
                StopCoroutine(_fade);
            _fade = StartCoroutine(Fade(hold));
        }

        IEnumerator Fade(float hold)
        {
            float t = 0f;
            while (t < 0.35f)
            {
                t += Time.unscaledDeltaTime;
                _group.alpha = Mathf.Lerp(0f, 1f, t / 0.35f);
                yield return null;
            }

            _group.alpha = 1f;
            yield return new WaitForSecondsRealtime(hold);

            t = 0f;
            while (t < 0.5f)
            {
                t += Time.unscaledDeltaTime;
                _group.alpha = Mathf.Lerp(1f, 0f, t / 0.5f);
                yield return null;
            }

            _group.alpha = 0f;
            _fade = null;
        }
    }
}

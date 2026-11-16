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
            var bar = new GameObject("Thought", typeof(RectTransform));
            var barRt = bar.GetComponent<RectTransform>();
            barRt.SetParent(parent, false);
            barRt.anchorMin = barRt.anchorMax = new Vector2(0.5f, 0.26f);
            barRt.pivot = new Vector2(0.5f, 0.5f);
            barRt.sizeDelta = new Vector2(1080f, 210f);

            _group = bar.AddComponent<CanvasGroup>();
            _group.alpha = 0f;
            _group.blocksRaycasts = false;
            _group.interactable = false;

            var photoGo = new GameObject("Portrait", typeof(RectTransform), typeof(Image));
            var photoRt = photoGo.GetComponent<RectTransform>();
            photoRt.SetParent(barRt, false);
            photoRt.anchorMin = new Vector2(0f, 0.5f);
            photoRt.anchorMax = new Vector2(0f, 0.5f);
            photoRt.pivot = new Vector2(0f, 0.5f);
            photoRt.anchoredPosition = Vector2.zero;
            photoRt.sizeDelta = new Vector2(200f, 198f);
            var photo = photoGo.GetComponent<Image>();
            photo.raycastTarget = false;
            UiArt.Apply(photo, UiArt.ThoughtPhoto, false);

            var plateGo = new GameObject("Plate", typeof(RectTransform), typeof(Image));
            var plateRt = plateGo.GetComponent<RectTransform>();
            plateRt.SetParent(barRt, false);
            plateRt.anchorMin = Vector2.zero;
            plateRt.anchorMax = Vector2.one;
            plateRt.offsetMin = new Vector2(214f, 10f);
            plateRt.offsetMax = new Vector2(0f, -10f);
            var plate = plateGo.GetComponent<Image>();
            plate.raycastTarget = false;
            UiArt.Apply(plate, UiArt.ThoughtPlate, true);

            _text = UITheme.Text(plateRt, "Line", "", 24, UiArt.Ink, TextAnchor.MiddleCenter, FontStyle.Italic);
            _text.resizeTextForBestFit = true;
            _text.resizeTextMinSize = 16;
            _text.resizeTextMaxSize = 24;
            UITheme.Stretch(_text.rectTransform, Vector2.zero, Vector2.one, new Vector2(36f, 16f), new Vector2(-28f, -16f));
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

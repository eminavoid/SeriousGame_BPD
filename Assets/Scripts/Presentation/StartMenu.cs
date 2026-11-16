using System;
using UnityEngine;
using UnityEngine.UI;

namespace SeriousGameBPD.Presentation
{
    /// <summary>
    /// Primera pantalla. Usa el mismo panel, fondo y botón pill que la pausa.
    /// </summary>
    public class StartMenu : MonoBehaviour
    {
        public void Build(Transform parent, Action onStart)
        {
            var screen = new GameObject("StartScreen", typeof(RectTransform));
            var screenRt = screen.GetComponent<RectTransform>();
            screenRt.SetParent(parent, false);
            UITheme.Stretch(screenRt, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            UITheme.Panel(screenRt, "Dim",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
                new Color(0.08f, 0.05f, 0.03f, 0.55f));

            var card = new GameObject("Card", typeof(RectTransform), typeof(Image));
            var cardRt = card.GetComponent<RectTransform>();
            cardRt.SetParent(screenRt, false);
            cardRt.anchorMin = cardRt.anchorMax = new Vector2(0.5f, 0.5f);
            cardRt.pivot = new Vector2(0.5f, 0.5f);
            cardRt.sizeDelta = new Vector2(760f, 540f);
            UiArt.Apply(card.GetComponent<Image>(), UiArt.PausePanel, true);

            var title = UITheme.Text(cardRt, "Title", "Inicio", 42, UiArt.Ink, TextAnchor.MiddleCenter, FontStyle.Bold);
            UITheme.Stretch(title.rectTransform, new Vector2(0.12f, 0.62f), new Vector2(0.88f, 0.82f), Vector2.zero, Vector2.zero);

            MakeButton(cardRt, "Empezar", new Vector2(0f, -20f), onStart);
        }

        static void MakeButton(RectTransform parent, string label, Vector2 anchored, Action onClick)
        {
            var go = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchored;
            rt.sizeDelta = new Vector2(500f, 118f);

            var image = go.GetComponent<Image>();
            UiArt.Apply(image, UiArt.ButtonFace, false);

            var button = go.GetComponent<Button>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 0.95f, 0.88f, 1f);
            colors.pressedColor = new Color(0.82f, 0.74f, 0.64f, 1f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
            button.onClick.AddListener(() => onClick?.Invoke());

            var text = UITheme.Text(rt, "Label", label, 28, UiArt.Cream, TextAnchor.MiddleCenter, FontStyle.Bold);
            UITheme.Stretch(text.rectTransform, Vector2.zero, Vector2.one, new Vector2(28f, 10f), new Vector2(-28f, -10f));
        }
    }
}

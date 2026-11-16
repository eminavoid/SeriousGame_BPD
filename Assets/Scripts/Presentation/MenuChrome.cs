using System;
using UnityEngine;
using UnityEngine.UI;

namespace SeriousGameBPD.Presentation
{
    /// <summary>
    /// Panel, fondo y botones pill compartidos por el inicio y la pausa.
    /// </summary>
    public static class MenuChrome
    {
        public static readonly Color InkStrong = new Color(0.15f, 0.07f, 0.03f, 1f);
        public static readonly Color InkSoft = new Color(0.40f, 0.25f, 0.13f, 1f);
        static readonly Color Dim = new Color(0.07f, 0.04f, 0.02f, 0.72f);

        public static RectTransform Card(Transform parent, string screenName, float width, float height)
        {
            var screen = new GameObject(screenName, typeof(RectTransform));
            var screenRt = screen.GetComponent<RectTransform>();
            screenRt.SetParent(parent, false);
            UITheme.Stretch(screenRt, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            UITheme.Panel(screenRt, "Dim", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Dim);

            var card = new GameObject("Card", typeof(RectTransform), typeof(Image), typeof(VerticalLayoutGroup));
            var cardRt = card.GetComponent<RectTransform>();
            cardRt.SetParent(screenRt, false);
            cardRt.anchorMin = cardRt.anchorMax = new Vector2(0.5f, 0.5f);
            cardRt.pivot = new Vector2(0.5f, 0.5f);
            cardRt.sizeDelta = new Vector2(width, height);
            UiArt.Apply(card.GetComponent<Image>(), UiArt.PausePanel, true);

            var layout = card.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(78, 78, 68, 52);
            layout.spacing = 0f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            return cardRt;
        }

        public static void Title(RectTransform card, string text)
        {
            var label = UITheme.Text(card, "Title", text, 48, InkStrong, TextAnchor.MiddleCenter, FontStyle.Bold);
            var element = label.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 72f;
            element.preferredHeight = 72f;
        }

        public static void Gap(RectTransform card, float height)
        {
            var go = new GameObject("Gap", typeof(RectTransform), typeof(LayoutElement));
            go.transform.SetParent(card, false);
            var element = go.GetComponent<LayoutElement>();
            element.minHeight = height;
            element.preferredHeight = height;
        }

        public static void Flex(RectTransform card)
        {
            var go = new GameObject("Flex", typeof(RectTransform), typeof(LayoutElement));
            go.transform.SetParent(card, false);
            var element = go.GetComponent<LayoutElement>();
            element.minHeight = 8f;
            element.flexibleHeight = 1f;
        }

        public static Button Pill(RectTransform card, string label, Action onClick, bool primary)
        {
            float width = primary ? 460f : 420f;
            float height = primary ? 110f : 100f;

            var row = new GameObject(label + "Row", typeof(RectTransform), typeof(LayoutElement));
            row.transform.SetParent(card, false);
            var rowElement = row.GetComponent<LayoutElement>();
            rowElement.minHeight = height;
            rowElement.preferredHeight = height;

            var go = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(row.transform, false);
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(width, height);

            var image = go.GetComponent<Image>();
            UiArt.Apply(image, UiArt.ButtonFace, false);

            var button = go.GetComponent<Button>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 0.96f, 0.90f, 1f);
            colors.pressedColor = new Color(0.78f, 0.68f, 0.56f, 1f);
            colors.selectedColor = colors.highlightedColor;
            colors.fadeDuration = 0.08f;
            button.colors = colors;
            button.onClick.AddListener(() => onClick?.Invoke());

            var ink = primary ? UiArt.Cream : new Color(0.90f, 0.84f, 0.74f, 1f);
            var style = primary ? FontStyle.Bold : FontStyle.Normal;
            var text = UITheme.Text(rt, "Label", label, primary ? 30 : 26, ink, TextAnchor.MiddleCenter, style);
            UITheme.Stretch(text.rectTransform, Vector2.zero, Vector2.one, new Vector2(24f, 8f), new Vector2(-24f, -8f));
            return button;
        }

        public static void Hint(RectTransform card, string text)
        {
            var label = UITheme.Text(card, "Hint", text, 16, InkSoft, TextAnchor.MiddleCenter);
            var element = label.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 28f;
            element.preferredHeight = 28f;
        }
    }
}

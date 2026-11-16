using System;
using SeriousGameBPD.Core;
using SeriousGameBPD.Presentation;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SeriousGameBPD.World
{
    /// <summary>
    /// Menú chico con Esc: continuar o volver al inicio.
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        GameObject _screen;
        bool _open;
        bool _holdingLock;
        WorldClock _clock;
        bool _clockWasPaused;

        public bool IsOpen => _open;

        public void Build(Transform parent)
        {
            var root = gameObject.GetComponent<RectTransform>();
            if (root == null)
                root = gameObject.AddComponent<RectTransform>();
            root.SetParent(parent, false);
            UITheme.Stretch(root, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            _screen = new GameObject("Screen", typeof(RectTransform));
            var screenRt = _screen.GetComponent<RectTransform>();
            screenRt.SetParent(root, false);
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

            var title = UITheme.Text(cardRt, "Title", "Pausa", 42, UiArt.Ink, TextAnchor.MiddleCenter, FontStyle.Bold);
            UITheme.Stretch(title.rectTransform, new Vector2(0.12f, 0.74f), new Vector2(0.88f, 0.90f), Vector2.zero, Vector2.zero);

            MakeButton(cardRt, "Continuar", new Vector2(0f, 36f), Close);
            MakeButton(cardRt, "Salir al inicio", new Vector2(0f, -100f), Leave);

            var hint = UITheme.Text(cardRt, "Hint", "Esc también vuelve al juego", 18, UiArt.Ink, TextAnchor.MiddleCenter);
            UITheme.Stretch(hint.rectTransform, new Vector2(0.12f, 0.06f), new Vector2(0.88f, 0.16f), Vector2.zero, Vector2.zero);

            _screen.SetActive(false);
        }

        void Update()
        {
            if (!EscapePressed())
                return;

            if (_open)
                Close();
            else
                TryOpen();
        }

        void TryOpen()
        {
            if (_open || PlayerLock.IsLocked)
                return;

            var ctx = HouseContext.Instance;
            if (ctx != null && ctx.Hud != null && ctx.Hud.OverlayOpen)
                return;

            _screen.SetActive(true);
            transform.SetAsLastSibling();
            _open = true;
            _holdingLock = true;
            PlayerLock.Push();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _clock = ctx != null ? ctx.Clock : null;
            if (_clock != null)
            {
                _clockWasPaused = _clock.Paused;
                _clock.Paused = true;
            }
        }

        public void Close()
        {
            if (!_open && !_holdingLock)
                return;

            _open = false;
            if (_screen != null)
                _screen.SetActive(false);
            Release();
        }

        void Leave()
        {
            Close();
            if (AppFlow.Instance != null)
                AppFlow.Instance.ShowBoot();
            else
            {
                PlayerLock.Reset();
                SceneFlow.LoadBoot();
            }
        }

        void OnDestroy()
        {
            Release();
        }

        void Release()
        {
            if (_clock != null)
            {
                _clock.Paused = _clockWasPaused;
                _clock = null;
            }

            if (!_holdingLock)
                return;

            _holdingLock = false;
            PlayerLock.Pop();
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
            button.onClick.AddListener(() => onClick());

            var text = UITheme.Text(rt, "Label", label, 28, UiArt.Cream, TextAnchor.MiddleCenter, FontStyle.Bold);
            UITheme.Stretch(text.rectTransform, Vector2.zero, Vector2.one, new Vector2(28f, 10f), new Vector2(-28f, -10f));
        }

        static bool EscapePressed()
        {
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            return kb != null && kb.escapeKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(KeyCode.Escape);
#endif
        }
    }
}

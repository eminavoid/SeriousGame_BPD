using SeriousGameBPD.Core;
using SeriousGameBPD.Presentation;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SeriousGameBPD.World
{
    /// <summary>
    /// Menú chico con Esc: continuar, volver al inicio o salir del juego.
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

            // Se amplía la altura a 720f para acomodar los 3 botones principales
            var card = MenuChrome.Card(root, "Screen", 680f, 720f);
            _screen = card.parent.gameObject;

            MenuChrome.Title(card, "Pausa");
            MenuChrome.Gap(card, 24f);

            // Botón 1: Continuar
            MenuChrome.Pill(card, "Continuar", Close, true);
            MenuChrome.Gap(card, 14f);

            // Botón 2: Salir al inicio (primary: true para mismo tamaño 460x110 y negrita)
            MenuChrome.Pill(card, "Salir al inicio", Leave, true);
            MenuChrome.Gap(card, 14f);

            // Botón 3: Salir del juego (primary: true para consistencia)
            MenuChrome.Pill(card, "Salir del juego", QuitGame, true);

            MenuChrome.Flex(card);
            MenuChrome.Hint(card, "Esc también vuelve al juego");

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

        void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
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
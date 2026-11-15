using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SeriousGameBPD.World
{
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] float _range = 2.5f;

        Camera _eye;
        HouseHud _hud;
        Interactable _current;
        Collider _hitCollider;
        Interactable _cachedOnCollider;
        string _shownPrompt;

        public Interactable Current => _current;

        public void Setup(Camera eye, HouseHud hud)
        {
            _eye = eye;
            _hud = hud;
        }

        void Update()
        {
            if (_eye == null || PlayerLock.IsLocked)
            {
                SetCurrent(null);
                return;
            }

            Interactable next = null;
            if (Physics.Raycast(_eye.transform.position, _eye.transform.forward, out var hit, _range, ~0, QueryTriggerInteraction.Collide))
            {
                if (hit.collider != _hitCollider)
                {
                    _hitCollider = hit.collider;
                    _cachedOnCollider = hit.collider != null
                        ? hit.collider.GetComponentInParent<Interactable>()
                        : null;
                }

                if (_cachedOnCollider != null && _cachedOnCollider.CanInteract)
                    next = _cachedOnCollider;
            }
            else if (_hitCollider != null)
            {
                _hitCollider = null;
                _cachedOnCollider = null;
            }

            SetCurrent(next);

            if (_current == null) return;

            bool pressed;
#if ENABLE_INPUT_SYSTEM
            pressed = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
#else
            pressed = Input.GetKeyDown(KeyCode.E);
#endif
            if (pressed)
                _current.TryInteract();
        }

        void SetCurrent(Interactable next)
        {
            string prompt = next != null ? next.Prompt : null;
            if (next == _current && prompt == _shownPrompt)
                return;

            _current = next;
            _shownPrompt = prompt;
            _hud?.SetPrompt(next != null ? "E — " + prompt : "");
        }
    }
}

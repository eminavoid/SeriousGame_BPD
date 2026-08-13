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
                _current = null;
                _hud?.SetPrompt("");
                return;
            }

            _current = null;
            var ray = new Ray(_eye.transform.position, _eye.transform.forward);
            if (Physics.Raycast(ray, out var hit, _range, ~0, QueryTriggerInteraction.Collide))
            {
                var interactable = hit.collider.GetComponentInParent<Interactable>();
                if (interactable != null && interactable.CanInteract)
                    _current = interactable;
            }

            _hud?.SetPrompt(_current != null ? $"E — {_current.Prompt}" : "");

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
    }
}

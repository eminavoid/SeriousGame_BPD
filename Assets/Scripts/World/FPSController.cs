using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SeriousGameBPD.World
{
    public class FPSController : MonoBehaviour
    {
        [SerializeField] float _moveSpeed = 3.2f;
        [SerializeField] float _lookSensitivity = 0.12f;
        [SerializeField] float _gravity = 18f;

        CharacterController _cc;
        Transform _head;
        float _pitch;
        float _yaw;
        float _verticalVelocity;
        Vector3 _spawn;
        public Camera Eye { get; private set; }

        public void Possess(Camera camera, Vector3 spawn, float yaw)
        {
            _cc = gameObject.GetComponent<CharacterController>();
            if (_cc == null)
                _cc = gameObject.AddComponent<CharacterController>();
            _cc.height = 1.8f;
            _cc.radius = 0.28f;
            _cc.center = new Vector3(0f, 0.9f, 0f);
            _cc.skinWidth = 0.08f;
            _cc.minMoveDistance = 0f;
            _cc.slopeLimit = 45f;
            _cc.stepOffset = 0.3f;

            _spawn = spawn;
            _pitch = 0f;
            _verticalVelocity = 0f;
            Teleport(spawn, yaw);

            var headGo = new GameObject("Head");
            _head = headGo.transform;
            _head.SetParent(transform, false);
            _head.localPosition = new Vector3(0f, 1.6f, 0f);

            Eye = camera;
            camera.tag = "MainCamera";
            camera.transform.SetParent(_head, false);
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localRotation = Quaternion.identity;
            camera.nearClipPlane = 0.08f;
            camera.fieldOfView = 70f;

            var listener = camera.GetComponent<AudioListener>();
            if (listener == null)
                camera.gameObject.AddComponent<AudioListener>();
        }

        void Update()
        {
            if (PlayerLock.IsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Vector2 look = Vector2.zero;
            Vector2 move = Vector2.zero;
#if ENABLE_INPUT_SYSTEM
            var mouse = Mouse.current;
            var kb = Keyboard.current;
            if (mouse != null)
                look = mouse.delta.ReadValue();
            if (kb != null)
            {
                if (kb.wKey.isPressed) move.y += 1f;
                if (kb.sKey.isPressed) move.y -= 1f;
                if (kb.aKey.isPressed) move.x -= 1f;
                if (kb.dKey.isPressed) move.x += 1f;
            }
#else
            look = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 12f;
            move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
#endif

            _yaw += look.x * _lookSensitivity;
            _pitch = Mathf.Clamp(_pitch - look.y * _lookSensitivity, -82f, 82f);
            transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
            if (_head != null)
                _head.localRotation = Quaternion.Euler(_pitch, 0f, 0f);

            if (_cc == null) return;

            if (transform.position.y < -8f)
            {
                Teleport(_spawn, _yaw);
                return;
            }

            if (move.sqrMagnitude > 1f)
                move.Normalize();

            var wish = transform.right * move.x + transform.forward * move.y;
            if (_cc.isGrounded)
                _verticalVelocity = -2f;
            else
                _verticalVelocity = Mathf.Max(_verticalVelocity - _gravity * Time.deltaTime, -24f);

            _cc.Move((wish * _moveSpeed + Vector3.up * _verticalVelocity) * Time.deltaTime);
        }

        void Teleport(Vector3 worldPos, float yaw)
        {
            _yaw = yaw;
            if (_cc != null)
                _cc.enabled = false;
            transform.SetPositionAndRotation(worldPos, Quaternion.Euler(0f, yaw, 0f));
            Physics.SyncTransforms();
            if (_cc != null)
                _cc.enabled = true;
            _verticalVelocity = 0f;
        }
    }
}

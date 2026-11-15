using SeriousGameBPD.Data;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SeriousGameBPD.World
{
    public class PhoneController : MonoBehaviour
    {
        Interactable _interactable;
        AudioSource _audio;
        Vector3 _rest;
        bool _buzzing;
        float _buzzTimer;
        DayEvent _pending;
        AudioClip _buzzClip;
        MeshRenderer _renderer;
        MaterialPropertyBlock _block;

        static readonly int NotifyId = Shader.PropertyToID("_Notify");

        public Interactable Interactable => _interactable;
        public bool IsBuzzing => _buzzing;
        public DayEvent PendingEvent => _pending;

        public void Setup(Interactable interactable)
        {
            _interactable = interactable;
            _rest = transform.localPosition;
            _audio = gameObject.AddComponent<AudioSource>();
            _audio.playOnAwake = false;
            _audio.spatialBlend = 1f;
            _audio.minDistance = 0.4f;
            _audio.maxDistance = 8f;
            _buzzClip = ProceduralAudio.Beep(180f, 0.18f, 0.4f);
            _audio.clip = _buzzClip;
            SetNotify(0f);
        }

        public void StartBuzzing(DayEvent evt)
        {
            _pending = evt;
            _buzzing = true;
            if (_interactable != null)
                _interactable.Prompt = "Atender el celular (vibra)";
            SetNotify(1f);
        }

        public void StopBuzzing()
        {
            _buzzing = false;
            _pending = null;
            transform.localPosition = _rest;
            if (_interactable != null)
                _interactable.Prompt = "Mirar el celular";
            SetNotify(0f);
            StopRumble();
        }

        void SetNotify(float value)
        {
            if (_renderer == null)
                _renderer = GetComponent<MeshRenderer>();
            if (_renderer == null)
                return;
            if (_block == null)
                _block = new MaterialPropertyBlock();

            var mats = _renderer.sharedMaterials;
            for (var i = 0; i < mats.Length; i++)
            {
                var mat = mats[i];
                if (mat == null || !mat.HasProperty(NotifyId))
                    continue;
                _renderer.GetPropertyBlock(_block, i);
                _block.SetFloat(NotifyId, value);
                _renderer.SetPropertyBlock(_block, i);
            }
        }

        void Update()
        {
            if (!_buzzing) return;

            _buzzTimer -= Time.deltaTime;
            float shake = 0.012f + (HouseContext.Instance != null &&
                                    HouseContext.Instance.SessionAffectHigh ? 0.02f : 0f);
            transform.localPosition = _rest + new Vector3(
                Mathf.Sin(Time.time * 55f) * shake,
                Mathf.Cos(Time.time * 70f) * shake,
                0f);

            if (_buzzTimer <= 0f)
            {
                _buzzTimer = 1.15f;
                if (_audio != null && _buzzClip != null)
                    _audio.Play();
#if ENABLE_INPUT_SYSTEM
                var pad = Gamepad.current;
                if (pad != null)
                    pad.SetMotorSpeeds(0.25f, 0.55f);
#endif
            }
            else if (_buzzTimer < 0.85f)
            {
                StopRumble();
            }
        }

        static void StopRumble()
        {
#if ENABLE_INPUT_SYSTEM
            var pad = Gamepad.current;
            if (pad != null)
                pad.SetMotorSpeeds(0f, 0f);
#endif
        }

        void OnDisable() => StopRumble();
    }
}

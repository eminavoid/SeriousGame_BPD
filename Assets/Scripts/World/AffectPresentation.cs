using SeriousGameBPD.Affect;
using SeriousGameBPD.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace SeriousGameBPD.World
{
    public class AffectPresentation : MonoBehaviour
    {
        Camera _eye;
        AffectState _state;
        Image _vignette;
        AudioSource _heart;
        Light _sun;
        float _baseFov = 70f;
        float _baseLight = 1.6f;
        float _wave;
        float _heartTimer;

        public void Setup(Camera eye, AffectState state, Transform hudParent, Light sun)
        {
            _eye = eye;
            _state = state;
            _sun = sun;
            if (_eye != null)
                _baseFov = _eye.fieldOfView;
            if (_sun != null)
                _baseLight = _sun.intensity;

            var vg = UITheme.Panel(hudParent, "Vignette",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
                new Color(0.12f, 0.04f, 0.06f, 0f));
            _vignette = vg.GetComponent<Image>();
            _vignette.raycastTarget = false;
            vg.SetAsFirstSibling();

            _heart = gameObject.AddComponent<AudioSource>();
            _heart.playOnAwake = false;
            _heart.spatialBlend = 0f;
            _heart.clip = ProceduralAudio.Heartbeat();
            _heart.volume = 0.35f;
        }

        public void PulseWave(float amount = 1f)
        {
            _wave = Mathf.Max(_wave, amount);
        }

        void Update()
        {
            if (_eye == null || _state == null) return;

            float impl = _state.Impulsivity / 100f;
            float dys = _state.IsDysregulated ? 1f : impl;
            _wave = Mathf.MoveTowards(_wave, 0f, Time.deltaTime * 0.35f);

            float fovTarget = Mathf.Lerp(_baseFov, 52f, impl * 0.75f + _wave * 0.25f);
            _eye.fieldOfView = Mathf.Lerp(_eye.fieldOfView, fovTarget, Time.deltaTime * 3f);

            float shake = (impl > 0.55f || _wave > 0.2f) ? (impl * 0.04f + _wave * 0.08f) : 0f;
            var head = _eye.transform.parent;
            if (head != null)
            {
                var basePos = new Vector3(0f, 1.6f, 0f);
                if (shake > 0f && !PlayerLock.IsLocked)
                {
                    float t = Time.unscaledTime * 18f;
                    head.localPosition = basePos + new Vector3(
                        Mathf.Sin(t) * shake,
                        Mathf.Cos(t * 1.3f) * shake * 0.5f,
                        0f);
                }
                else
                {
                    head.localPosition = basePos;
                }
            }

            if (_vignette != null)
            {
                float a = Mathf.Clamp01(dys * 0.28f + _wave * 0.35f);
                _vignette.color = new Color(0.12f, 0.04f, 0.06f, a);
            }

            if (_sun != null)
            {
                var clock = HouseContext.Instance != null ? HouseContext.Instance.Clock : null;
                float night = 0f;
                if (clock != null && clock.Minutes >= 20f * 60f)
                    night = Mathf.InverseLerp(20f * 60f, 22.5f * 60f, clock.Minutes);
                _sun.intensity = Mathf.Lerp(_baseLight, 0.35f, night);
                _sun.color = Color.Lerp(new Color(1f, 0.96f, 0.9f), new Color(0.45f, 0.5f, 0.7f), night);
            }

            if (_state.IsDysregulated || _wave > 0.4f)
            {
                _heartTimer -= Time.unscaledDeltaTime;
                if (_heartTimer <= 0f && _heart != null && _heart.clip != null)
                {
                    _heart.pitch = 0.95f + impl * 0.25f;
                    _heart.Play();
                    _heartTimer = Mathf.Lerp(1.15f, 0.55f, impl);
                }
            }
        }
    }
}

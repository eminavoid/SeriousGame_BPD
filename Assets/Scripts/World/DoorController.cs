using UnityEngine;

namespace SeriousGameBPD.World
{
    public class DoorController : MonoBehaviour
    {
        Interactable _interactable;
        AudioSource _audio;
        Vector3 _rest;
        bool _knocking;
        float _knockTimer;
        AudioClip _knockClip;

        public Interactable Interactable => _interactable;
        public bool IsKnocking => _knocking;

        public void Setup(Interactable interactable)
        {
            _interactable = interactable;
            _rest = transform.localPosition;
            _audio = gameObject.AddComponent<AudioSource>();
            _audio.playOnAwake = false;
            _audio.spatialBlend = 1f;
            _audio.minDistance = 0.8f;
            _audio.maxDistance = 12f;
            _knockClip = ProceduralAudio.Knock();
            _audio.clip = _knockClip;
        }

        public void StartKnocking()
        {
            _knocking = true;
            if (_interactable != null)
                _interactable.Prompt = "Atender la puerta (golpean)";
        }

        public void StopKnocking()
        {
            _knocking = false;
            transform.localPosition = _rest;
            if (_interactable != null)
                _interactable.Prompt = "Mirar la puerta";
        }

        void Update()
        {
            if (!_knocking) return;

            _knockTimer -= Time.deltaTime;
            transform.localPosition = _rest + Vector3.right * (Mathf.Sin(Time.time * 40f) * 0.008f);

            if (_knockTimer <= 0f)
            {
                _knockTimer = 1.8f;
                if (_audio != null && _knockClip != null)
                    _audio.Play();
            }
        }
    }
}

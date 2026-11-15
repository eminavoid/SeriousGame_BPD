using System.Collections.Generic;
using SeriousGameBPD.Core;
using SeriousGameBPD.Data;
using UnityEngine;

namespace SeriousGameBPD.World
{
    public class DayDirector : MonoBehaviour
    {
        class Beat
        {
            public DayEvent Event;
            public float WaitSeconds;
            public float MaxDeferSeconds;
            public float ClockMinutes;
            public bool CountsForDay = true;
        }

        HouseContext _ctx;
        readonly List<Beat> _beats = new List<Beat>();
        readonly HashSet<string> _done = new HashSet<string>();
        int _index = -1;
        int _clockMinute = int.MinValue;
        float _wait;
        float _pendingTime;
        float _deferTick;
        Beat _active;
        bool _playing;
        bool _dayEnded;

        public void Setup(HouseContext ctx)
        {
            _ctx = ctx;
            var plan = GameSession.Instance.DayPlan;
            if (plan == null || plan.events == null)
                return;

            DayEvent Find(string id)
            {
                foreach (var e in plan.events)
                    if (e != null && e.eventId == id)
                        return e;
                return null;
            }

            _beats.Clear();
            _beats.Add(new Beat { Event = Find("manana_espejo"), WaitSeconds = 12f, MaxDeferSeconds = 20f, ClockMinutes = 7.6f * 60f });
            _beats.Add(new Beat { Event = Find("manana_mensaje"), WaitSeconds = 9f, MaxDeferSeconds = 24f, ClockMinutes = 8.2f * 60f });
            _beats.Add(new Beat { Event = Find("mediodia_cafe"), WaitSeconds = 9f, MaxDeferSeconds = 22f, ClockMinutes = 12.25f * 60f });
            _beats.Add(new Beat { Event = ContentFactory.BuildDoorKnockEvent(), WaitSeconds = 6f, MaxDeferSeconds = 16f, ClockMinutes = 13.1f * 60f, CountsForDay = false });
            _beats.Add(new Beat { Event = Find("tarde_urgencia"), WaitSeconds = 8f, MaxDeferSeconds = 14f, ClockMinutes = 16.1f * 60f });
            _beats.Add(new Beat { Event = Find("tarde_feedback"), WaitSeconds = 7f, MaxDeferSeconds = 20f, ClockMinutes = 17.45f * 60f });
            _beats.Add(new Beat { Event = Find("tarde_pareja"), WaitSeconds = 8f, MaxDeferSeconds = 18f, ClockMinutes = 19.7f * 60f });
            _beats.Add(new Beat { Event = Find("noche_cierre"), WaitSeconds = 12f, MaxDeferSeconds = 18f, ClockMinutes = 21.2f * 60f });

            foreach (var interactable in FindObjectsByType<Interactable>(FindObjectsSortMode.None))
                interactable.Interacted += OnInteracted;

            Advance();
        }

        void OnDestroy()
        {
            foreach (var interactable in FindObjectsByType<Interactable>(FindObjectsSortMode.None))
                interactable.Interacted -= OnInteracted;
        }

        void Update()
        {
            if (_dayEnded || _playing || PlayerLock.IsLocked) return;
            if (_ctx == null || _ctx.Clock == null) return;

            int minute = Mathf.Clamp(Mathf.RoundToInt(_ctx.Clock.Minutes), 0, 24 * 60 - 1);
            if (minute != _clockMinute)
            {
                _clockMinute = minute;
                _ctx.Hud?.SetClock(_ctx.Clock.DisplayBlock, _ctx.Clock.DisplayTime);
            }

            if (_active == null) return;

            if (_wait > 0f)
            {
                _wait -= Time.deltaTime;
                if (_wait <= 0f)
                    EnterPending();
                return;
            }

            _pendingTime += Time.deltaTime;
            _deferTick += Time.deltaTime;
            if (_active.Event != null && _active.Event.canDefer && _deferTick >= 4f)
            {
                _deferTick = 0f;
                GameSession.Instance.Affect.Apply(_active.Event.deferDelta);
                _ctx.Thoughts?.Show(DeferThought(_active.Event));
            }

            if (_pendingTime >= _active.MaxDeferSeconds)
                PlayActive();
        }

        void Advance()
        {
            _index++;
            _active = null;
            StopSignals();

            if (_index >= _beats.Count)
            {
                EndDay();
                return;
            }

            _active = _beats[_index];
            _wait = _active.WaitSeconds;
            _pendingTime = 0f;
            _deferTick = 0f;
            if (_active.Event != null)
                _ctx.Clock.JumpTo(_active.ClockMinutes);
        }

        void EnterPending()
        {
            if (_active == null || _active.Event == null)
            {
                Advance();
                return;
            }

            var evt = _active.Event;
            _pendingTime = 0f;
            _deferTick = 0f;

            if (evt.worldAnchorId == WorldAnchorIds.Phone)
            {
                _ctx.Phone?.StartBuzzing(evt);
                _ctx.Thoughts?.Show("El celular vibra. El pecho responde antes que la cabeza.");
            }
            else if (evt.worldAnchorId == WorldAnchorIds.Door)
            {
                _ctx.Door?.StartKnocking();
                _ctx.Thoughts?.Show("Golpean. ¿Quién es? ¿Por qué ahora?");
            }
            else if (evt.worldAnchorId == WorldAnchorIds.Laptop)
            {
                _ctx.Thoughts?.Show("Una notificación en el notebook. 'esto podría mejorar'.");
                if (_ctx.Laptop != null)
                    _ctx.Laptop.Prompt = "Abrir el mail / comentario";
            }
            else if (evt.worldAnchorId == WorldAnchorIds.Couch)
            {
                _ctx.Thoughts?.Show("Tu pareja cuenta el día. Nombró a un ex. El pecho ya armó una historia.");
            }
            else if (evt.worldAnchorId == WorldAnchorIds.Body)
            {
                _ctx.AffectFx?.PulseWave(1f);
                _ctx.Thoughts?.Show("Una ola sube sin una causa clara. El cuerpo pide alivio YA.");
                PlayActive();
                return;
            }
            else if (evt.worldAnchorId == WorldAnchorIds.Mirror)
            {
                _ctx.Thoughts?.Show("El baño está ahí. El espejo también. ¿Quién mirás hoy?");
            }
            else if (evt.worldAnchorId == WorldAnchorIds.Bed)
            {
                _ctx.Thoughts?.Show("La casa se apaga. El día pide cierre — o rumia.");
            }
        }

        void PlayActive()
        {
            if (_playing || _active == null || _active.Event == null) return;
            _playing = true;
            StopSignals();
            PlayerLock.Push();
            _ctx.Clock.Paused = true;

            var evt = _active.Event;
            if (evt.worldAnchorId == WorldAnchorIds.Body)
                _ctx.AffectFx?.PulseWave(1f);

            if (evt.worldAnchorId == WorldAnchorIds.Phone)
                _ctx.Hud.PhoneChat.Play(evt, OnBeatFinished);
            else
                _ctx.Hud.Events.Play(evt, OnBeatFinished);
        }

        void OnBeatFinished()
        {
            if (_active != null && _active.Event != null)
                _done.Add(_active.Event.eventId);

            _playing = false;
            PlayerLock.Pop();
            _ctx.Clock.Paused = false;
            Advance();
        }

        void OnInteracted(Interactable interactable)
        {
            if (_playing || PlayerLock.IsLocked) return;

            if (_active != null && _active.Event != null &&
                interactable.AnchorId == _active.Event.worldAnchorId &&
                _wait <= 0f)
            {
                PlayActive();
                return;
            }

            if (_active != null && _active.Event != null &&
                interactable.AnchorId == _active.Event.worldAnchorId &&
                _wait > 0f && _active.Event.triggerType == EventTriggerType.Pull)
            {
                _wait = 0f;
                PlayActive();
                return;
            }

            PlayAmbient(interactable);
        }

        void PlayAmbient(Interactable interactable)
        {
            if (!string.IsNullOrEmpty(interactable.IdleThought))
            {
                _ctx.Thoughts?.Show(interactable.IdleThought);
                return;
            }

            switch (interactable.AnchorId)
            {
                case WorldAnchorIds.Sink:
                    _ctx.Thoughts?.Show("Agua fría. TIPP vive acá — si querés, usá la herramienta 2.");
                    _ctx.Hud?.UseFreeSkill(SkillId.Tipp);
                    break;
                case WorldAnchorIds.Phone:
                    _ctx.Thoughts?.Show("La pantalla está quieta. Por ahora.");
                    break;
                case WorldAnchorIds.Door:
                    _ctx.Thoughts?.Show("Nadie. Solo la madera y tu alarma.");
                    break;
                case WorldAnchorIds.Bed:
                    _ctx.Thoughts?.Show("Todavía no. El día no terminó.");
                    break;
                case WorldAnchorIds.Laptop:
                    _ctx.Thoughts?.Show("La tapa cerrada. El trabajo puede esperar un poco.");
                    break;
                default:
                    _ctx.Thoughts?.Show("La casa está quieta. Adentro, no tanto.");
                    break;
            }
        }

        static string DeferThought(DayEvent evt)
        {
            if (evt.worldAnchorId == WorldAnchorIds.Phone)
                return "Sigue ahí. Evitarlo no apaga la alarma: la sube.";
            if (evt.worldAnchorId == WorldAnchorIds.Door)
                return "Los golpes siguen. El pecho también.";
            if (evt.worldAnchorId == WorldAnchorIds.Laptop)
                return "El comentario no se borra porque no lo abras.";
            if (evt.worldAnchorId == WorldAnchorIds.Couch)
                return "La frase sigue en la mesa. Irte al otro cuarto no apaga la historia.";
            return "Lo que no mirás igual está en el cuerpo.";
        }

        void StopSignals()
        {
            _ctx.Phone?.StopBuzzing();
            _ctx.Door?.StopKnocking();
        }

        void EndDay()
        {
            if (_dayEnded) return;
            _dayEnded = true;
            _ctx.Hud?.CompleteDay();
        }
    }
}

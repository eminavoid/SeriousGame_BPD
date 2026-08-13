using System;
using SeriousGameBPD.Data;
using UnityEngine;

namespace SeriousGameBPD.World
{
    public class WorldClock : MonoBehaviour
    {
        public const float StartMinutes = 7.5f * 60f;
        public const float EndMinutes = 23f * 60f;

        [SerializeField] float _minutesPerRealSecond = 2.2f;

        public float Minutes { get; private set; } = StartMinutes;
        public bool Paused { get; set; }

        public DayBlock Block => BlockAt(Minutes);
        public event Action Changed;

        public string DisplayTime
        {
            get
            {
                int total = Mathf.Clamp(Mathf.RoundToInt(Minutes), 0, 24 * 60 - 1);
                int h = total / 60;
                int m = total % 60;
                return $"{h:00}:{m:00}";
            }
        }

        public string DisplayBlock => Block switch
        {
            DayBlock.Manana => "Mañana",
            DayBlock.Mediodia => "Mediodía",
            DayBlock.Tarde => "Tarde",
            DayBlock.Noche => "Noche",
            _ => ""
        };

        void Update()
        {
            if (Paused || PlayerLock.IsLocked) return;
            Minutes = Mathf.Min(EndMinutes, Minutes + _minutesPerRealSecond * Time.deltaTime);
            Changed?.Invoke();
        }

        public void JumpTo(float minutes)
        {
            Minutes = Mathf.Clamp(minutes, StartMinutes, EndMinutes);
            Changed?.Invoke();
        }

        public static DayBlock BlockAt(float minutes)
        {
            if (minutes < 12f * 60f) return DayBlock.Manana;
            if (minutes < 15f * 60f) return DayBlock.Mediodia;
            if (minutes < 20f * 60f) return DayBlock.Tarde;
            return DayBlock.Noche;
        }
    }
}

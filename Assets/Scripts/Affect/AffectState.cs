using System;
using UnityEngine;

namespace SeriousGameBPD.Affect
{
    /// <summary>
    /// Runtime emotional meters for the day simulation.
    /// Affect: -100..100, Impulsivity: 0..100, Clarity: 0..100.
    /// </summary>
    public class AffectState
    {
        public const float AffectMin = -100f;
        public const float AffectMax = 100f;
        public const float MeterMin = 0f;
        public const float MeterMax = 100f;

        public float Affect { get; private set; }
        public float Impulsivity { get; private set; }
        public float Clarity { get; private set; }

        public event Action Changed;

        public AffectState(float affect = 10f, float impulsivity = 25f, float clarity = 55f)
        {
            Affect = Mathf.Clamp(affect, AffectMin, AffectMax);
            Impulsivity = Mathf.Clamp(impulsivity, MeterMin, MeterMax);
            Clarity = Mathf.Clamp(clarity, MeterMin, MeterMax);
        }

        public void Reset(float affect = 10f, float impulsivity = 25f, float clarity = 55f)
        {
            Affect = Mathf.Clamp(affect, AffectMin, AffectMax);
            Impulsivity = Mathf.Clamp(impulsivity, MeterMin, MeterMax);
            Clarity = Mathf.Clamp(clarity, MeterMin, MeterMax);
            Changed?.Invoke();
        }

        public void Apply(Data.AffectDelta delta)
        {
            Affect = Mathf.Clamp(Affect + delta.affect, AffectMin, AffectMax);
            Impulsivity = Mathf.Clamp(Impulsivity + delta.impulsivity, MeterMin, MeterMax);
            Clarity = Mathf.Clamp(Clarity + delta.clarity, MeterMin, MeterMax);
            Changed?.Invoke();
        }

        public bool IsHighImpulsivity => Impulsivity >= 65f;
        public bool IsLowClarity => Clarity <= 35f;
        public bool IsDysregulated => Impulsivity >= 70f || Mathf.Abs(Affect) >= 70f;

        public float NormalizedAffect01 => (Affect - AffectMin) / (AffectMax - AffectMin);
    }
}

using System;
using UnityEngine;

namespace SeriousGameBPD.Data
{
    [Serializable]
    public struct AffectDelta
    {
        [Range(-100, 100)] public float affect;
        [Range(-100, 100)] public float impulsivity;
        [Range(-100, 100)] public float clarity;

        public AffectDelta(float affect, float impulsivity, float clarity)
        {
            this.affect = affect;
            this.impulsivity = impulsivity;
            this.clarity = clarity;
        }

        public static AffectDelta Zero => new AffectDelta(0, 0, 0);
    }
}

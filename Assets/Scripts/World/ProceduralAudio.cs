using UnityEngine;

namespace SeriousGameBPD.World
{
    public static class ProceduralAudio
    {
        public static AudioClip Beep(float freq, float duration, float volume = 0.28f)
        {
            const int hz = 22050;
            int n = Mathf.Max(1, Mathf.RoundToInt(hz * duration));
            var clip = AudioClip.Create("beep", n, 1, hz, false);
            var data = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)hz;
                float env = 1f - t / duration;
                data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * env * volume;
            }

            clip.SetData(data, 0);
            return clip;
        }

        public static AudioClip Knock()
        {
            const int hz = 22050;
            float duration = 0.22f;
            int n = Mathf.RoundToInt(hz * duration);
            var clip = AudioClip.Create("knock", n, 1, hz, false);
            var data = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)hz;
                float env = Mathf.Exp(-t * 22f);
                float noise = (Random.value * 2f - 1f) * 0.4f;
                data[i] = (Mathf.Sin(2f * Mathf.PI * 90f * t) + noise) * env * 0.55f;
            }

            clip.SetData(data, 0);
            return clip;
        }

        public static AudioClip Heartbeat()
        {
            const int hz = 22050;
            float duration = 0.35f;
            int n = Mathf.RoundToInt(hz * duration);
            var clip = AudioClip.Create("heart", n, 1, hz, false);
            var data = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)hz;
                float env = Mathf.Exp(-t * 14f);
                data[i] = Mathf.Sin(2f * Mathf.PI * 55f * t) * env * 0.4f;
            }

            clip.SetData(data, 0);
            return clip;
        }
    }
}

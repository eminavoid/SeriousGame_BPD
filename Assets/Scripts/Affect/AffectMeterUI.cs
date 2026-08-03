using SeriousGameBPD.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace SeriousGameBPD.Affect
{
    public class AffectMeterUI : MonoBehaviour
    {
        Text _affectLabel;
        Text _impulsivityLabel;
        Text _clarityLabel;
        Image _affectFill;
        Image _impulsivityFill;
        Image _clarityFill;
        Image _moodWash;
        AffectState _state;

        public void Build(Transform parent, AffectState state)
        {
            _state = state;
            var hud = UITheme.Panel(parent, "AffectHUD",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(24, -110), new Vector2(-24, -16),
                UITheme.BgPanel);

            _moodWash = hud.GetComponent<Image>();

            CreateMeter(hud, "Afecto", 0, out _affectLabel, out _affectFill, UITheme.AccentWarm);
            CreateMeter(hud, "Impulsividad", 1, out _impulsivityLabel, out _impulsivityFill, UITheme.Danger);
            CreateMeter(hud, "Claridad", 2, out _clarityLabel, out _clarityFill, UITheme.Good);

            state.Changed += Refresh;
            Refresh();
        }

        void OnDestroy()
        {
            if (_state != null)
                _state.Changed -= Refresh;
        }

        static void CreateMeter(RectTransform parent, string title, int index, out Text label, out Image fill, Color fillColor)
        {
            float x0 = 20 + index * 620;
            var block = new GameObject(title + "Meter", typeof(RectTransform));
            var rt = block.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            UITheme.Stretch(rt, new Vector2(0, 0), new Vector2(0, 1), new Vector2(x0, 12), new Vector2(x0 + 580, -12));

            label = UITheme.Text(rt, "Label", title, 20, UITheme.TextMuted, TextAnchor.UpperLeft);
            UITheme.Stretch(label.rectTransform, new Vector2(0, 0.55f), new Vector2(1, 1), Vector2.zero, Vector2.zero);

            var trackGo = new GameObject("Track", typeof(RectTransform), typeof(Image));
            var trackRt = trackGo.GetComponent<RectTransform>();
            trackRt.SetParent(rt, false);
            UITheme.Stretch(trackRt, new Vector2(0, 0), new Vector2(1, 0.5f), new Vector2(0, 4), new Vector2(0, 0));
            trackGo.GetComponent<Image>().color = UITheme.MeterTrack;

            var fillGo = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            var fillRt = fillGo.GetComponent<RectTransform>();
            fillRt.SetParent(trackRt, false);
            fillRt.anchorMin = Vector2.zero;
            fillRt.anchorMax = new Vector2(0.5f, 1);
            fillRt.offsetMin = Vector2.zero;
            fillRt.offsetMax = Vector2.zero;
            fill = fillGo.GetComponent<Image>();
            fill.color = fillColor;
        }

        void Refresh()
        {
            if (_state == null) return;

            _affectLabel.text = $"Afecto: {_state.Affect:0}";
            _impulsivityLabel.text = $"Impulsividad: {_state.Impulsivity:0}";
            _clarityLabel.text = $"Claridad: {_state.Clarity:0}";

            SetFill(_affectFill, _state.NormalizedAffect01);
            SetFill(_impulsivityFill, _state.Impulsivity / 100f);
            SetFill(_clarityFill, _state.Clarity / 100f);

            if (_moodWash != null)
            {
                if (_state.IsDysregulated)
                    _moodWash.color = Color.Lerp(UITheme.BgPanel, UITheme.Danger, 0.25f);
                else if (_state.Clarity >= 70f)
                    _moodWash.color = Color.Lerp(UITheme.BgPanel, UITheme.Good, 0.12f);
                else
                    _moodWash.color = UITheme.BgPanel;
            }
        }

        static void SetFill(Image fill, float amount01)
        {
            var rt = fill.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(Mathf.Clamp01(amount01), 1f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}

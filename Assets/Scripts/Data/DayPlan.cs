using System.Collections.Generic;
using UnityEngine;

namespace SeriousGameBPD.Data
{
    [CreateAssetMenu(menuName = "SeriousGame BPD/Day Plan", fileName = "DayPlan")]
    public class DayPlan : ScriptableObject
    {
        public string dayTitle = "Un día";
        [TextArea(2, 6)] public string dayIntro;
        public List<DayEvent> events = new List<DayEvent>();
        public List<GlossaryEntry> glossary = new List<GlossaryEntry>();
        public List<SkillDef> skills = new List<SkillDef>();
    }
}

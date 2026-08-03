using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeriousGameBPD.Data
{
    public enum DayBlock
    {
        Manana,
        Mediodia,
        Tarde,
        Noche
    }

    [Serializable]
    public class EventOption
    {
        public string optionId;
        [TextArea(1, 4)] public string label;
        public AffectDelta delta;
        public bool requiresSkillOffer;
        public SkillId suggestedSkill = SkillId.None;
        [TextArea(1, 4)] public string outcomeWithoutSkill;
        [TextArea(1, 4)] public string outcomeWithSkill;
        public AffectDelta skillBonusDelta;
    }

    [CreateAssetMenu(menuName = "SeriousGame BPD/Day Event", fileName = "DayEvent")]
    public class DayEvent : ScriptableObject
    {
        public string eventId;
        public DayBlock block;
        public string title;
        [TextArea(3, 12)] public string body;
        public List<EducationalTag> tags = new List<EducationalTag>();
        public AffectDelta enterDelta;
        public List<EventOption> options = new List<EventOption>();
        public SkillId recommendedSkill = SkillId.None;
    }
}

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

    public enum EventTriggerType
    {
        Push,
        Pull
    }

    public static class WorldAnchorIds
    {
        public const string Mirror = "mirror";
        public const string Phone = "phone";
        public const string Door = "door";
        public const string Laptop = "laptop";
        public const string Bed = "bed";
        public const string Body = "body";
        public const string Sink = "sink";
        public const string Window = "window";
        public const string Fridge = "fridge";
        public const string Coffee = "coffee";
        public const string Couch = "couch";
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
        public EventTriggerType triggerType = EventTriggerType.Push;
        public string worldAnchorId;
        public bool canDefer;
        public AffectDelta deferDelta;
    }
}

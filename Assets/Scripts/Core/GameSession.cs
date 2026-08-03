using System.Collections.Generic;
using SeriousGameBPD.Affect;
using SeriousGameBPD.Data;
using UnityEngine;

namespace SeriousGameBPD.Core
{
    /// <summary>
    /// Persists across Boot / Day / Debrief scene flow.
    /// </summary>
    public class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; }

        public AffectState Affect { get; private set; }
        public DayPlan DayPlan { get; private set; }
        public HashSet<EducationalTag> ExperiencedTags { get; } = new HashSet<EducationalTag>();
        public HashSet<SkillId> UsedSkills { get; } = new HashSet<SkillId>();
        public List<string> OutcomeLog { get; } = new List<string>();
        public bool AcceptedWarning { get; set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Affect = new AffectState();
        }

        public void BeginNewDay(DayPlan plan)
        {
            DayPlan = plan;
            Affect.Reset();
            ExperiencedTags.Clear();
            UsedSkills.Clear();
            OutcomeLog.Clear();
        }

        public void RegisterTags(IEnumerable<EducationalTag> tags)
        {
            if (tags == null) return;
            foreach (var tag in tags)
                ExperiencedTags.Add(tag);
        }

        public void RegisterSkillUse(SkillId id)
        {
            if (id != SkillId.None)
                UsedSkills.Add(id);
        }

        public void LogOutcome(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
                OutcomeLog.Add(text);
        }
    }
}

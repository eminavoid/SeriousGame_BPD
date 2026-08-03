using UnityEngine;

namespace SeriousGameBPD.Data
{
    [CreateAssetMenu(menuName = "SeriousGame BPD/Skill Def", fileName = "SkillDef")]
    public class SkillDef : ScriptableObject
    {
        public SkillId id;
        public string displayName;
        [TextArea(2, 6)] public string shortDescription;
        [TextArea(2, 8)] public string howToUse;
        public AffectDelta successDelta;
    }
}

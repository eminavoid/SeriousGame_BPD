using System.Collections.Generic;
using SeriousGameBPD.Data;
using UnityEngine;

namespace SeriousGameBPD.Skills
{
    public class SkillController
    {
        readonly Dictionary<SkillId, SkillBase> _skills = new Dictionary<SkillId, SkillBase>();

        public SkillController(IEnumerable<SkillDef> defs)
        {
            if (defs == null) return;
            foreach (var def in defs)
            {
                if (def == null) continue;
                SkillBase skill = def.id switch
                {
                    SkillId.Stop => new StopSkill(def),
                    SkillId.Tipp => new TippSkill(def),
                    SkillId.OppositeAction => new OppositeActionSkill(def),
                    _ => null
                };
                if (skill != null)
                    _skills[def.id] = skill;
            }
        }

        public bool TryGet(SkillId id, out SkillBase skill) => _skills.TryGetValue(id, out skill);

        public SkillDef GetDef(SkillId id)
        {
            return TryGet(id, out var s) ? s.Def : null;
        }
    }
}

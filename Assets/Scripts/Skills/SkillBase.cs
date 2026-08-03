using System;
using SeriousGameBPD.Core;
using SeriousGameBPD.Data;
using UnityEngine;

namespace SeriousGameBPD.Skills
{
    public abstract class SkillBase
    {
        public SkillDef Def { get; }
        public SkillId Id => Def != null ? Def.id : SkillId.None;

        protected SkillBase(SkillDef def) => Def = def;

        public abstract void Present(Transform uiParent, Action<bool> onComplete);

        protected void ApplySuccess()
        {
            if (Def == null || GameSession.Instance == null) return;
            GameSession.Instance.Affect.Apply(Def.successDelta);
            GameSession.Instance.RegisterSkillUse(Id);
        }
    }
}

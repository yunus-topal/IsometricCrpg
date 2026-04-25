using System.Collections.Generic;

namespace DataModels
{
    public abstract class SkillExecutable
    {
        public SkillBase Base { get; private set; }

        public SkillExecutable(SkillBase skillBase)
        {
            Base = skillBase;
        }
        
        // TODO:
        public abstract void Execute(Combatant user, List<Combatant> targets);
    }
}
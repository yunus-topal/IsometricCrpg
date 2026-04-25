using System.Collections.Generic;
using DataModels;

namespace Logic
{
    public static class EnemyAIStub
    {
        // TODO: check neutral details and use correct logic. 
        // for beginning, a random skill selection would be fine.
        public static (SkillExecutable, List<Combatant>) SelectSkillAndTargets(Combatant enemy, List<Combatant> allCombatants)
        {
            return (null, null);
        }
    }
}
using System;
using System.Collections.Generic;
using DataModels;
using UnityEngine;

namespace InGameManagers
{
    public class UIManager : MonoBehaviour
    {
        // TODO: call onSkillConfirmed when player confirms their action in the UI, passing the chosen skill and targets.
        public void ShowPlayerTurnUI(Combatant activeCombatant, List<Combatant> allCombatants,
            Action<SkillBase, List<Combatant>> onSkillConfirmed)
        {
            
        }

        public void HideCombatUI()
        {
            
        }
        
    }
}
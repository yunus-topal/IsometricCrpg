using DataModels;
using UnityEngine;

namespace Utils
{
    public static class CombatUtils
    {
        /// <summary>
        /// Base formula: Agility + random noise so equal-agility units don't
        /// always act in the same order.
        /// </summary>
        public static float CalculateInitiative(Combatant combatant)
        {
            float agility  = combatant.Attributes.Agility;
            float noise    = Random.Range(0f, 3f);   // small tiebreaker
            return agility + noise;

            // Future hooks (commented out until needed):
            // + GetEquipmentBonus(combatant)
            // + GetStatusBonus(combatant)
        }
    }
}
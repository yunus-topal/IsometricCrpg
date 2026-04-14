using DataModels;
using UnityEngine;

namespace InGameManagers
{
    /// <summary>
    /// Handle combat loop, turn order, and combat state (e.g., player turn, enemy turn, victory, defeat).
    /// Provide basic methods for dealing damage, healing etc. Skills should not directly modify character stats.
    /// This class should use StatusEffectManager to apply status effects and handle their interactions during combat.
    /// </summary>
    public class CombatManager : MonoBehaviour
    {
        // for testing purposes only. should be removed later and replaced with actual combat triggering logic.
        [SerializeField] private CharacterSo[] PlayerCharacters;
        [SerializeField] private CharacterSo[] EnemyCharacters;

        public void StartCombat()
        {
            
            
            
        }
    }
}

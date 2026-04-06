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
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}

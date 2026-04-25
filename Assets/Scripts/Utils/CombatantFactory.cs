using System.Collections.Generic;
using DataModels;
using InGameManagers;

namespace Utils
{
    /// <summary>
    /// Resolves allegiance for every character entering combat.
    ///
    /// The factory is the ONLY place that knows the rule:
    ///   "if GameManager recognises you as a player character → Player,
    ///    otherwise → Enemy"
    ///
    /// CharData and RuntimeCharData stay allegiance-free.
    /// </summary>
    public static class CombatantFactory
    {
        public static List<Combatant> BuildCombatants(
            List<RuntimeCharData> nearby,
            GameManager gameManager)
        {
            var combatants = new List<Combatant>();

            // The set of RuntimeCharData the player controls — O(1) lookup
            var playerSet = new HashSet<RuntimeCharData>(gameManager.PlayerCharacters);

            foreach (var rcd in nearby)
            {
                // Extend this switch when you need faction-based neutrals,
                // temporary allies, charmed enemies, etc.
                Allegiance allegiance = playerSet.Contains(rcd)
                    ? Allegiance.Player
                    : Allegiance.Enemy;

                combatants.Add(new Combatant(rcd, allegiance));
            }

            return combatants;
        }
    }

}
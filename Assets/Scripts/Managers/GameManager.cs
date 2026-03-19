using System.Collections.Generic;
using DataModels;
using UnityEngine;

namespace Managers
{
    /**
     * Should handle game overall game state by controlling these:
     * - States of player characters
     * - Saving or loading game data
     * - Managing game progression (levels, quests, etc.)
     * - Handling game events and triggers
     */
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public List<Character> PlayerCharacters { get; private set; } = new List<Character>();
        
        // TODO: trigger combat manager.
        public void StartCombat()
        {
            
        }
        
    }
}

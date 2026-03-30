using System.Collections.Generic;
using Databases;
using DataModels;
using UnityEngine;

namespace InGameManagers
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
        
        [SerializeField] private CharacterSpriteDb CharacterSpriteDb; // assign in inspector, should be used to fetch sprites for characters on runtime.
        public List<CharacterData> PlayerCharacters { get; private set; } = new List<CharacterData>();
        
        // TODO: trigger combat manager.
        public void StartCombat()
        {
            // get a colliding sphere around the player and find all enemies in it, then trigger combat manager with those characters.
            
            
        }
        
        public Sprite GetCharacterSprite(int id)
        {
            return CharacterSpriteDb.GetSpriteById(id);
        }
    }
}

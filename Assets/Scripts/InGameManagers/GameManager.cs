using System;
using System.Collections.Generic;
using Databases;
using DataModels;
using UI;
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
        
        #region Databases 
        
        [SerializeField] private CharacterSpriteDb CharacterSpriteDb;
        [SerializeField] private SkillDb SkillDb;
        [SerializeField] private ItemDb ItemDb;
        [SerializeField] private StatusEffectDb StatusEffectDb;

        public CharacterSpriteDb GetCharacterSpriteDb() => CharacterSpriteDb;
        public SkillDb GetSkillDb() => SkillDb;
        public ItemDb GetItemDb() => ItemDb;
        public StatusEffectDb GetStatusEffectDb() => StatusEffectDb;
        
        #endregion

        #region Scene References

            [SerializeField] private CharacterMainUI characterMainUI;
        #endregion
        
        public List<RuntimeCharData> PlayerCharacters { get; private set; } = new List<RuntimeCharData>();
        
        [SerializeField] private CharacterSo testCharSo; // for testing, should be removed later.
        
        private void Awake()
        {
            // Check if an instance already exists to prevent duplicates
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
        
            // Optional: Keep the manager alive across different scenes
            // DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            characterMainUI.Initialize(new RuntimeCharData(testCharSo.ToCharacterData()));
        }

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

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
        [SerializeField] private SkillDb SkillDb; // assign in inspector, should be used to fetch skills for characters on runtime.
        
        
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
        
        public List<SkillBase> GetSkillsByCharData(CharacterData charData)
        {
            List<SkillBase> skills = new List<SkillBase>();
            foreach (var id in charData.SkillIds)
            {
                skills.Add(SkillDb.GetSkillById(id));
            }
            return skills;
        }
        
        public List<SkillBase> GetSkillsByIds(List<int> skillIds)
        {
            List<SkillBase> skills = new List<SkillBase>();
            foreach (var id in skillIds)
            {
                skills.Add(SkillDb.GetSkillById(id));
            }
            return skills;
        }
        
        public List<int> GetSkillIds(List<SkillBase> skills)
        {
            List<int> skillIds = new List<int>();
            foreach (var skill in skills)
            {
                int id = SkillDb.GetAllSkills().IndexOf(skill);
                if (id != -1)
                {
                    skillIds.Add(id);
                }
                else
                {
                    Debug.LogError("Skill " + skill.name + " not found in SkillDatabase.");
                }
            }
            return skillIds;
        }
    }
}

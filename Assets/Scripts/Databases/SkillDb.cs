using System.Collections.Generic;
using System.Linq;
using DataModels;
using Enums;
using UnityEngine;
using SkillExecutables;

namespace Databases
{
    [CreateAssetMenu(menuName = "Database/Skills")]
    public class SkillDb : DatabaseBase
    {
        public List<SkillBase> Skills = new List<SkillBase>();
        
        private Dictionary<string, SkillBase> _skillsDict = new();
        
        // for matching execution skills.
        public delegate void SkillAction(Combatant user, List<Combatant> targets, SkillBase skillBase);
        private static readonly Dictionary<string, SkillAction> _registry = new()
        {
            // Warrior skills
            [nameof(WarriorExecutables.BattleCry)]   = WarriorExecutables.BattleCry,
            [nameof(WarriorExecutables.Charge)]      = WarriorExecutables.Charge,
            [nameof(WarriorExecutables.LowBlow)]     = WarriorExecutables.LowBlow,
        };
        
        public static SkillAction Get(string skillName)
        {
            if (_registry.TryGetValue(skillName, out var action))
                return action;

            Debug.LogWarning($"[SkillLibrary] No skill registered for '{skillName}'");
            return null;
        }
        
        private void OnEnable()
        {
            _skillsDict.Clear();
            _skillsDict = Skills
                .Where(s => s != null && !string.IsNullOrEmpty(s.SkillId))
                .ToDictionary(s => s.SkillId);
            
            Debug.Log($"[SkillDb] Loaded {_skillsDict.Count} skills into the database.");
        }
        
        public List<SkillBase> GetAllSkills()
        {
            return Skills;
        }
        public SkillBase GetSkillById(string id)
        {
            if (_skillsDict.TryGetValue(id, out var skill))
                return skill;

            Debug.LogError($"Skill with id '{id}' not found in SkillDatabase.");
            return null;
        }
        
        public List<SkillBase> GetSkillsByIds(List<string> ids)
        {
            List<SkillBase> skills = new List<SkillBase>();
            foreach (var id in ids)
            {
                var skill = GetSkillById(id);
                if (skill != null)
                    skills.Add(skill);
            }
            return skills;
        }
        
        public List<SkillBase> GetSkillsByClass(CharacterClass characterClass)
        {
            List<SkillBase> skills = new List<SkillBase>();

            foreach (var skill in Skills)
            {
                if (skill.prerequisites.eligibleClasses.HasFlag((CharacterClassFlags)(1 << (int)characterClass)))
                {
                    skills.Add(skill);
                }
            }
            return skills;
        }
    }
}
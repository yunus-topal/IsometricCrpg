using System.Collections.Generic;
using System.Linq;
using DataModels;
using Enums;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Database/Skills")]
    public class SkillDb : ScriptableObject
    {
        public List<SkillBase> Skills = new List<SkillBase>();
        
        private Dictionary<string, SkillBase> _skillsDict = new();
        
        private void OnEnable()
        {
            _skillsDict = Skills
                .Where(s => s != null && !string.IsNullOrEmpty(s.SkillId))
                .ToDictionary(s => s.SkillId);
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
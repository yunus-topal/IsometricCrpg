using System.Collections.Generic;
using DataModels;
using Enums;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Database/Skills")]
    public class SkillDb : ScriptableObject
    {
        public List<SkillBase> Skills = new List<SkillBase>();
        
        private Dictionary<int, SkillBase> _skillsDict = new Dictionary<int, SkillBase>();
        
        private void OnEnable()
        {
            // populate the dictionary with the skills from the list, using the index as the id.
            _skillsDict.Clear();
            for (int i = 0; i < Skills.Count; i++)
            {
                _skillsDict.Add(i, Skills[i]);
            }
        }
        
        public List<SkillBase> GetAllSkills()
        {
            return Skills;
        }
        public SkillBase GetSkillById(int id)
        {
            // safety
            if(_skillsDict.Count == 0) {
                for (int i = 0; i < Skills.Count; i++)
                {
                    _skillsDict.Add(i, Skills[i]);
                }
            }
            // try to get the skill from the dictionary, if not found, return null and log an error.
            if (_skillsDict.TryGetValue(id, out var skill))  {
                return skill;
            }
            // return null if not found, should be handled on runtime to avoid null reference exception.
            Debug.LogError("Skill with id " + id + " not found in SkillDatabase.");
            return null;
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

        public int GetSkillId(SkillBase skill)
        {
            return Skills.IndexOf(skill);
        }
    }
}
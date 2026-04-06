using System.Collections.Generic;
using Enums;
using InGameManagers;
using UnityEngine;

namespace DataModels
{
    public class RuntimeCharData : CharacterData
    {
        public Sprite Sprite; // fetch from sprite db load on runtime.
        public List<SkillBase> Skills = new(); // TODO: convert to Skill object once it finalized.
        public string EquippedWeapon; // TODO: convert to Weapon object once it finalized.
        
        // TODO: keep track of selected traits, applied effects (not the ones applied during combat), equipment, inventory, etc.
        // they should be used for updating runtime stats.
        
        public List<StatusEffectBase> StatusEffects = new();

        public RuntimeCharData(CharacterData data)
        {
            Name = data.Name;
            Class = data.Class;
            Level = data.Level;
            Xp = data.Xp;
            Sprite = GameManager.Instance.GetCharacterSprite(data.SpriteId);
            CurrentHp = data.CurrentHp;
            Attributes = data.Attributes;
            
            // TODO: initialize skills, stattus effects, items etc. here using the ids in the base data, for now just leave it empty.
            Skills = GameManager.Instance.SkillDb.GetSkillsByIds(base.SkillIds); // TODO: also add skills coming from equipped weapon and traits when those systems are ready.
            
            CalculateDerivedStats();
        }
    }
}
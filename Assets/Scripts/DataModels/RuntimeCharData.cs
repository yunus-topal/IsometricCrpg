using System.Collections.Generic;
using DataModels.SkillSystem;
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

        public RuntimeCharData(CharacterData data)
        {
            Name = data.Name;
            Class = data.Class;
            Level = data.Level;
            Xp = data.Xp;
            Sprite = GameManager.Instance.GetCharacterSprite(data.SpriteId);
            CurrentHp = data.CurrentHp;
            Skills = new();
            Attributes = data.Attributes;
            // TODO: intialize weapon here later.
            
            CalculateDerivedStats();
        }
    }
}
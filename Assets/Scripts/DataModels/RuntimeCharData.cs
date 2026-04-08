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
        public ItemBase EquippedWeapon; 
        public List<ItemBase> InventoryItems = new();
        
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
            
            Skills = GameManager.Instance.GetSkillDb().GetSkillsByIds(base.SkillIds); // TODO: also add skills coming from equipped weapon and traits when those systems are ready.
            EquippedWeapon = GameManager.Instance.GetItemDb().GetItemById(data.EquippedWeaponId); 
            InventoryItems = GameManager.Instance.GetItemDb().GetItemsByIds(data.InventoryItemIds) ?? new List<ItemBase>();
            
            CalculateDerivedStats();
        }
    }
}
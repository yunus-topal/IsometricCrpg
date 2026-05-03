using System.Collections.Generic;
using Enums;
using InGameManagers;
using UnityEngine;

namespace DataModels
{
    public class RuntimeCharData : CharacterData
    {
        public Sprite Sprite; // fetch from sprite db load on runtime.
        public List<SkillBase> Skills = new();
        public List<RuntimeEquipmentData> Equipments = new();
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
            
            Skills = GameManager.Instance.GetSkillDb().GetSkillsByIds(base.SkillIds);
            Equipments = equipments.ConvertAll(c => new RuntimeEquipmentData(GameManager.Instance.GetItemDb().GetItemById(c.itemId), c.slot, c.canUnequip));
            InventoryItems = GameManager.Instance.GetItemDb().GetItemsByIds(data.InventoryItemIds) ?? new List<ItemBase>();
            
            CalculateDerivedStats();
        }
    }
    
    public class RuntimeEquipmentData
    {
        public ItemBase item;
        public EquipmentSlot slot;
        public bool canUnequip; // for some special items that cannot be unequipped, e.g. cursed items, quest items, etc.
        
        public RuntimeEquipmentData(ItemBase item, EquipmentSlot slot, bool canUnequip = true)
        {
            this.item = item;
            this.slot = slot;
            this.canUnequip = canUnequip;
        }
        
        public EquipmentData ToEquipmentData() => new(item.ItemId, slot, canUnequip);
    }
}
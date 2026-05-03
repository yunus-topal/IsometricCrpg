using System;
using System.Collections.Generic;
using Enums;
using InGameManagers;
using UnityEngine;
using Utils;

namespace DataModels
{
    public class RuntimeCharData
    {
        #region Base Data
        
        public CharacterData CharacterData;
        public Sprite Sprite; // fetch from sprite db load on runtime.
        public List<SkillBase> Skills = new();
        public List<RuntimeEquipmentData> Equipments = new();
        public List<ItemBase> InventoryItems = new();
        // TODO: keep track of selected traits, applied effects (not the ones applied during combat), equipment, inventory, etc.
        // they should be used for updating runtime stats.
        public List<StatusEffectBase> StatusEffects = new();

        #endregion

        #region Derived Stats

        // additional data that should be generated on runtime, not saved to save file using base data.
        public BindableProperty<int> MaxHp = new();
        public int Accuracy;
        public int Evasion;
        public int Resistance;
        public int CriticalChance;

        #endregion
        public RuntimeCharData(CharacterData data)
        {
            CharacterData = data;
            
            Sprite = GameManager.Instance.GetCharacterSprite(data.SpriteId);
            Skills = GameManager.Instance.GetSkillDb().GetSkillsByIds(data.SkillIds);
            Equipments = data.EquipmentDatas.ConvertAll(c => new RuntimeEquipmentData(GameManager.Instance.GetItemDb().GetItemById(c.itemId), c.slot, c.canUnequip));
            InventoryItems = GameManager.Instance.GetItemDb().GetItemsByIds(data.InventoryItemIds) ?? new List<ItemBase>();
            
            CalculateDerivedStats();
        }
        
        private void CalculateDerivedStats()
        {
            Accuracy = 100; // keep it %100 for now.
            Evasion = GlobalConstants.baseEvasion + GetAttributeModifier(CharacterData.Attributes.Agility); // base evasion + dexterity bonus
            Resistance = Math.Max(0, GlobalConstants.baseEnduranceMultiplier * GetAttributeModifier(CharacterData.Attributes.Endurance)); // for now, just use endurance as resistance.
            CriticalChance = GlobalConstants.baseCritChance;
        }
        
        private int GetAttributeModifier(int attributeValue)
        {
            return attributeValue switch
            {
                // brackets for -3, -2, -1, 0, +1, +2, +3 modifiers based on attribute value.
                < 3 => -3,
                < 6 => -2,
                < 9 => -1,
                < 12 => 0,
                < 15 => 1,
                < 18 => 2,
                _ => 3
            };
        }

        public override string ToString()
        {
            return base.ToString() + $"\nSprite: {Sprite}, \nStatusEffects: [{string.Join(", ", StatusEffects)}], \nRuntimeEquipments: [{string.Join(", ", Equipments)}]," +
                   $"\nSkills: [{string.Join(", ", Skills)}], \nInventoryItems: [{string.Join(", ", InventoryItems)}]";
        }
    }
    
    [Serializable]
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
        
        public override string ToString() => $"\n{item.ItemId} - {slot}";
    }
}
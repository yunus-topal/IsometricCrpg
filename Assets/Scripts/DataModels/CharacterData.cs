using System;
using System.Collections.Generic;
using Enums;
using Utils;

namespace DataModels
{
    /// <summary>
    /// This class should not be used directly.
    /// It is a data model to read and write save files.
    /// RuntimeCharData should be used for all runtime operations, and it should be generated from CharacterData on runtime.
    /// However, to use CharacterSo, it should first be converted to CharacterData, and then to RuntimeCharData on runtime.
    /// </summary>
    [Serializable]
    public class CharacterData
    {
        // base data for character, should be saved and loaded from save file.
        public string Name;
        public CharacterClass Class;
        public BindableProperty<int> Level = new (1);
        public BindableProperty<int> Xp = new();
        public int SpriteId; // fetch from sprite db load on runtime.
        public BindableProperty<int> CurrentHp = new();
        public List<string> SkillIds = new();
        public Attributes Attributes = new(); // fetch from skill db load on runtime.
        
        public List<EquipmentData> EquipmentDatas = new();
        public List<string> InventoryItemIds = new();
        
        public CharacterData (CharacterSo so)
        {
            Name = so.Name;
            Class = so.Class;
            Level = new(so.Level);
            Xp = new(so.Xp);
            SpriteId = so.SpriteId;
            CurrentHp = new(so.CurrentHp); // start with base hp
            Attributes = so.Attributes;

            SkillIds = so.Skills.ConvertAll(s => s.SkillId); // convert skill objects to their ids for saving.
            EquipmentDatas = so.Equipments.ConvertAll(e => new EquipmentData(e.item.ItemId, e.slot, e.canUnequip)); // convert item objects to their ids for saving.
            InventoryItemIds = so.InventoryItems.ConvertAll(i => i.ItemId); // convert item objects to their ids for saving.
        }

        // for deserialization
        public CharacterData()
        {
        }

        public override string ToString()
        {
            // write all fields in a readable format for debugging.
            return $"Name: {Name}, \nClass: {Class}, \nLevel: {Level.Value}, \nXp: {Xp.Value}, \nSpriteId: {SpriteId}, \nCurrentHp: {CurrentHp.Value}, " +
                   $"\nAttributes: [{Attributes}], \nSkillIds: [{string.Join(", ", SkillIds)}], \nEquipments: [{string.Join(", ", EquipmentDatas)}], \nInventoryItems: [{string.Join(", ", InventoryItemIds)}]";
        }
    }
    
    [System.Serializable]
    public class Attributes
    {
        public int Strength;
        public int Dexterity;
        public int Agility;
        public int Endurance;
        public int Intelligence;
        public int Willpower;

        // for deserialization
        public Attributes()
        {
        }
        public Attributes(int strength, int dexterity, int agility, int endurance, int intelligence, int willpower)
        {
            Strength = strength;
            Dexterity = dexterity;
            Agility = agility;
            Endurance = endurance;
            Intelligence = intelligence;
            Willpower = willpower;
        }

        public int GetAttribute(AttributeType type)
        {
            return type switch
            {
                AttributeType.Strength => Strength,
                AttributeType.Dexterity => Dexterity,
                AttributeType.Agility => Agility,
                AttributeType.Endurance => Endurance,
                AttributeType.Intelligence => Intelligence,
                AttributeType.Willpower => Willpower,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public override string ToString()
        {
            return $"STR: {Strength}, DEX: {Dexterity}, AGI: {Agility}, END: {Endurance}, INT: {Intelligence}, WIL: {Willpower}";
        }
    }

    [System.Serializable]
    public class EquipmentData
    {
        public string itemId;
        public EquipmentSlot slot;
        public bool canUnequip; // for some special items that cannot be unequipped, e.g. cursed items, quest items, etc.
        
        // for deserialization
        public EquipmentData(){}
        
        public EquipmentData(string itemId, EquipmentSlot slot, bool canUnequip = true)
        {
            this.itemId = itemId;
            this.slot = slot;
            this.canUnequip = canUnequip;
        }

        public override string ToString()
        {
            return $"\nItemId: {itemId}, Slot: {slot}, CanUnequip: {canUnequip}";
        }
    }
}
using System;
using System.Collections.Generic;
using Enums;

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
        public int Level;
        public int Xp;
        public int SpriteId; // fetch from sprite db load on runtime.
        public int CurrentHp;
        public List<string> SkillIds = new();
        public Attributes Attributes = new(); // fetch from skill db load on runtime.
        
        // additional data that should be generated on runtime, not saved to save file using base data.
        public int MaxHp;
        public int Accuracy;
        public int Evasion;
        public int Resistance;
        public int CriticalChance;
        
        // for later: selected traits, applied effects (not the ones applied during combat), equipment, inventory, etc.

        
        public CharacterData (CharacterSo so)
        {
            Name = so.Name;
            Class = so.Class;
            Level = 1; // start at level 1
            Xp = 0; // start with 0 xp
            SpriteId = so.SpriteId;
            CurrentHp = so.CurrentHp; // start with base hp
            Attributes = so.Attributes;
            
            CalculateDerivedStats();
        }

        // for deserialization
        public CharacterData()
        {
        }

        public void CalculateDerivedStats()
        {
            Accuracy = 100; // keep it %100 for now.
            Evasion = GlobalConstants.baseEvasion + GetAttributeModifier(Attributes.Agility); // base evasion + dexterity bonus
            Resistance = Math.Max(0, GlobalConstants.baseEnduranceMultiplier * GetAttributeModifier(Attributes.Endurance)); // for now, just use endurance as resistance.
            CriticalChance = GlobalConstants.baseCritChance;
        }

        public int GetAttributeModifier(int attributeValue)
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
    }
}
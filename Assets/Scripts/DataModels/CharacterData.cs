using System.Collections.Generic;

namespace DataModels
{
    [System.Serializable]
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
    }
}
using System.Collections.Generic;

namespace DataModels
{
    public class Character
    {
        // base data for character, should be saved and loaded from save file.
        public string Name { get; set; }
        public CharacterClass Class { get; set; }
        public int Level { get; set; }
        public int Xp { get; set; }
        public Attributes Attributes { get; set; }
        public string SpriteId { get; set; } // fetch from sprite db load on runtime.
        public int CurrentHp { get; set; }
        public List<string> SkillIds { get; set; } // fetch from skill db load on runtime.
        
        // additional data that should be generated on runtime, not saved to save file using base data.
        public int MaxHp { get; set; } 
        public int Accuracy { get; set; }
        public int Evasion { get; set; }
        public int Resistance { get; set; }
        public int CriticalChance { get; set; }
        
        // for later: selected traits, applied effects (not the ones applied during combat), equipment, inventory, etc.
    }
    
    public class Attributes
    {
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Agility { get; set; }
        public int Endurance { get; set; }
        public int Intelligence { get; set; }
        public int Willpower { get; set; }
    }
}
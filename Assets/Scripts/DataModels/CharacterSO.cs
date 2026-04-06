using System.Collections.Generic;
using Enums;
using InGameManagers;
using UnityEngine;

namespace DataModels
{
    /// <summary>
    /// in case I need a bunch of prebuild characters this can be helpful.
    /// OR maybe just for setting up some template enemy types easily.
    /// IMPORTANT: don't use this directly, instantiate it or create an instance of CharacterData
    /// to avoid modifying the SO data during runtime, which will affect all characters using this SO as base data.
    /// </summary>
    [CreateAssetMenu(fileName = "Character", menuName = "RPG/Character")]
    public class CharacterSo : ScriptableObject
    {
        public string Name;
        public CharacterClass Class;
        public int Level;
        public int Xp;
        public int SpriteId;
        public int CurrentHp;
        public List<SkillBase> Skills = new();
        public Attributes Attributes = new();
                
        // additional data that should be generated on runtime, not saved to save file using base data.
        public int MaxHp;
        public int Accuracy;
        public int Evasion;
        public int Resistance;
        public int CriticalChance;
        
        // for later: selected traits, applied effects (not the ones applied during combat), equipment, inventory, etc.
        
        public CharacterData ToCharacterData() => new()
        {
            Name        = Name,
            Class       = Class,
            Level       = Level,
            Xp          = Xp,
            SpriteId    = SpriteId,
            CurrentHp   = CurrentHp,
            SkillIds    = Skills.ConvertAll(s => s.SkillId), // convert skill objects to their ids for saving.
            Attributes  = new Attributes
            {
                Strength     = Attributes.Strength,
                Dexterity    = Attributes.Dexterity,
                Agility      = Attributes.Agility,
                Endurance    = Attributes.Endurance,
                Intelligence = Attributes.Intelligence,
                Willpower    = Attributes.Willpower,
            }
        };

        public void LoadFromSaveData(CharacterData data)
        {
            Name       = data.Name;
            Class      = data.Class;
            Level      = data.Level;
            Xp         = data.Xp;
            SpriteId   = data.SpriteId;
            CurrentHp  = data.CurrentHp;
            Skills = GameManager.Instance.SkillDb.GetSkillsByIds(data.SkillIds) ?? new List<SkillBase>(); // TODO: also add skills coming from equipped weapon and traits when those systems are ready.
            Attributes = data.Attributes;
        }
    }
}
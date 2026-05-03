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
        #region Base Data
        
        public string Name;
        public CharacterClass Class;
        public int Level;
        public int Xp;
        public int SpriteId;
        public int CurrentHp;
        public List<SkillBase> Skills = new();
        public Attributes Attributes = new();
        public List<RuntimeEquipmentData> Equipments = new (); 
        public List<ItemBase> InventoryItems = new(); 

        #endregion

        public CharacterData ToCharacterData() => new(this);

        public void LoadFromSaveData(CharacterData data)
        {
            Name       = data.Name;
            Class      = data.Class;
            Level      = data.Level.Value;
            Xp         = data.Xp.Value;
            SpriteId   = data.SpriteId;
            CurrentHp  = data.CurrentHp.Value;
            Skills = GameManager.Instance.GetSkillDb().GetSkillsByIds(data.SkillIds) ?? new List<SkillBase>(); 
            Attributes = data.Attributes;
            Equipments = data.EquipmentDatas.ConvertAll(e => new RuntimeEquipmentData(GameManager.Instance.GetItemDb().GetItemById(e.itemId), e.slot, e.canUnequip));
            InventoryItems = GameManager.Instance.GetItemDb().GetItemsByIds(data.InventoryItemIds) ?? new List<ItemBase>(); 
        }

        public override string ToString()
        {
            return $"Name: {Name}, \nClass: {Class}, \nLevel: {Level}, \nXp: {Xp}, \nSpriteId: {SpriteId}, \nCurrentHp: {CurrentHp}, " +
                   $"\nSkills: [{string.Join(", ", Skills)}], \nAttributes: {Attributes}, " +
                   $"\nEquipments: [{string.Join(", ", Equipments)}], \nInventoryItems: [{string.Join(", ", InventoryItems)}]";
        }
    }
}
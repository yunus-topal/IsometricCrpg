using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace DataModels
{
    // ─── Flags ──────────────────────────────────────────────────────────────────

    [Flags]
    public enum ItemFlags
    {
        None        = 0,
        Equippable  = 1 << 0,  // can be worn / held
        Consumable  = 1 << 1,  // disappears on use (potions, food)
        Throwable   = 1 << 2,  // can be thrown (grenades, knives)
        QuestItem   = 1 << 3,  // cannot be dropped or sold
    }

    // ─── Item type ───────────────────────────────────────────────────────────────

    public enum ItemType
    {
        None,
        Weapon,
        Shield,
        Helmet,
        Armor,
        Gauntlet,
        Boots,
        Potion,
        Grenade,
        Scroll,
        Food,
        QuestItem,
        Valuable,
        Book,
        Misc,
    }

    [Serializable]
    public struct ItemRequirements
    {
        [Tooltip("Leave empty to allow all classes.")]
        public CharacterClassFlags eligibleClasses;

        [Tooltip("Each entry is one attribute + minimum value pair.")]
        public List<AttributeRequirement> attributes;

        [Min(0)]
        public int minLevel;

        public bool MeetRequirements(RuntimeCharData character)
        {
            CharacterClassFlags characterFlag = (CharacterClassFlags)(1 << (int)character.Class);
            if ((eligibleClasses & characterFlag) == 0)
                return false;

            if (attributes is { Count: > 0 })
            {
                foreach (var req in attributes)
                    if (character.Attributes.GetAttribute(req.stat) < req.minValue)
                        return false;
            }

            if (character.Level < minLevel)
                return false;

            return true;
        }
    }

// ─── Item base ───────────────────────────────────────────────────────────────

    [CreateAssetMenu(fileName = "NewItem", menuName = "RPG/ItemBase")]
    public class ItemBase : ScriptableObject
    {
        // ── Display ──────────────────────────────────────────────────────────────

        [Header("Display")]
        public string itemName;
        [TextArea(2, 4)]
        public string description;
        public Sprite icon;
        
        [HideInInspector]
        public string ItemId;

        // ── Economy ──────────────────────────────────────────────────────────────

        [Header("Economy & Weight")]
        [Min(0)] public int   value;
        [Min(0)] public float weight;

        // ── Stacking ─────────────────────────────────────────────────────────────
        public enum StackSize
        {
            NotStackable = 0,
            Small        = 99,
            Large        = 999,
        }
        [Header("Stacking")]
        public StackSize stackSize = StackSize.NotStackable;
        public bool IsStackable  => stackSize != StackSize.NotStackable;
        public int  MaxStackSize => (int)stackSize;

        // ── Classification ───────────────────────────────────────────────────────

        [Header("Classification")]
        public ItemType  itemType;
        public ItemFlags flags;

        // ── Uses ─────────────────────────────────────────────────────────────────

        [Header("Uses")]
        [Tooltip("-1 means infinite (equipment, valuables, etc.)")]
        public int usesRemaining = -1;

        // ── Skills ───────────────────────────────────────────────────────────────

        [Header("Granted Skills")]
        [Tooltip(
            "Equipment: skills granted while item is equipped.\n" +
            "Consumables / Throwables: the skill triggered on use/throw.\n" +
            "Multiple skills are allowed (e.g. a weapon with an active + a passive).")]
        public List<SkillBase> grantedSkills = new();

        // ── Equip prerequisites ──────────────────────────────────────────────────

        [Header("Equip Requirements")]
        [Tooltip("Only evaluated when the item has the Equippable flag.")]
        public ItemRequirements requirements;

        // ── Convenience helpers ──────────────────────────────────────────────────

        private bool IsEquippable  => flags.HasFlag(ItemFlags.Equippable);
        private bool IsConsumable  => flags.HasFlag(ItemFlags.Consumable);
        private bool IsThrowable   => flags.HasFlag(ItemFlags.Throwable);
        private bool IsQuestItem   => flags.HasFlag(ItemFlags.QuestItem);
        private bool GrantsSkills  => grantedSkills is { Count: > 0 };
        private bool IsInfiniteUse => usesRemaining == -1;
        

        // a usable item should be usable from both inventory and equipped state (potions, food, scrolls etc,
        // weapons or armors are not usable! they are equippable, not usable).
        // only throwable and consumable items are usable. check those flags
        public bool CanUse(RuntimeCharData charData) =>  requirements.MeetRequirements(charData) && (IsConsumable || IsThrowable);
        
        // both wearable items such as weapons and armors, and usable items are equippable.
        public bool CanEquip(RuntimeCharData charData) => IsEquippable && requirements.MeetRequirements(charData);
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(ItemId))
            {
                ItemId = System.Guid.NewGuid().ToString();
                UnityEditor.EditorUtility.SetDirty(this); // marks asset dirty so Unity saves it
            }
        }
#endif
    }
}
using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace DataModels
{
    // ─────────────────────────────────────────────
//  Enums shared across the skill system
// ─────────────────────────────────────────────
    public enum TargetingMode
    {
        SingleEnemy,      // click a target
        SingleAlly,
        Self,
        CircleFree,      // click a point on the ground, circle AoE around it
        CircleCasterCentred, // circle around caster, no target needed
        RectangleFromCaster, // rectangle extending from caster, no target needed
        ConeFromCaster,  // aims in caster direction
        LineFromCaster,  // thin laser-style line
        Cross,            // + shaped, caster-centred
        Global            // no targeting needed
    }

    public enum ShapeType { Circle, Rectangle, Cone, Line, Cross }

// ─────────────────────────────────────────────
//  Prerequisite block
// ─────────────────────────────────────────────

    [System.Serializable]
    public class SkillPrerequisites
    {
        [Header("Learning requirements")]
        [Header("Class")]
        [Tooltip("Any of these classes can learn the skill.")]
        public CharacterClassFlags eligibleClasses ;

        [Header("Level")]
        public int requiredLevel = 1;

        [Header("Skills that must already be learned")]
        public List<SkillBase> requiredSkills = new();

        [Header("Attributes")]
        public List<AttributeRequirement> requiredAttributes = new();

        [Header("Usage requirements")]
        [Header("Status effects")]
        [Tooltip("Must NOT have any of these status effects to use the skill.")]
        public RestrictionTypeFlags prohibitedStatusEffects;
        
        [Header("Weapon")]
        [Tooltip("Any of these weapon types can use the skill.")]
        public WeaponTypeFlags eligibleWeapons;

        // Returns true when all requirements are met for the given character.
        public bool CanLearn(RuntimeCharData charData)
        {
            // Convert the plain enum to its flag equivalent by name
            CharacterClassFlags characterFlag = (CharacterClassFlags)(1 << (int)charData.Class);
            if ((eligibleClasses & characterFlag) == 0)
                return false;

            if (charData.Level < requiredLevel)
                return false;

            foreach (var skill in requiredSkills)
                if (!charData.Skills.Contains(skill))
                    return false;

            foreach (var attr in requiredAttributes)
                if (charData.Attributes.GetAttribute(attr.stat) < attr.minValue)
                    return false;
                
            return true;
        }
        
        public bool CanUse(RuntimeCharData charData)
        {
            // TODO: get current status effects from char data and check against prohibitedStatusEffects using RestrictionTypeFlags.
            
            WeaponTypeFlags weaponFlag = (WeaponTypeFlags)(1 << (int)1 /*charData.EquippedWeapon.WeaponType*/);
            if ((eligibleWeapons & weaponFlag) == 0)
                return false;
            return true;
        }
    }

    [System.Serializable]
    public class AttributeRequirement
    {
        public AttributeType stat;
        public int minValue;
    }

// ─────────────────────────────────────────────
//  Stat scaling entry
// ─────────────────────────────────────────────

// ─────────────────────────────────────────────
//  Target shape for AoE visualisation
// ─────────────────────────────────────────────

    [System.Serializable]
    public class TargetShapeData
    {
        public ShapeType shape = ShapeType.Circle;

        [Tooltip("Radius for Circle / Cross arm length.")]
        public float radius = 2f;

        [Tooltip("Width of Rectangle or Line.")]
        public float width = 2f;

        [Tooltip("Length of Rectangle or Line.")]
        public float length = 5f;

        [Tooltip("Half-angle in degrees for Cone.")]
        [Range(5f, 180f)]
        public float coneAngle = 45f;

        [Tooltip("Decal/projector material override. Leave null to use the default.")]
        public Material indicatorMaterialOverride;
    }

// ─────────────────────────────────────────────
//  Applied status effect entry inside SkillBase
// ─────────────────────────────────────────────

// TODO: experiment first,
    // [System.Serializable]
    // public class SkillStatusEffectEntry
    // {
    //     public StatusEffectBase effect;
    //
    //     [Tooltip("Number of stacks to apply. Ignored for non-stackable effects.")]
    //     public int stacks = 1;
    //
    //     [Tooltip("Apply to caster (true) or target (false).")]
    //     public bool applyToCaster = false;
    //
    //     [Tooltip("Chance 0–1 to apply. 1 = guaranteed.")]
    //     [Range(0f, 1f)]
    //     public float applicationChance = 1f;
    // }

// ─────────────────────────────────────────────
//  SkillBase – the main ScriptableObject
// ─────────────────────────────────────────────

    [CreateAssetMenu(fileName = "NewSkill", menuName = "RPG/SkillBase")]
    public class SkillBase : ScriptableObject
    {
        [Header("Identity")]
        public string skillName;
        
        // hide id from editor since it's auto-generated and not meant to be edited by hand
        [HideInInspector]
        public string SkillId; // GUID
        [TextArea(2, 4)]
        public string description;
        public Sprite icon;

        [Header("Timing")]
        [Tooltip("Cooldown in turns (or seconds if you use real-time).")]
        public float cooldown = 3f;

        [Header("Range")]
        [Tooltip("Maximum distance from caster to valid target / AoE centre.")]
        public float range = 6f;

        public TargetingMode targetingMode = TargetingMode.SingleEnemy;
        public TargetShapeData shape;

        [Header("Damage / Heal scaling")]
        [Tooltip("Min value before scaling (damage, heal amount, etc.).")]
        public float minValue = 10f;
        [Tooltip("Max value before scaling (damage, heal amount, etc.).")]
        public float maxValue = 10f;      

        // [Header("Status effects applied on cast")]
        // public List<SkillStatusEffectEntry> statusEffects = new();

        [Header("Prerequisites")]
        public SkillPrerequisites prerequisites;

        [Header("UI Tooltip extras")]
        [TextArea(1, 3)]
        public string flavorText;

        // ── Helpers ──────────────────────────────────────────────────────────

        /// <summary>Returns true if the character meets all prerequisites.</summary>
        public bool CanLearn(RuntimeCharData charData) => prerequisites.CanLearn(charData);
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(SkillId))
            {
                SkillId = System.Guid.NewGuid().ToString();
                UnityEditor.EditorUtility.SetDirty(this); // marks asset dirty so Unity saves it
            }
        }
#endif
    }
}
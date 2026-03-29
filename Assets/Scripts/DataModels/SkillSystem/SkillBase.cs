using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;

namespace DataModels.SkillSystem
{
    // ─────────────────────────────────────────────
//  Enums shared across the skill system
// ─────────────────────────────────────────────
    public enum TargetingMode
    {
        SingleEnemy,      // click a target
        SingleAlly,
        Self,
        GroundCircle,     // click ground → circle AoE
        GroundRectangle,  // click ground → rect AoE
        DirectionalCone,  // aims in caster direction
        DirectionalLine,  // thin laser-style line
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
        [Header("Class")]
        [Tooltip("Any of these classes can learn the skill. Empty means no class requirement.")]
        public List<CharacterClass> requiredClass;

        [Header("Level")]
        public int requiredLevel = 1;

        [Header("Skills that must already be learned")]
        public List<SkillBase> requiredSkills = new();

        [Header("Attributes")]
        public List<AttributeRequirement> requiredAttributes = new();

        [Header("Weapon")]
        public WeaponType requiredWeapon = WeaponType.Any;

        // Returns true when all requirements are met for the given character.
        public bool IsMet(RuntimeCharData charData)
        {
            if (requiredClass.Count == 0 || requiredClass.Contains(charData.Class))
                return false;

            if (charData.Level < requiredLevel)
                return false;

            foreach (var skill in requiredSkills)
                if (!charData.Skills.Contains(skill))
                    return false;

            foreach (var attr in requiredAttributes)
                if (charData.Attributes.GetAttribute(attr.stat) < attr.minValue)
                    return false;

            // TODO later
            // if (requiredWeapon != WeaponType.Any && charData.equippedWeapon != requiredWeapon)
            //     return false;

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

// TODO: change this logic.
    [System.Serializable]
    public class StatScaling
    {
        public AttributeType stat;
        [Range(0f, 5f)]
        [Tooltip("Multiplier applied to the stat value (e.g. 0.5 = 50% of Strength added to damage).")]
        public float ratio;

        public float ComputeBonus(RuntimeCharData stats) => stats.Attributes.GetAttribute(stat) * ratio;
    }

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
        [Tooltip("Base value before scaling (damage, heal amount, etc.).")]
        public float baseValue = 10f;
        public List<StatScaling> scaling = new();

        // [Header("Status effects applied on cast")]
        // public List<SkillStatusEffectEntry> statusEffects = new();

        [Header("Prerequisites")]
        public SkillPrerequisites prerequisites;

        [Header("UI Tooltip extras")]
        [TextArea(1, 3)]
        public string flavorText;

        // ── Helpers ──────────────────────────────────────────────────────────

        /// <summary>Returns true if the character meets all prerequisites.</summary>
        public bool CanLearn(RuntimeCharData charData) => prerequisites.IsMet(charData);

        /// <summary>Computes the total effective value (damage/heal) for a caster.</summary>
        public float ComputeValue(RuntimeCharData caster)
        {
            float total = baseValue;
            foreach (var s in scaling)
                total += s.ComputeBonus(caster);
            return total;
        }

        /// <summary>Builds the portion of the tooltip showing scaling breakdown.</summary>
        public string BuildScalingTooltip()
        {
            if (scaling.Count == 0) return string.Empty;
            var sb = new System.Text.StringBuilder();
            foreach (var s in scaling)
                sb.AppendLine($"+ {s.ratio * 100:0}% {s.stat}");
            return sb.ToString().TrimEnd();
        }
    }
}
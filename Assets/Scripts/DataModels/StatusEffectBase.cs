using UnityEngine;
using UnityEngine.Serialization;

namespace DataModels
{
    public enum StackingBehavior
    {
        None,               // can't stack, always 1 turn.
        Refresh,            // can't stack, but reapplying refreshes duration.
        Duration,           // one instance, stacks just extend duration
        MultiInstance,      // multiple instances, each tracked separately
    }
    
    public enum RestrictionType
    {
        None,
        Incapacitated,      // can't act at all
        Immobilized,        // can't move, but can still act
        Silenced,           // can't use skills, but can still basic attack and items
        Disarmed,           // can't use weapon-based skills, but can still use unarmed skills and items
        Slowed,             // reduced movement speed, but can still act normally otherwise
    }
    
    [System.Flags]
    public enum RestrictionTypeFlags
    {
        None = 0,
        Incapacitated = 1 << 0, // can't act at all
        Immobilized = 1 << 1,   // can't move, but can still act
        Silenced = 1 << 2,      // can't use skills, but can still basic attack and items
        Disarmed = 1 << 3,      // can't use weapon-based skills, but can still use unarmed skills and items
        Slowed = 1 << 4,        // reduced movement speed, but can still act normally otherwise
    }
    
    
    [CreateAssetMenu(fileName = "NewStatusEffect", menuName = "RPG/StatusEffectBase")]
    public class StatusEffectBase : ScriptableObject
    {
        [Header("Identity")]
        public string statusEffectName;
        // hide id from editor since it's auto-generated and not meant to be edited by hand
        [HideInInspector]
        public string statusEffectId;
        [TextArea(2, 4)]
        public string description;
        public Sprite icon;
        
        public StackingBehavior stackingBehavior;
        public RestrictionType restrictionType;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(statusEffectId))
            {
                statusEffectId = System.Guid.NewGuid().ToString();
                UnityEditor.EditorUtility.SetDirty(this); // marks asset dirty so Unity saves it
            }
        }
#endif
    }
}
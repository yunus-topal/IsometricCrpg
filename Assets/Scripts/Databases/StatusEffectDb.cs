using System.Collections.Generic;
using System.Linq;
using DataModels;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Database/StatusEffects")]
    public class StatusEffectDb : DatabaseBase
    {
        public List<StatusEffectBase> StatusEffects = new List<StatusEffectBase>();
        
        private Dictionary<string, StatusEffectBase> _statusEffectsDict = new();
        
        private void OnEnable()
        {
            _statusEffectsDict.Clear();
            
            _statusEffectsDict = StatusEffects
                .Where(s => s != null && !string.IsNullOrEmpty(s.statusEffectId))
                .ToDictionary(s => s.statusEffectName);
            
            Debug.Log($"[StatusEffectDb] Loaded {_statusEffectsDict.Count} status effects into the database.");
        }
        
        public List<StatusEffectBase> GetAllStatusEffects()
        {
            return StatusEffects;
        }

        public StatusEffectBase GetStatusEffectById(string id)
        {
            if (_statusEffectsDict.TryGetValue(id, out var statusEffect))
                return statusEffect;

            Debug.LogError($"Status effect with id '{id}' not found in StatusEffectDatabase.");
            return null;
        }
        
        public List<StatusEffectBase> GetStatusEffectsByIds(List<string> ids)
        {
            List<StatusEffectBase> statusEffects = new List<StatusEffectBase>();
            foreach (var id in ids)
            {
                var statusEffect = GetStatusEffectById(id);
                if (statusEffect != null)
                    statusEffects.Add(statusEffect);
            }
            return statusEffects;
        }
    }
}

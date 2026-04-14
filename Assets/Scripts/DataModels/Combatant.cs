using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace DataModels
{
    /// <summary>
    /// Thin wrapper around RuntimeCharData that adds combat-only state.
    /// Allegiance lives here — not in CharData or RuntimeCharData — because
    /// it is context-dependent (the same character could be neutral in one
    /// encounter and hostile in another).
    /// </summary>
    public class Combatant
    {
        // ── Core reference (single source of truth for stats) ────────────────
        public RuntimeCharData RuntimeData { get; private set; }

        // Convenience shortcut — never duplicate, always delegate
        public string Name => RuntimeData.Name;
        public Attributes Attributes => RuntimeData.Attributes;

        // ── Combat-context state (transient, lives only during combat) ────────
        public Allegiance Allegiance { get; private set; }

        public int CurrentHp { get; set; }
        public float Initiative { get; private set; }

        public bool IsAlive => CurrentHp > 0;
        public bool HasActed { get; set; } // reset each round

        public List<StatusEffectInstance> ActiveStatusEffects { get; } = new();

        // ── Construction ──────────────────────────────────────────────────────
        public Combatant(RuntimeCharData runtimeData, Allegiance allegiance)
        {
            RuntimeData = runtimeData;
            Allegiance = allegiance;

            // Snapshot HP/MP from runtime data at combat start
            CurrentHp = RuntimeData.CurrentHp;
        }

        // ── Initiative ────────────────────────────────────────────────────────
        /// <summary>
        /// Roll initiative. Formula is intentionally isolated here so you can
        /// extend it later (equipment bonuses, status effects, etc.).
        /// </summary>
        public void RollInitiative()
        {
            Initiative = CombatUtils.CalculateInitiative(this);
        }

        // ── Status effects ────────────────────────────────────────────────────
        public void ApplyStatusEffect(StatusEffectInstance effect)
        {
            ActiveStatusEffects.Add(effect);
            Debug.Log($"[Combatant] {Name} afflicted with {effect.Definition.name}.");
        }

        public void RemoveStatusEffect(StatusEffectInstance effect)
        {
            ActiveStatusEffects.Remove(effect);
            Debug.Log($"[Combatant] {Name} recovered from {effect.Definition.name}.");
        }

        /// <summary>
        /// Tick every status effect at the start of this combatant's turn.
        /// Removes expired effects automatically.
        /// </summary>
        public void TickStatusEffects()
        {
            for (int i = ActiveStatusEffects.Count - 1; i >= 0; i--)
            {
                var instance = ActiveStatusEffects[i];
                instance.Tick(this);

                if (instance.IsExpired)
                {
                    Debug.Log($"[Combatant] {Name}: {instance.Definition.name} wore off.");
                    ActiveStatusEffects.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Flush combat-only state back to RuntimeCharData so changes persist
        /// after combat ends.
        /// </summary>
        public void FlushToRuntimeData()
        {
            RuntimeData.CurrentHp = CurrentHp;
        }
    }

// ─────────────────────────────────────────────────────────────────────────────
// Allegiance enum — kept small and extensible
// ─────────────────────────────────────────────────────────────────────────────
    public enum Allegiance
    {
        Player,
        Enemy,
        Neutral // e.g. summons, wildlife — acts on its own logic
    }
}
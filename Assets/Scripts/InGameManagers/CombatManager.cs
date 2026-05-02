using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataModels;
using Logic;
using UnityEngine;
using Utils;

namespace InGameManagers
{
    /// <summary>
    /// Handle combat loop, turn order, and combat state (e.g., player turn, enemy turn, victory, defeat).
    /// Provide basic methods for dealing damage, healing etc. Skills should not directly modify character stats.
    /// This class should use StatusEffectManager to apply status effects and handle their interactions during combat.
    /// </summary>
    public class CombatManager : MonoBehaviour
    {
    // ── External dependencies ─────────────────────────────────────────────
    [Tooltip("Dependent Managers.")]
    private GameManager   gameManager;
    private UIManager     uiManager;
    
    [Tooltip("Combat settings.")]
    [SerializeField] private float AiTurnDelay = 1.0f; // seconds to wait before enemy acts, for readability
    

    // ── Runtime state ─────────────────────────────────────────────────────
    private List<Combatant> _turnOrder    = new();
    private int             _currentIndex = 0;
    private bool            _combatActive = false;

    // ── Events (optional — lets other systems react without tight coupling)
    public event Action<Combatant>        OnTurnStarted;
    public event Action<Combatant>        OnTurnEnded;
    public event Action<List<Combatant>>  OnCombatStarted;
    public event Action<Allegiance>       OnCombatEnded;   // winner side

    private void Start()
    {
        gameManager = GetComponent<GameManager>();
        uiManager = GetComponent<UIManager>();
    }


    // ─────────────────────────────────────────────────────────────────────
    // 1. START COMBAT
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Entry point.  Pass in every character near the encounter trigger.
    /// The factory figures out allegiance from GameManager's player roster.
    /// </summary>
    public void StartCombat(List<RuntimeCharData> nearbyCharacters)
    {
        if (_combatActive)
        {
            Debug.LogWarning("[CombatManager] StartCombat called while combat is already active.");
            return;
        }

        _turnOrder    = CombatantFactory.BuildCombatants(nearbyCharacters, gameManager);
        _currentIndex = 0;
        _combatActive = true;

        Debug.Log($"[CombatManager] Combat started with {_turnOrder.Count} combatants.");
        OnCombatStarted?.Invoke(_turnOrder);

        RollAndSortInitiative();
        StartCoroutine(CombatLoop());
    }

    // ─────────────────────────────────────────────────────────────────────
    // 2. INITIATIVE
    // ─────────────────────────────────────────────────────────────────────

    private void RollAndSortInitiative()
    {
        foreach (var c in _turnOrder)
            c.RollInitiative();

        // Descending: highest initiative acts first
        _turnOrder = _turnOrder
            .OrderByDescending(c => c.Initiative)
            .ToList();

        Debug.Log("[CombatManager] Turn order: " +
                  string.Join(" → ", _turnOrder.Select(c => $"{c.Name}({c.Initiative:F1})")));
    }

    // ─────────────────────────────────────────────────────────────────────
    // 3. MAIN COMBAT LOOP
    // ─────────────────────────────────────────────────────────────────────

    private IEnumerator CombatLoop()
    {
        while (_combatActive)
        {
            // Filter dead combatants each iteration — don't remove from list
            // mid-loop to keep index stable; just skip them.
            var current = _turnOrder[_currentIndex];

            if (!current.IsAlive)
            {
                AdvanceTurn();
                continue;
            }

            // ── Status effects tick at start of turn ─────────────────────
            current.TickStatusEffects();

            // Re-check after DoT effects
            if (!current.IsAlive)
            {
                Debug.Log($"[CombatManager] {current.Name} died from status effects.");
                if (CheckCombatEnd()) yield break;
                AdvanceTurn();
                continue;
            }

            // ── Dispatch turn by allegiance ───────────────────────────────
            OnTurnStarted?.Invoke(current);
            Debug.Log($"[CombatManager] {current.Name}'s turn ({current.Allegiance}).");

            bool turnComplete = false;

            switch (current.Allegiance)
            {
                case Allegiance.Player:
                    yield return StartCoroutine(HandlePlayerTurn(current, () => turnComplete = true));
                    break;

                case Allegiance.Enemy:
                    yield return StartCoroutine(HandleEnemyTurn(current));
                    turnComplete = true;
                    break;

                case Allegiance.Neutral:
                    yield return StartCoroutine(HandleNeutralTurn(current));
                    turnComplete = true;
                    break;
            }

            // Wait until callback fires (player turns are async)
            yield return new WaitUntil(() => turnComplete || !_combatActive);

            if (!_combatActive) yield break;

            OnTurnEnded?.Invoke(current);

            if (CheckCombatEnd()) yield break;

            AdvanceTurn();
        }
    }

    private void AdvanceTurn()
    {
        _currentIndex = (_currentIndex + 1) % _turnOrder.Count;
    }

    // ─────────────────────────────────────────────────────────────────────
    // 4. PLAYER TURN  (UI-driven, callback-based)
    // ─────────────────────────────────────────────────────────────────────

    private IEnumerator HandlePlayerTurn(Combatant player, Action onComplete)
    {
        // Ask UIManager to show skill picker and target selector.
        // UIManager calls back with the chosen skill + resolved targets.
        bool waiting = true;

        uiManager.ShowPlayerTurnUI(
            activeCombatant : player,
            allCombatants   : _turnOrder,
            onSkillConfirmed: (skill, targets) =>
            {
                ExecuteSkill(player, skill, targets);
                waiting = false;
                onComplete?.Invoke();
            }
        );

        yield return new WaitUntil(() => !waiting);
    }

    // ─────────────────────────────────────────────────────────────────────
    // 5. ENEMY TURN  (AI stub — replace with real AI later)
    // ─────────────────────────────────────────────────────────────────────

    private IEnumerator HandleEnemyTurn(Combatant enemy)
    {
        yield return new WaitForSeconds(AiTurnDelay);  // brief pause for readability

        var (skill, targets) = EnemyAIStub.SelectSkillAndTargets(enemy, _turnOrder);

        if (skill != null && targets != null && targets.Count > 0)
            ExecuteSkill(enemy, skill, targets);
        else
            Debug.Log($"[CombatManager] {enemy.Name} skips its turn (no valid action).");
    }

    // ─────────────────────────────────────────────────────────────────────
    // 6. NEUTRAL TURN  (stub — customize per-creature type)
    // ─────────────────────────────────────────────────────────────────────

    private IEnumerator HandleNeutralTurn(Combatant neutral)
    {
        yield return new WaitForSeconds(AiTurnDelay);
        var (skill, targets) = EnemyAIStub.SelectSkillAndTargets(neutral, _turnOrder);

        if (skill != null && targets != null && targets.Count > 0)
            ExecuteSkill(neutral, skill, targets);
        else
            Debug.Log($"[CombatManager] {neutral.Name} skips its turn (no valid action).");
    }

    // ─────────────────────────────────────────────────────────────────────
    // 7. SKILL EXECUTION
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Single execution path for all allegiances.  TargetType is already
    /// resolved by the time we get here — either by the AI stub or by UIManager.
    /// </summary>
    private void ExecuteSkill(Combatant user, SkillBase skill, List<Combatant> targets)
    {
        // TODO: check skill cooldown
        // can also be don on the ui side.
        skill.Execute(user, targets);
    }

    // ─────────────────────────────────────────────────────────────────────
    // 8. COMBAT-END CHECK
    // ─────────────────────────────────────────────────────────────────────

    /// <returns>True if combat should end.</returns>
    private bool CheckCombatEnd()
    {
        bool playersAlive  = _turnOrder.Any(c => c.Allegiance == Allegiance.Player  && c.IsAlive);
        bool enemiesAlive  = _turnOrder.Any(c => c.Allegiance == Allegiance.Enemy   && c.IsAlive);

        if (!playersAlive)
        {
            EndCombat(Allegiance.Enemy);
            return true;
        }

        if (!enemiesAlive)
        {
            EndCombat(Allegiance.Player);
            return true;
        }

        return false;
    }

    private void EndCombat(Allegiance winner)
    {
        _combatActive = false;

        Debug.Log($"[CombatManager] Combat ended. Winner: {winner}");
        OnCombatEnded?.Invoke(winner);

        uiManager.HideCombatUI();
    }
    }
}

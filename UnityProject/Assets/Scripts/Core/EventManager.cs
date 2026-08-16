using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeasOfLegends.Core
{
    /// <summary>
    /// Global event bus using Observer pattern.
    /// Decouples systems so they don't need direct references.
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }

        // Combat Events
        public event Action<CombatEventData> OnPlayerDealtDamage;
        public event Action<CombatEventData> OnPlayerTookDamage;
        public event Action<GameObject> OnEnemyDefeated;
        public event Action OnComboBroken;
        public event Action<int> OnComboExtended; // int = current combo count
        public event Action OnFinisherTriggered;

        // World Events
        public event Action<IslandBiomeSO> OnIslandDiscovered;
        public event Action OnWeatherChanged;
        public event Action<float> OnStormIntensityChanged;

        // Player Events
        public event Action OnPlayerDashed;
        public event Action OnPlayerWallRunStarted;
        public event Action OnPlayerWallRunEnded;
        public event Action OnPlayerLanded;
        public event Action OnPlayerDeath;

        // Quest Events
        public event Action<QuestData> OnQuestStarted;
        public event Action<QuestData> OnQuestCompleted;
        public event Action<QuestData, int> OnQuestObjectiveUpdated; // quest, objectiveIndex
        public event Action<string> OnDialogueTriggered; // dialogueNodeID

        // Input Events
        public event Action OnPauseRequested;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // --- Combat Event Triggers ---
        public void TriggerPlayerDealtDamage(CombatEventData data) => OnPlayerDealtDamage?.Invoke(data);
        public void TriggerPlayerTookDamage(CombatEventData data) => OnPlayerTookDamage?.Invoke(data);
        public void TriggerEnemyDefeated(GameObject enemy) => OnEnemyDefeated?.Invoke(enemy);
        public void TriggerComboBroken() => OnComboBroken?.Invoke();
        public void TriggerComboExtended(int count) => OnComboExtended?.Invoke(count);
        public void TriggerFinisher() => OnFinisherTriggered?.Invoke();

        // --- World Event Triggers ---
        public void TriggerIslandDiscovered(IslandBiomeSO island) => OnIslandDiscovered?.Invoke(island);
        public void TriggerWeatherChanged() => OnWeatherChanged?.Invoke();
        public void TriggerStormIntensity(float intensity) => OnStormIntensityChanged?.Invoke(intensity);

        // --- Player Event Triggers ---
        public void TriggerPlayerDashed() => OnPlayerDashed?.Invoke();
        public void TriggerPlayerWallRunStart() => OnPlayerWallRunStarted?.Invoke();
        public void TriggerPlayerWallRunEnd() => OnPlayerWallRunEnded?.Invoke();
        public void TriggerPlayerLanded() => OnPlayerLanded?.Invoke();
        public void TriggerPlayerDeath() => OnPlayerDeath?.Invoke();

        // --- Quest Event Triggers ---
        public void TriggerQuestStarted(QuestData quest) => OnQuestStarted?.Invoke(quest);
        public void TriggerQuestCompleted(QuestData quest) => OnQuestCompleted?.Invoke(quest);
        public void TriggerQuestObjectiveUpdated(QuestData quest, int objectiveIndex) 
            => OnQuestObjectiveUpdated?.Invoke(quest, objectiveIndex);
        public void TriggerDialogue(string nodeID) => OnDialogueTriggered?.Invoke(nodeID);

        // --- Input Event Triggers ---
        public void TriggerPause() => OnPauseRequested?.Invoke();
    }

    /// <summary>
    /// Data packet sent with combat events. Contains all context for VFX, camera, UI.
    /// </summary>
    [System.Serializable]
    public struct CombatEventData
    {
        public GameObject Attacker;
        public GameObject Defender;
        public float Damage;
        public float HitStopDuration;
        public Vector3 HitPoint;
        public Vector3 HitNormal;
        public ElementType Element;
        public bool WasCritical;
        public bool WasBlocked;
        public AttackDataSO AttackData;
    }

    public enum ElementType
    {
        None,
        Fire,       // Red/orange trails, burn VFX
        Water,      // Blue splashes, ice crystals
        Lightning,  // Yellow arcs, thunder bursts
        Wind,       // Green spirals, leaf particles
        Earth,      // Brown dust, rock shards
        Darkness,   // Purple/black smoke, void tendrils
        Light       // Golden rays, holy sparkles
    }
}

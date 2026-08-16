using System;
using UnityEngine;

namespace SeasOfLegends.Core
{
    /// <summary>
    /// Lightweight application-wide event hub. Systems publish immutable payloads instead of
    /// depending directly on UI, camera, quest, or VFX implementations.
    /// </summary>
    public static class GameEvents
    {
        public static event Action<CombatHit> CombatHitResolved;
        public static event Action<string> QuestCompleted;
        public static event Action<IslandStreamingEvent> IslandStreamChanged;

        public static void RaiseCombatHit(CombatHit hit) => CombatHitResolved?.Invoke(hit);
        public static void RaiseQuestCompleted(string questId) => QuestCompleted?.Invoke(questId);
        public static void RaiseIslandStreamChanged(IslandStreamingEvent streamEvent) => IslandStreamChanged?.Invoke(streamEvent);
    }

    public struct CombatHit
    {
        public readonly GameObject Attacker;
        public readonly GameObject Defender;
        public readonly Vector3 Point;
        public readonly Vector3 Direction;
        public readonly float Damage;
        public readonly bool Blocked;
        public readonly bool Launched;

        public CombatHit(GameObject attacker, GameObject defender, Vector3 point, Vector3 direction, float damage, bool blocked, bool launched)
        {
            Attacker = attacker;
            Defender = defender;
            Point = point;
            Direction = direction;
            Damage = damage;
            Blocked = blocked;
            Launched = launched;
        }
    }

    public struct IslandStreamingEvent
    {
        public readonly string IslandId;
        public readonly bool IsLoaded;

        public IslandStreamingEvent(string islandId, bool isLoaded)
        {
            IslandId = islandId;
            IsLoaded = isLoaded;
        }
    }
}

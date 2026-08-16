using System.Collections.Generic;
using UnityEngine;
using SeasOfLegends.Core;
using SeasOfLegends.Data;

namespace SeasOfLegends.World
{
    /// <summary>
    /// Assign the ship or player as streamingFocus and author IslandDefinitions with prefab and
    /// world position. Separate load/unload radii prevent instantiate/destroy thrashing at edges.
    /// For production, replace Instantiate with Addressables.LoadAssetAsync or additive scenes.
    /// </summary>
    public sealed class IslandManager : MonoBehaviour
    {
        [SerializeField] private Transform streamingFocus;
        [SerializeField] private IslandDefinition[] islands;
        [SerializeField, Min(0.05f)] private float pollInterval = 0.5f;

        private readonly Dictionary<IslandDefinition, GameObject> loaded = new Dictionary<IslandDefinition, GameObject>();
        private float nextPollAt;

        private void Update()
        {
            if (streamingFocus == null || islands == null || Time.time < nextPollAt) return;
            nextPollAt = Time.time + pollInterval;
            for (int i = 0; i < islands.Length; i++) EvaluateIsland(islands[i]);
        }

        private void EvaluateIsland(IslandDefinition island)
        {
            if (island == null || island.IslandPrefab == null) return;
            float distanceSquared = (streamingFocus.position - island.WorldPosition).sqrMagnitude;
            bool isLoaded = loaded.ContainsKey(island);
            if (!isLoaded && distanceSquared <= island.LoadRadius * island.LoadRadius) Load(island);
            else if (isLoaded && distanceSquared >= island.UnloadRadius * island.UnloadRadius) Unload(island);
        }

        private void Load(IslandDefinition island)
        {
            GameObject instance = Instantiate(island.IslandPrefab, island.WorldPosition, Quaternion.identity, transform);
            instance.name = "Island_" + island.IslandId;
            loaded.Add(island, instance);
            GameEvents.RaiseIslandStreamChanged(new IslandStreamingEvent(island.IslandId, true));
        }

        private void Unload(IslandDefinition island)
        {
            if (!loaded.TryGetValue(island, out GameObject instance)) return;
            Destroy(instance);
            loaded.Remove(island);
            GameEvents.RaiseIslandStreamChanged(new IslandStreamingEvent(island.IslandId, false));
        }

        private void OnDrawGizmosSelected()
        {
            if (islands == null) return;
            foreach (IslandDefinition island in islands)
            {
                if (island == null) continue;
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(island.WorldPosition, island.LoadRadius);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(island.WorldPosition, island.UnloadRadius);
            }
        }
    }
}

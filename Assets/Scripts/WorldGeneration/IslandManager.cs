using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the loading and unloading of island chunks in an open-world ocean.
/// Uses a grid-based chunk system to stream islands based on player distance.
/// Attach to a persistent GameObject (e.g., GameManager).
/// </summary>
public class IslandManager : MonoBehaviour
{
    [Header("Chunk Settings")]
    public float chunkSize = 1000f; // Size of each chunk in world units
    public float loadDistance = 2000f; // Load chunks within this distance of player
    public float unloadDistance = 2500f; // Unload chunks beyond this distance

    [Header("References")]
    public Transform player; // Reference to the player's ship or character
    public GameObject oceanPrefab; // Base ocean prefab (for empty chunks)

    [Header("Island Data")]
    public List<IslandData> islandDataList; // Population of possible islands

    // Internal tracking of loaded chunks: key = chunk coordinates (x,z) as string
    private Dictionary<string, GameObject> loadedChunks = new Dictionary<string, GameObject>();

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("IslandManager: Player reference not set!");
            enabled = false;
            return;
        }

        // Initial load around player
        UpdateChunks();
    }

    private void Update()
    {
        // Update chunk loading/unloading every few seconds to avoid per-frame cost
        // Alternatively, could trigger on significant player movement
        static float lastUpdateTime;
        if (Time.time - lastUpdateTime > 2f) // Update every 2 seconds
        {
            lastUpdateTime = Time.time;
            UpdateChunks();
        }
    }

    /// <summary>
    /// Calculates which chunks should be loaded based on player position and loads/unloads accordingly.
    /// </summary>
    private void UpdateChunks()
    {
        if (player == null) return;

        // Get player's current chunk coordinates
        Vector3 playerPos = player.position;
        int playerChunkX = Mathf.FloorToInt(playerPos.x / chunkSize);
        int playerChunkZ = Mathf.FloorToInt(playerPos.z / chunkSize);

        // Determine which chunks to consider (within load radius)
        int radiusChunks = Mathf.CeilToInt(loadDistance / chunkSize);

        // First, unload chunks that are too far away
        List<string> toUnload = new List<string>();
        foreach (var kvp in loadedChunks)
        {
            string[] coords = kvp.Key.Split(',');
            int chunkX = int.Parse(coords[0]);
            int chunkZ = int.Parse(coords[1]);

            float distance = Mathf.Sqrt(
                Mathf.Pow((chunkX - playerChunkX) * chunkSize, 2) +
                Mathf.Pow((chunkZ - playerChunkZ) * chunkSize, 2)
            );

            if (distance > unloadDistance)
            {
                toUnload.Add(kvp.Key);
            }
        }

        // Unload distant chunks
        foreach (string key in toUnload)
        {
            UnloadChunk(key);
        }

        // Load/newload chunks that are within load distance but not yet loaded
        for (int xOffset = -radiusChunks; xOffset <= radiusChunks; xOffset++)
        {
            for (int zOffset = -radiusChunks; zOffset <= radiusChunks; zOffset++)
            {
                int chunkX = playerChunkX + xOffset;
                int chunkZ = playerChunkZ + zOffset;
                string key = $"{chunkX},{chunkZ}";

                // Skip if already loaded
                if (loadedChunks.ContainsKey(key)) continue;

                // Calculate distance to this chunk's center
                Vector3 chunkCenter = new Vector3(
                    chunkX * chunkSize + chunkSize / 2f,
                    0,
                    chunkZ * chunkSize + chunkSize / 2f
                );
                float distance = Vector3.Distance(playerPos, chunkCenter);

                if (distance <= loadDistance)
                {
                    LoadChunk(chunkX, chunkZ);
                }
            }
        }
    }

    /// <summary>
    /// Loads a chunk at the given coordinates.
    /// Decides whether to place an island or just ocean based on some logic (e.g., noise, predefined list).
    /// </summary>
    private void LoadChunk(int chunkX, int chunkZ)
    {
        // Determine if this chunk should contain an island.
        // For simplicity, we'll use a placeholder: every 10th chunk has an island.
        // In a real game, you'd use procedural generation (perlin noise) or hand-placed data.
        bool shouldHaveIsland = (chunkX * chunkZ) % 10 == 0; // Example condition

        GameObject chunkObj = new GameObject($"Chunk_{chunkX}_{chunkZ}");
        chunkObj.transform.position = new Vector3(
            chunkX * chunkSize,
            0,
            chunkZ * chunkSize
        );

        if (shouldHaveIsland)
        {
            // Select a random island data from our list (or use procedural generation)
            IslandData data = islandDataList.Count > 0
                ? islandDataList[Random.Range(0, islandDataList.Count)]
                : null;

            if (data != null && data.islandPrefab != null)
            {
                // Instantiate the island prefab at the chunk's center
                GameObject islandInstance = Instantiate(
                    data.islandPrefab,
                    chunkObj.transform.position + Vector3.up * 50f, // Slightly above chunk base
                    Quaternion.identity,
                    chunkObj.transform
                );

                // You could also set up the island's biome, culture, etc. here using the data
                // For example, change materials, spawn NPCs, etc. based on data.biome, data.culture
            }
            else
            {
                // Fallback: just create a basic ocean chunk with maybe a small island placeholder
                Create Ocean(chunkObj.transform);
            }
        }
        else
        {
            // Just ocean
            Create Ocean(chunkObj.transform);
        }

        loadedChunks[$"{chunkX},{chunkZ}"] = chunkObj;
    }

    /// <summary>
    /// Unloads and destroys the chunk at the given key.
    /// </summary>
    private void UnloadChunk(string key)
    {
        if (loadedChunks.TryGetValue(key, out GameObject chunkObj))
        {
            Destroy(chunkObj);
            loadedChunks.Remove(key);
        }
    }

    /// <summary>
    /// Helper to create a basic ocean plane for a chunk.
    /// </summary>
    private void CreateOcean(Transform parent)
    {
        if (oceanPrefab != null)
        {
            Instantiate(oceanPrefab, parent.position, Quaternion.identity, parent);
        }
        else
        {
            // Create a simple plane as fallback
            GameObject ocean = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ocean.transform.SetParent(parent);
            ocean.transform.localPosition = Vector3.zero;
            ocean.transform.localScale = new Vector3(chunkSize, 1, chunkSize);
            ocean.name = "Ocean";
            // Apply ocean material (would need to be set up)
        }
    }
}
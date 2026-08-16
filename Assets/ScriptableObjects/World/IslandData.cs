using UnityEngine;

/// <summary>
/// ScriptableObject defining an island's biome, culture, faction, and visual/theme data.
/// Create assets via right-click -> Create -> World -> Island Data
/// </summary>
[CreateAssetMenu(fileName = "NewIslandData", menuName = "World/Island Data", order = 1)]
public class IslandData : ScriptableObject
{
    [Header("Basic Info")]
    public string islandName = "Unnamed Island";
    public Sprite mapIcon; // For minimap
    public GameObject islandPrefab; // The prefab to instantiate for this island

    [Header("Biome")]
    public BiomeType biome;
    public float temperature; // 0-1 scale
    public float humidity; // 0-1 scale
    public TerrainType terrainType;

    [Header("Culture & Faction")]
    public Faction controllingFaction;
    public CultureType culture;
    public int difficultyLevel; // 1-10, affects enemy strength, loot

    [Header("Spawns")]
    public GameObject[] enemySpawns;
    public GameObject[] npcSpawns;
    public GameObject[] resourceNodes;
    public GameObject[] pointsOfInterest; // e.g., towns, dungeons

    [Header("Audio")]
    public AudioClip ambientMusic;
    public AudioClip[] ambientSFX;

    // Enums for categorization
    public enum BiomeType { Tropical, Desert, Arctic, Jungle, Volcanic, Swamp, Mountain }
    public enum TerrainType { Flat, Hilly, Rocky, Sandy, Forested }
    public enum Faction { Neutral, Pirates, Marines, Revolutionaries, AncientKingdom }
    public enum CultureType { none, Oriental, Western, Tribal, Futuristic, Ancient }
}
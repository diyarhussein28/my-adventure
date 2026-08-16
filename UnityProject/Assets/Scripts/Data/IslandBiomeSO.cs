using UnityEngine;

namespace SeasOfLegends.Data
{
    /// <summary>
    /// ScriptableObject defining island biome properties for procedural generation.
    /// </summary>
    [CreateAssetMenu(fileName = "NewIslandBiome", menuName = "SeasOfLegends/Island Biome")]
    public class IslandBiomeSO : ScriptableObject
    {
        [Header("Biome Identity")]
        public string biomeName = "Tropical Island";
        public string factionName = "Pirate Republic";
        public Sprite biomeIcon;
        public Color mapColor = Color.green;

        [Header("Terrain Generation")]
        [Tooltip("Base terrain height multiplier")]
        public float terrainHeightScale = 15f;

        [Tooltip("Noise frequency for terrain detail")]
        public float terrainFrequency = 0.03f;

        [Tooltip("Number of octaves in terrain noise")]
        public int terrainOctaves = 4;

        [Tooltip("Size of the island in world units")]
        public float islandRadius = 200f;

        [Tooltip("Maximum elevation of mountains")]
        public float maxElevation = 80f;

        [Header("Vegetation")]
        public GameObject[] treePrefabs;
        public int treeDensity = 150;
        public GameObject[] bushPrefabs;
        public int bushDensity = 300;

        [Header("Structures")]
        public GameObject[] buildingPrefabs;
        public int buildingCount = 8;
        public GameObject portPrefab;
        public bool hasPort = true;

        [Header("Spawns")]
        public GameObject[] enemyPrefabs;
        public int enemyCount = 12;
        public GameObject[] npcPrefabs;
        public int npcCount = 6;
        public GameObject[] wildlifePrefabs;
        public int wildlifeCount = 20;

        [Header("Weather Profile")]
        [Tooltip("Base weather for this biome")]
        public WeatherType defaultWeather = WeatherType.Clear;

        [Tooltip("Chance of storm occurring")]
        [Range(0f, 1f)]
        public float stormChance = 0.15f;

        [Tooltip("Fog density in this biome")]
        [Range(0f, 1f)]
        public float fogDensity = 0.02f;

        [Header("Ambient Audio")]
        public AudioClip ambientDaySFX;
        public AudioClip ambientNightSFX;
        public AudioClip ambientStormSFX;

        [Header("Loot Tables")]
        public LootTable commonLoot;
        public LootTable rareLoot;
        public LootTable legendaryLoot;
    }

    [System.Serializable]
    public class LootTable
    {
        public GameObject[] itemPrefabs;
        [Range(0f, 1f)] public float dropChance = 0.5f;
        public int minDrops = 1;
        public int maxDrops = 3;
    }

    public enum WeatherType
    {
        Clear,
        Cloudy,
        Rain,
        Storm,
        Fog,
        HeatWave,
        Blizzard
    }
}

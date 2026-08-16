using UnityEngine;

namespace SeasOfLegends.Data
{
    [CreateAssetMenu(fileName = "WeaponDefinition", menuName = "Seas of Legends/Combat/Weapon Definition")]
    public sealed class WeaponDefinition : ScriptableObject
    {
        [SerializeField] private string weaponId = "training_blade";
        [SerializeField] private float damageMultiplier = 1f;
        [SerializeField] private ComboDefinition[] supportedCombos;
        [SerializeField] private GameObject weaponTrailVfx;

        public string WeaponId => weaponId;
        public float DamageMultiplier => damageMultiplier;
        public ComboDefinition[] SupportedCombos => supportedCombos;
        public GameObject WeaponTrailVfx => weaponTrailVfx;
    }

    [CreateAssetMenu(fileName = "IslandBiomeDefinition", menuName = "Seas of Legends/World/Island Biome Definition")]
    public sealed class IslandBiomeDefinition : ScriptableObject
    {
        [SerializeField] private string biomeId = "tropical";
        [SerializeField] private Material oceanMaterial;
        [SerializeField] private Material terrainMaterial;
        [SerializeField] private GameObject[] vegetationPrefabs;
        [SerializeField] private Color fogColor = Color.cyan;
        [SerializeField, Range(0f, 1f)] private float stormFrequency = 0.2f;

        public string BiomeId => biomeId;
        public Material OceanMaterial => oceanMaterial;
        public Material TerrainMaterial => terrainMaterial;
        public GameObject[] VegetationPrefabs => vegetationPrefabs;
        public Color FogColor => fogColor;
        public float StormFrequency => stormFrequency;
    }
}

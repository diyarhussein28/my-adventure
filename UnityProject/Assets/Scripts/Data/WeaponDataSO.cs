using UnityEngine;

namespace SeasOfLegends.Data
{
    /// <summary>
    /// ScriptableObject defining weapon properties and its associated combo set.
    /// </summary>
    [CreateAssetMenu(fileName = "NewWeaponData", menuName = "SeasOfLegends/Weapon Data")]
    public class WeaponDataSO : ScriptableObject
    {
        [Header("Weapon Identity")]
        public string weaponName = "Iron Sword";
        public string description = "A balanced blade for close combat.";
        public Sprite weaponIcon;

        [Header("Weapon Type")]
        public WeaponType weaponType = WeaponType.Sword;

        [Header("Base Stats")]
        [Tooltip("Damage multiplier applied to all attacks")]
        public float damageMultiplier = 1f;

        [Tooltip("Range multiplier for hitbox size")]
        public float rangeMultiplier = 1f;

        [Tooltip("Speed multiplier for attack animations")]
        public float speedMultiplier = 1f;

        [Header("Combo Sets")]
        [Tooltip("All combos available with this weapon")]
        public ComboDataSO[] availableCombos;

        [Header("Hitbox Configuration")]
        [Tooltip("Prefab for the weapon's hitbox trigger")]
        public GameObject hitboxPrefab;

        [Tooltip("Size of the hitbox relative to default")]
        public Vector3 hitboxScale = Vector3.one;

        [Tooltip("Offset from weapon grip to hitbox center")]
        public Vector3 hitboxOffset = Vector3.forward * 0.5f;

        [Header("Elemental Infusion")]
        public ElementType defaultElement = ElementType.None;

        [Tooltip("Can this weapon be infused with elements?")]
        public bool canBeInfused = true;

        [Header("VFX & Audio")]
        public GameObject equipVFX;
        public AudioClip equipSFX;
        public AudioClip sheathSFX;
    }

    public enum WeaponType
    {
        Sword,
        Greatsword,
        DualBlades,
        Spear,
        Gauntlets,
        Katana,
        Scythe,
        WarHammer,
        Bow,
        Pistol,
        DevilFruit // One Piece inspired special abilities
    }
}

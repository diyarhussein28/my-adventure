using System;
using UnityEngine;

namespace SeasOfLegends.Data
{
    [CreateAssetMenu(fileName = "IslandDefinition", menuName = "Seas of Legends/World/Island Definition")]
    public sealed class IslandDefinition : ScriptableObject
    {
        [SerializeField] private string islandId = "starter_island";
        [SerializeField] private string biomeName = "Tropical";
        [SerializeField] private GameObject islandPrefab;
        [SerializeField] private Vector3 worldPosition;
        [Min(10f), SerializeField] private float loadRadius = 650f;
        [Min(10f), SerializeField] private float unloadRadius = 750f;

        public string IslandId => islandId;
        public string BiomeName => biomeName;
        public GameObject IslandPrefab => islandPrefab;
        public Vector3 WorldPosition => worldPosition;
        public float LoadRadius => loadRadius;
        public float UnloadRadius => Mathf.Max(unloadRadius, loadRadius + 1f);
    }

    [Serializable]
    public struct QuestObjectiveDefinition
    {
        [TextArea] public string description;
        [Min(1)] public int targetAmount;
        public string targetId;
    }

    [CreateAssetMenu(fileName = "QuestDefinition", menuName = "Seas of Legends/Quests/Quest Definition")]
    public sealed class QuestDefinition : ScriptableObject
    {
        [SerializeField] private string questId = "first_voyage";
        [SerializeField] private string title = "First Voyage";
        [TextArea, SerializeField] private string description;
        [SerializeField] private QuestObjectiveDefinition[] objectives;
        [SerializeField] private QuestDefinition[] prerequisites;

        public string QuestId => questId;
        public string Title => title;
        public string Description => description;
        public QuestObjectiveDefinition[] Objectives => objectives;
        public QuestDefinition[] Prerequisites => prerequisites;
    }
}

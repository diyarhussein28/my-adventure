using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject representing a quest definition.
/// Create assets via right-click -> Create -> Quest -> Quest Data
/// </summary>
[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest/Quest Data", order = 1)]
public class QuestData : ScriptableObject
{
    [Header("Quest Info")]
    public string questName = "Unnamed Quest";
    [TextArea] public string description;
    public Sprite questIcon;
    public XP_REWARD xpReward;
    public int goldReward;
    public List<ItemReward> itemRewards;

    [Header("Objectives")]
    public List<ObjectiveData> objectives = new List<ObjectiveData>();

    [Header("Prerequisites")]
    public List<QuestData> requiredQuests; // Must complete these first

    [Header("Completion")]
    public bool canRepeat = false;
    public float repeatDelay = 1f; // In days

    // Enums for reward types
    [System.Serializable]
    public struct XP_REWARD { public int amount; public bool isMultiplier; }
    [System.Serializable]
    public struct ItemReward { public Item item; public int amount; }
}

// Abstract base for objective data (to be extended for each objective type)
public abstract class ObjectiveData : ScriptableObject
{
    public string description;
    public int amountRequired = 1; // e.g., kill 5 enemies
    public bool isOptional = false;

    public abstract string GetProgressString(int currentAmount);
}

// Example objective types (create more as needed)
[CreateAssetMenu(fileName = "NewKillObjective", menuName = "Quest/Objective/Kill", order = 1)]
public class KillObjectiveData : ObjectiveData
{
    public EnemyType enemyToKill;

    public enum EnemyType { Generic, Boss, Specific }
    public SpecificEnemy specificEnemy; // If Specific is chosen

    public override string GetProgressString(int currentAmount)
    {
        return $"Defeat {enemyToKill}: {currentAmount}/{amountRequired}";
    }
}

[CreateAssetMenu(fileName = "NewCollectObjective", menuName = "Quest/Objective/Collect", order = 1)]
public class CollectObjectiveData : ObjectiveData
{
    public Item itemToCollect;

    public override string GetProgressString(int currentAmount)
    {
        return $"Collect {itemToCollect.itemName}: {currentAmount}/{amountRequired}";
    }
}

[CreateAssetMenu(fileName = "NewTalkObjective", menuName = "Quest/Objective/Talk", order = 1)]
public class TalkObjectiveData : ObjectiveData
{
    public NPC npcToTalk;

    public override string GetProgressString(int currentAmount)
    {
        return $"Talk to {npcToTalk.npcName}: {currentAmount}/{amountRequired}";
    }
}

// Placeholder classes for Item and NPC (would be defined elsewhere)
[System.Serializable] public class Item { public string itemName; public Sprite icon; }
[System.Serializable] public class NPC { public string npcName; public Sprite portrait; }
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player's quest log: active, completed, and failed quests.
/// Handles quest progression, objective updates, and rewards.
/// Should be a singleton or attached to a persistent GameObject.
/// </summary>
public class QuestManager : MonoBehaviour
{
    // Singleton pattern for easy access
    public static QuestManager Instance { get; private set; }

    // Serialized fields for setting up in inspector
    [Header("References")]
    public GameObject questUIPrefab; // Reference to a UI prefab to display quests

    [Header("Debug")]
    public List<QuestData> startingQuests; // Quests to start with (for testing)

    // Internal tracking
    private class ActiveQuest
    {
        public QuestData data;
        public Dictionary<ObjectiveData, int> objectiveProgress = new Dictionary<ObjectiveData, int>();
        public bool isCompleted = false;
        public bool isFailed = false;
        public float timeAccepted;
    }

    private List<ActiveQuest> activeQuests = new List<ActiveQuest>();
    private List<QuestData> completedQuests = new List<QuestData>();
    private List<QuestData> failedQuests = new List<QuestData>();

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Accept starting quests for debugging
        foreach (var quest in startingQuests)
        {
            AcceptQuest(quest);
        }
    }

    /// <summary>
    /// Accepts a quest, adding it to the active quest list.
    /// </summary>
    public void AcceptQuest(QuestData questData)
    {
        // Check if quest is already active or completed (if not repeatable)
        if (IsQuestActive(questData) || (completedQuests.Contains(questData) && !questData.canRepeat))
        {
            Debug.LogWarning($"Quest '{questData.questName}' cannot be accepted again.");
            return;
        }

        // Check prerequisites
        foreach (var req in questData.requiredQuests)
        {
            if (!completedQuests.Contains(req))
            {
                Debug.LogWarning($"Prerequisite quest '{req.questName}' not completed.");
                return;
            }
        }

        ActiveQuest newQuest = new ActiveQuest
        {
            data = questData,
            timeAccepted = Time.time
        };

        // Initialize objective progress
        foreach (var obj in questData.objectives)
        {
            newQuest.objectiveProgress[obj] = 0;
        }

        activeQuests.Add(newQuest);
        Debug.Log($"Quest accepted: {questData.questName}");

        // TODO: Show quest accepted UI
        UpdateQuestUI();
    }

    /// <summary>
    /// Called by gameplay systems to update progress on an objective.
    /// </summary>
    /// <param name="objective">The objective definition to update</param>
    /// <param name="amount">How much to increment progress by (usually 1)</param>
    public void UpdateObjective(ObjectiveData objective, int amount = 1)
    {
        // Find all active quests that contain this objective
        foreach (var quest in activeQuests)
        {
            if (quest.data.objectives.Contains(objective) && !quest.isCompleted)
            {
                quest.objectiveProgress[objective] += amount;
                Debug.Log($"Quest '{quest.data.questName}' objective updated: {objective.description} progress {quest.objectiveProgress[objective]}/{GetObjectiveTarget(quest, objective)}");

                // Check if this quest is now complete
                if (IsQuestComplete(quest))
                {
                    CompleteQuest(quest);
                }

                // Update UI
                UpdateQuestUI();
                break; // Assuming each objective belongs to one quest at a time (adjust if shared)
            }
        }
    }

    /// <summary>
    /// Returns the target amount for an objective in a given quest.
    /// </summary>
    private int GetObjectiveTarget(ActiveQuest quest, ObjectiveData objective)
    {
        // Find the objective in the quest's list to get its amountRequired
        foreach (var obj in quest.data.objectives)
        {
            if (obj == objective)
                return obj.amountRequired;
        }
        return 0;
    }

    /// <summary>
    /// Checks if all objectives of a quest are satisfied.
    /// </summary>
    private bool IsQuestComplete(ActiveQuest quest)
    {
        foreach (var kvp in quest.objectiveProgress)
        {
            if (kvp.Value < GetObjectiveTarget(quest, kvp.Key))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Completes a quest, gives rewards, and moves it to completed list.
    /// </summary>
    private void CompleteQuest(ActiveQuest quest)
    {
        if (quest.isCompleted) return;

        quest.isCompleted = true;
        Debug.Log($"Quest completed: {quest.data.questName}");

        // Give rewards
        GiveRewards(quest.data);

        // Move to completed list
        completedQuests.Add(quest.data);
        activeQuests.Remove(quest);

        // TODO: Show quest completion UI and rewards
        UpdateQuestUI();
    }

    /// <summary>
    /// Gives the rewards defined in the quest data.
    /// </summary>
    private void GiveRewards(QuestData questData)
    {
        // XP
        // Find player's experience system and add xpReward.amount
        // Example: PlayerExperience.Instance.AddXP(questData.xpReward.amount);

        // Gold
        // Example: CurrencyManager.Instance.AddGold(questData.goldReward);

        // Items
        // Example: foreach (var reward in questData.itemRewards) { Inventory.Instance.AddItem(reward.item, reward.amount); }

        Debug.Log($"Rewards given: {questData.xpReward.amount} XP, {questData.goldReward} Gold, {questData.itemRewards.Count} items");
    }

    /// <summary>
    /// Checks if a quest is currently active.
    /// </summary>
    public bool IsQuestActive(QuestData questData)
    {
        return activeQuests.Exists(q => q.data == questData);
    }

    /// <summary>
    /// Checks if a quest has been completed.
    /// </summary>
    public bool IsQuestCompleted(QuestData questData)
    {
        return completedQuests.Contains(questData);
    }

    /// <summary>
    /// Forces a quest to fail (optional).
    /// </summary>
    public void FailQuest(QuestData questData)
    {
        ActiveQuest quest = activeQuests.Find(q => q.data == questData);
        if (quest == null) return;

        quest.isFailed = true;
        Debug.Log($"Quest failed: {questData.questName}");
        activeQuests.Remove(quest);
        failedQuests.Add(questData);
        UpdateQuestUI();
    }

    /// <summary>
    /// Updates the quest UI (if any) to reflect current quest state.
    /// </summary>
    private void UpdateQuestUI()
    {
        // If you have a quest UI system, update it here.
        // Example: QuestUI.Instance.RefreshQuestList(activeQuests, completedQuests);
    }

    // --- Getter methods for UI or other systems ---
    public List<ActiveQuest> GetActiveQuests() => new List<ActiveQuest>(activeQuests);
    public List<QuestData> GetCompletedQuests() => new List<QuestData>(completedQuests);
    public List<QuestData> GetFailedQuests() => new List<QuestData>(failedQuests);
}
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor script to create sample ScriptableObjects for IslandData and QuestData.
/// Use via menu: Assets -> Create Sample Game Data
/// </summary>
public static class CreateSampleAssets
{
    [MenuItem("Assets/Create Sample Game Data/Island Data (Tropical)", false, 100)]
    public static void CreateTropicalIslandData()
    {
        var island = ScriptableObject.CreateInstance<IslandData>();
        island.islandName = "Tropical Paradise";
        island.biome = IslandData.BiomeType.Tropical;
        island.temperature = 0.9f;
        island.humidity = 0.8f;
        island.terrainType = IslandData.TerrainType.Sandy;
        island.controllingFaction = IslandData.Faction.Neutral;
        island.culture = IslandData.CultureType.Tribal;
        island.difficultyLevel = 2;
        // Assign a default prefab reference (user must set)
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/ScriptableObjects/World/Islands/TropicalIsland.asset");
        AssetDatabase.CreateAsset(island, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = island;
    }

    [MenuItem("Assets/Create Sample Game Data/Island Data (Volcanic)", false, 100)]
    public static void CreateVolcanicIslandData()
    {
        var island = ScriptableObject.CreateInstance<IslandData>();
        island.islandName = "Volcano Isle";
        island.biome = IslandData.BiomeType.Volcanic;
        island.temperature = 0.95f;
        island.humidity = 0.3f;
        island.terrainType = IslandData.TerrainType.Rocky;
        island.controllingFaction = IslandData.Faction.Pirates;
        island.culture = IslandData.CultureType.Oriental;
        island.difficultyLevel = 5;
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/ScriptableObjects/World/Islands/VolcanicIsland.asset");
        AssetDatabase.CreateAsset(island, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = island;
    }

    [MenuItem("Assets/Create Sample Game Data/Quest Data (Tutorial)", false, 200)]
    public static void CreateTutorialQuest()
    {
        var quest = ScriptableObject.CreateInstance<QuestData>();
        quest.questName = "Tutorial: Learn to Fight";
        quest.description = "Defeat 3 training dummies to learn the basics of combat.";
        quest.xpReward.amount = 50;
        quest.goldReward = 100;
        // Item rewards empty for now

        // Create objective
        var killObj = ScriptableObject.CreateInstance<KillObjectiveData>();
        killObj.description = "Defeat training dummies";
        killObj.amountRequired = 3;
        killObj.enemyToKill = KillObjectiveData.EnemyType.Generic;
        // In a real project, you'd save this as its own asset and reference it; for simplicity we embed?
        // Since QuestData expects List<ObjectiveData>, we need to have the objective as an asset.
        // We'll save it as a separate asset and then assign.
        string objPath = AssetDatabase.GenerateUniqueAssetPath("Assets/ScriptableObjects/Quest/Objectives/Kill_TrainingDummy.asset");
        AssetDatabase.CreateAsset(killObj, objPath);
        AssetDatabase.SaveAssets();

        quest.objectives = new System.Collections.Generic.List<ObjectiveData> { killObj };

        string questPath = AssetDatabase.GenerateUniqueAssetPath("Assets/ScriptableObjects/Quest/Quests/TutorialQuest.asset");
        AssetDatabase.CreateAsset(quest, questPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = quest;
    }
}
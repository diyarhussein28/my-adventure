using System;
using System.Collections.Generic;
using UnityEngine;
using SeasOfLegends.Core;
using SeasOfLegends.Data;

namespace SeasOfLegends.Quest
{
    public enum QuestState { Inactive, Active, Completed, Failed }

    [Serializable]
    public sealed class QuestProgress
    {
        public QuestDefinition Definition { get; private set; }
        public QuestState State { get; private set; }
        public int[] ObjectiveAmounts { get; private set; }

        public QuestProgress(QuestDefinition definition)
        {
            Definition = definition;
            State = QuestState.Inactive;
            ObjectiveAmounts = new int[definition.Objectives == null ? 0 : definition.Objectives.Length];
        }

        public void Activate() => State = QuestState.Active;
        public void Fail() => State = QuestState.Failed;
        public void Advance(int objectiveIndex, int amount)
        {
            if (State != QuestState.Active || objectiveIndex < 0 || objectiveIndex >= ObjectiveAmounts.Length) return;
            ObjectiveAmounts[objectiveIndex] = Mathf.Clamp(ObjectiveAmounts[objectiveIndex] + amount, 0, Definition.Objectives[objectiveIndex].targetAmount);
            for (int i = 0; i < ObjectiveAmounts.Length; i++)
                if (ObjectiveAmounts[i] < Definition.Objectives[i].targetAmount) return;
            State = QuestState.Completed;
        }
    }

    /// <summary>
    /// Scene-level narrative coordinator. NPC dialogue components can subscribe to DialogueRequested
    /// and the gameplay layer may call AdvanceObjectives with semantic target ids such as "navy_scout".
    /// </summary>
    public sealed class QuestManager : MonoBehaviour
    {
        [SerializeField] private QuestDefinition[] questCatalog;
        private readonly Dictionary<string, QuestProgress> progressById = new Dictionary<string, QuestProgress>();
        public event Action<string> DialogueRequested;

        private void Awake()
        {
            if (questCatalog == null) return;
            foreach (QuestDefinition definition in questCatalog)
                if (definition != null && !progressById.ContainsKey(definition.QuestId)) progressById.Add(definition.QuestId, new QuestProgress(definition));
        }

        public bool TryAccept(string questId)
        {
            if (!progressById.TryGetValue(questId, out QuestProgress progress) || progress.State != QuestState.Inactive) return false;
            QuestDefinition[] prerequisites = progress.Definition.Prerequisites;
            if (prerequisites != null)
            {
                foreach (QuestDefinition prerequisite in prerequisites)
                    if (prerequisite != null && (!progressById.TryGetValue(prerequisite.QuestId, out QuestProgress required) || required.State != QuestState.Completed)) return false;
            }
            progress.Activate();
            DialogueRequested?.Invoke("quest_accept_" + questId);
            return true;
        }

        public void AdvanceObjectives(string targetId, int amount = 1)
        {
            foreach (QuestProgress progress in progressById.Values)
            {
                if (progress.State != QuestState.Active) continue;
                QuestObjectiveDefinition[] objectives = progress.Definition.Objectives;
                if (objectives == null) continue;
                for (int i = 0; i < objectives.Length; i++)
                {
                    if (!string.Equals(objectives[i].targetId, targetId, StringComparison.OrdinalIgnoreCase)) continue;
                    QuestState before = progress.State;
                    progress.Advance(i, amount);
                    if (before != QuestState.Completed && progress.State == QuestState.Completed)
                    {
                        GameEvents.RaiseQuestCompleted(progress.Definition.QuestId);
                        DialogueRequested?.Invoke("quest_complete_" + progress.Definition.QuestId);
                    }
                }
            }
        }

        public bool TryGetProgress(string questId, out QuestProgress progress) => progressById.TryGetValue(questId, out progress);
        public void RequestDialogue(string dialogueKey) => DialogueRequested?.Invoke(dialogueKey);
    }
}

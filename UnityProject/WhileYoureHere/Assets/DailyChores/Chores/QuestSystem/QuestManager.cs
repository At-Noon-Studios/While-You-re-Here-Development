using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private List<SO_Quest> questsToLoad = new List<SO_Quest>();

    public Dictionary<int, Quest> Quests;

    private void Awake()
    {
        InitializeQuests();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartQuest(0); // Starting first quest
    }

    public bool StartQuest(int id)
    {
        if (!Quests.ContainsKey(id)) return false;
        if (Quests[id].QuestStatus != QuestStatus.Inactive) return false;
        
        Quests[id].Activate();
        Quests[id].OnQuestCompleted += QuestCompleted;

        Debug.Log($"{Quests[id].QuestName} has been started");
        return true;
    }

    private void QuestCompleted(Quest quest)
    {
        Debug.Log($"{quest.QuestName} has been completed");
        quest.OnQuestCompleted -= QuestCompleted;

        QuestEvents.TriggerQuestCompleted(quest);
    }

    private void InitializeQuests()
    {
        if (questsToLoad.Count <= 0) return;
        
        Quests = new Dictionary<int, Quest>();

        for (var i = 0; i < questsToLoad.Count; i++)
        {
            SO_Quest questToLoad = questsToLoad[i];
            Quest quest = new Quest(questToLoad.questName, questToLoad.id, questToLoad.components);
            Quests.Add(quest.QuestID, quest);
                
            Debug.Log($"{quest.QuestName} has been initialized");
        }
    }
}

using System;

public static class QuestEvents
{
    public static event Action<int> OnEnemyKilled;
    public static void TriggerEnemyKilled(int enemyID)
    {
        OnEnemyKilled?.Invoke(enemyID);
    }
    
    public static event Action<int> OnItemCollected;
    public static void TriggerItemCollected(int itemID)
    {
        OnItemCollected?.Invoke(itemID);
    }
    
    public static event Action<Quest> OnQuestCompleted;
    public static void TriggerQuestCompleted(Quest quest)
    {
        OnQuestCompleted?.Invoke(quest);
    }
}

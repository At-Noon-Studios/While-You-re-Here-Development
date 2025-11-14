using UnityEngine;

public class QC_EnemyKilled : QuestComponent
{
    private int _enemyID;
    private int _killCount;
    private int _killsNeeded;
    
    public QC_EnemyKilled(string name, string description, int enemyID, int killsNeeded) : base(name, description)
    {
        _enemyID = enemyID;
        _killsNeeded = killsNeeded;
        _killCount = 0;
        ComponentType = QuestComponentType.EnemyKilled;
    }
        
    // Is used in dictionary and Quest constructor
    public static QuestComponent CreateFactory(SO_QuestComponent so_questComponent) // Accept ScriptableObject data
    {
        SO_QC_EnemyKilled localQuestComponent = so_questComponent as SO_QC_EnemyKilled; // Check type

        if (localQuestComponent == null)
        {
            Debug.LogError($"Factory Error: Wrong SO type passed to QC_EnemyKilled.CreateFactory");
            return null;
        }
        
        return new QC_EnemyKilled( // Create runtime component
            localQuestComponent.componentName,
            localQuestComponent.description,
            localQuestComponent.enemyID,
            localQuestComponent.killsNeeded);
    }

    public override void EnableComponent()
    {
        base.EnableComponent();
        // Subscribe to enemy kill event
        QuestEvents.OnEnemyKilled += EnemyKilled;
    }

    public override void MarkCompleted()
    {
        base.MarkCompleted();
        // Unsubscribe from enemy kill event
        QuestEvents.OnEnemyKilled -= EnemyKilled;
    }

    private void EnemyKilled(int enemyID)
    {
        if (_enemyID != enemyID)
            return;
        
        _killCount++;
        
        Debug.unityLogger.Log($"{ComponentName}: Enemy Type {enemyID} was killed {_killCount}/{_killsNeeded}");

        if (_killCount < _killsNeeded)
            return;
        
        MarkCompleted();
        TriggerComponentCompleted(this);
    }
}

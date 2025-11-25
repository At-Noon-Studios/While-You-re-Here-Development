using System;

namespace chore
{
    public static class ChoreEvents
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

        public static event Action<Chore> OnChoreCompleted;

        public static void TriggerQuestCompleted(Chore chore)
        {
            OnChoreCompleted?.Invoke(chore);
        }

        // public static event Action<int> OnPlantsCollected;

        // public static void TriggerPlantCollected(int plantsID)
        // {
        //     OnPlantsCollected?.Invoke(plantsID);
        // }
    }
}

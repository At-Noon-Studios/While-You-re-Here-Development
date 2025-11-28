using System;

namespace chore
{
    public static class ChoreEvents
    {
        public static event Action<Chore> OnChoreCompleted;
        public static void TriggerQuestCompleted(Chore chore) => OnChoreCompleted?.Invoke(chore);
        
        public static event Action<int> OnWateringCanPickedUp;
        public static void TriggerWateringCanPickedUp(int wateringCanID) => OnWateringCanPickedUp?.Invoke(wateringCanID);
        
        public static event Action<int> OnPlantWatered;
        public static void TriggerPlantWatered(int plantID) => OnPlantWatered?.Invoke(plantID);
        
        public static event Action OnKettleFilled;
        public static void TriggerKettleFilled() => OnKettleFilled?.Invoke();
        
        public static event Action OnWaterBoiled;
        public static void TriggerWaterBoiled() => OnWaterBoiled?.Invoke();

        public static event Action OnTeabagAdded;
        public static void TriggerTeabagAdded() => OnTeabagAdded?.Invoke();

        public static event Action OnCupFilled;
        public static void TriggerCupFilled() => OnCupFilled?.Invoke();

        public static void TriggerChoreCompleted(Chore chore)
        {
            OnChoreCompleted?.Invoke(chore);
        }
    }
}

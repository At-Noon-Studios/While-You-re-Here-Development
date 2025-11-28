using ScriptableObjects.chores;
using UnityEngine;

namespace component.gardening
{
    [CreateAssetMenu(fileName = "CcWateringCanPickedUp", menuName = "QuestSystem/Components/CcWateringCanPickedUp", order = 1)]
    public class SoCcWateringCanPickedUp : SoChoreComponent
    {
        [Header("WateringCan settings")]
        public int wateringCanID;
        public int wateringCanAmountNeeded;
    }
}

using UnityEngine;

namespace component
{
    [CreateAssetMenu(fileName = "CcItemCollected", menuName = "QuestSystem/Components/CcItemCollected", order = 2)]
    public class SoCcItemCollected : SoChoreComponent
    {
        [Header("Item Settings")]
        public int itemID;
        public int itemAmountNeeded;
    }
}

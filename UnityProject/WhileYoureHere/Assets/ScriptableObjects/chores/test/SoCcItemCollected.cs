using UnityEngine;

namespace ScriptableObjects.chores.test
{
    [CreateAssetMenu(fileName = "CcItemCollected", menuName = "ChoreSystem/Components/CcItemCollected", order = 2)]
    public class SoCcItemCollected : SoChoreComponent
    {
        [Header("Item Settings")]
        public int itemID;
        public int itemAmountNeeded;
    }
}

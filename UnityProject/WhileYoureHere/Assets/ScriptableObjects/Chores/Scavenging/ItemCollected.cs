using ScriptableObjects.chores;
using UnityEngine;

namespace ScriptableObjects.Chores.scavenging
{
    [CreateAssetMenu(fileName = "CcItemCollected", menuName = "ChoreSystem/Components/CcItemCollected", order = 2)]
    public class SoCcItemCollected : SoChoreComponent
    {
        [Header("Item Settings")]
        public int itemID;
        public int itemAmountNeeded;
    }
}
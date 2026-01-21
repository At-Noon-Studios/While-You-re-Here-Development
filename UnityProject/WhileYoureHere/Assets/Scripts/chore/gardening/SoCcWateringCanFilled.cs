using ScriptableObjects.chores;
using UnityEngine;

namespace component.gardening
{
    [CreateAssetMenu(fileName="CcWateringCanFilled", menuName="ChoreSystem/Components/CcWateringCanFilled", order=2)]
    public class SoCcWateringCanFilled : SoChoreComponent
    {
        [Header("Filled settings")] 
        public int wateringCanID;
        public int requiredFillAmount;
    }
}

using UnityEngine;

namespace component
{
    [CreateAssetMenu(fileName = "CcPlantsCollected", menuName = "QuestSystem/Components/CcPlantsKilled", order = 3)]
    public class SoCCPlantsCollected : SoChoreComponent
    {
        [Header("Plant Settings")]
        public int plantsID;
        public int plantsNeeded;
    }
}

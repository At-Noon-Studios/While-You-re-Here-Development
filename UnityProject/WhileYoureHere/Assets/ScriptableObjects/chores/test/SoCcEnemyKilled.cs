using UnityEngine;

namespace component
{
    [CreateAssetMenu(fileName = "CcEnemyKilled", menuName = "QuestSystem/Components/CcEnemyKilled", order = 1)]
    public class SoCcEnemyKilled : SoChoreComponent
    {
        [Header("Enemy Settings")]
        public int enemyID;
        public int killsNeeded;
    }
}

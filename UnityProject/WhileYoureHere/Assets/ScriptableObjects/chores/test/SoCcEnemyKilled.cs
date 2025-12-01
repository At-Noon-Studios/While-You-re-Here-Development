using UnityEngine;

namespace ScriptableObjects.chores.test
{
    [CreateAssetMenu(fileName = "CcEnemyKilled", menuName = "ChoreSystem/Components/CcEnemyKilled", order = 1)]
    public class SoCcEnemyKilled : SoChoreComponent
    {
        [Header("Enemy Settings")]
        public int enemyID;
        public int killsNeeded;
    }
}

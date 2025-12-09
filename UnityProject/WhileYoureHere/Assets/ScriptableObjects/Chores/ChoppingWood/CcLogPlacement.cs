using UnityEngine;

namespace ScriptableObjects.chores.ChoppingWood
{
    [CreateAssetMenu(fileName = "CcPickingUpLog", menuName = "ScriptableObjects/chores/CcLogPlacement")]
    public class CcLogPlacement : SoChoreComponent
    {
        [Header("Log settings")]
        public int logID;
        public int logAmountNeeded;
        public int logCount;
    }
}
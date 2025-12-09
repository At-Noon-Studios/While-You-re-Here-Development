using UnityEngine;

namespace ScriptableObjects.chores.ChoppingWood
{
    [CreateAssetMenu(fileName = "CcPickingUpLog", menuName = "ScriptableObjects/chores/CcLogChopped")]
    public class CcLogChopped : SoChoreComponent
    {
        [Header("Log settings")] public int logID;
        public int logAmountNeeded;
        public int logCount;
    }
}
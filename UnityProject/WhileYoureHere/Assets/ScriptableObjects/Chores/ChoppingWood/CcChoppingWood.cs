using UnityEngine;

namespace ScriptableObjects.chores.ChoppingWood
{
    [CreateAssetMenu(fileName = "CcChoppingWood", menuName = "ScriptableObjects/chores/ChoppingWood")]
    public class CcChoppingWood : SoChoreComponent
    {
        [Header("Log settings")] 
        public int logID;
        public int logAmountNeeded;
        public int logCount;
    }
}
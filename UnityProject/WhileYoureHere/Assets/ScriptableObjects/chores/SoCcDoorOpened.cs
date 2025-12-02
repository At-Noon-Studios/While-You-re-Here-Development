using UnityEngine;

namespace ScriptableObjects.chores
{
    [CreateAssetMenu(fileName = "CcDoorOpened", menuName = "ChoreSystem/Components/CcDoorOpened", order = 1)]
    public class SoCcDoorOpened : SoChoreComponent
    {
        [Header("Door settings")]
        public int doorID;
        public int doorsOpenedNeeded;
    }
}

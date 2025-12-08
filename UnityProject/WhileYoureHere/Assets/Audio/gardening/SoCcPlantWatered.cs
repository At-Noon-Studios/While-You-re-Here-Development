using ScriptableObjects.chores;
using UnityEngine;

namespace component.gardening
{
    [CreateAssetMenu(fileName = "CcPlantWatered", menuName = "ChoreSystem/Components/CcPlantWatered", order = 3)]
    public class SoCcPlantWatered : SoChoreComponent
    {
        [Header("Plant settings")]
        public int plantID;
        public int plantStageNeeded;
        
    }
}

using System.Collections.Generic;
using ScriptableObjects.chores;
using UnityEngine;

namespace component.waterPlants
{
    [CreateAssetMenu(fileName = "CcPlantWatered", menuName = "QuestSystem/Components/CcPlantWatered", order = 2)]
    public class SoCcPlantWatered : SoChoreComponent
    {
        [Header("Plant settings")]
        public int plantID;
        public int plantStageNeeded;
        
    }
}

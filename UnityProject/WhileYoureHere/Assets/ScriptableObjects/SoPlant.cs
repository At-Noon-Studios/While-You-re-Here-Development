using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "SoPlant", menuName = "ScriptableObjects/SoPlant")]
    public class SoPlant : ScriptableObject
    {
        public string plantName;
        public List<GameObject> plantPrefabs;

        public float wateringTime = 3;
        
        public int MaxStage => plantPrefabs.Count;

        public GameObject GetPlantByStage(int stage)
        {
            if (stage >= MaxStage)
            {
                return null;
            }
            return plantPrefabs[stage];
        }
    }
}

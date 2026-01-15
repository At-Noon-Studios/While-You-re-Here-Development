using Fishing;
using UnityEngine;

namespace ScriptableObjects.Fishing {
    
    [CreateAssetMenu(fileName = "Fish", menuName = "ScriptableObjects/Fish")]
    public class SoFish : ScriptableObject
    {
        [Header("Fish settings")]
        public string fishName;
        public GameObject fishPrefab;
        public SoCatchDifficulty fishCatchDifficulty;
        public int fishFindProbability;
    }
}

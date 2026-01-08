using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ScriptableObjects.Fishing;
using UnityEngine;

namespace Fishing {
    public class FishingArea : MonoBehaviour
    {

        [SerializeField] private List<SoFish> fishPrefabs;
        
        public SoFish GetFish()
        {
            var rand = Random.Range(0f, 100f);
            float probabilityIncrease = 0;
            foreach (var f in fishPrefabs)
            {
                if (rand <= f.fishFindProbability + probabilityIncrease) return f;
                probabilityIncrease += f.fishFindProbability;
            }
            return null;
        }
    }
}

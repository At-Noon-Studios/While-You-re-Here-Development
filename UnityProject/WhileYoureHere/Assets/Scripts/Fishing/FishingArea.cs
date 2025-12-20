using System.Collections.Generic;
using UnityEngine;

namespace Fishing {
    public class FishingArea : MonoBehaviour
    {

        [SerializeField] private List<GameObject> fishPrefabs;

        public GameObject GetFish()
        {
            return fishPrefabs[Random.Range(0, fishPrefabs.Count)];
        }
    }
}

using Fishing;
using UnityEngine;

namespace ScriptableObjects.Fishing {
    
    [CreateAssetMenu(fileName = "Fish", menuName = "ScriptableObjects/CatchDifficulty")]
    public class SoCatchDifficulty : ScriptableObject
    {
        [Header("Difficulty settings")]
        public string difficultyName;
        public float splashInterval;
        public float splashDuration;
        public float sidewaysMovement;
    }
}
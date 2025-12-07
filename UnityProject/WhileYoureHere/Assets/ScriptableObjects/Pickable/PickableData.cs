using UnityEngine;

namespace ScriptableObjects.Pickable
{
    [CreateAssetMenu(fileName = "PickableData", menuName = "ScriptableObjects/PickableData")]
    public class PickableData : ScriptableObject
    {
        [Range(1f, 100f)]
        public float weight;
    }
}
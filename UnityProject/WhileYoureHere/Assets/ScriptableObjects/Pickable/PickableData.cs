using UnityEngine;

namespace ScriptableObjects.picking_up_objects
{
    [CreateAssetMenu(fileName = "PickableData", menuName = "Scriptable Objects/PickableData")]
    public class PickableData : ScriptableObject
    {
        [Range(1f, 100f)]
        public float weight;
    }
}
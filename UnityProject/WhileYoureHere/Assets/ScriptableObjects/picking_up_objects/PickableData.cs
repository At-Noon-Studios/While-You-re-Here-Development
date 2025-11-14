using UnityEngine;

namespace ScriptableObjects.picking_up_objects
{
    [CreateAssetMenu(fileName = "PickableData", menuName = "Scriptable Objects/PickableData")]
    public class PickableData : ScriptableObject
    {
        public float weight = 10f;
    }
}
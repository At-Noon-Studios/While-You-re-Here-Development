using UnityEngine;

namespace ScriptableObjects.Interactable.Holdable
{
    [CreateAssetMenu(fileName = "HoldableObject",  menuName = "ScriptableObjects/Interaction/HoldableObject")]
    public class HoldableObjectData : ScriptableObject
    {
        [Range(0, 100)]
        [SerializeField] private float weight = 50f;
        [SerializeField] private Vector3 offset = Vector3.zero;
        [SerializeField] private Vector3 rotation = Vector3.zero;
        [SerializeField] private float droppingForce = 400f;

        public float Weight => weight;
        public Vector3 Offset => offset;
        public Vector3 Rotation => rotation;
        public float DroppingForce => droppingForce;
    }
}





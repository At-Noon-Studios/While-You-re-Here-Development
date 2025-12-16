using UnityEngine;

namespace ScriptableObjects.Interactable.Holdable
{
    [CreateAssetMenu(fileName = "HoldableObject",  menuName = "ScriptableObjects/Interaction/HoldableObject")]
    public class HoldableObjectData : ScriptableObject
    {
        [SerializeField, Range(0, 100)] private float weight = 50f;
        [SerializeField, Tooltip("The offset that will be applied relative to the 'hold point' when holding this object")] private Vector3 holdingOffset = Vector3.zero;
        [SerializeField, Tooltip("The rotation that will be applied relative to the 'hold point' when holding this object")] private Vector3 holdingRotation = Vector3.zero;
        [SerializeField] private float droppingForce = 400f;
        [SerializeField] private GameObject holdingPrefab;

        public float Weight => weight;
        public Vector3 HoldingOffset => holdingOffset;
        public Vector3 HoldingRotation => holdingRotation;
        public float DroppingForce => droppingForce;
        public GameObject HoldingPrefab => holdingPrefab;
    }
}
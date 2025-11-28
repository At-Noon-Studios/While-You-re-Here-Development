using Interactable.Holdable;
using UnityEngine;

namespace ScriptableObjects.Interactable
{
    [CreateAssetMenu(fileName = "HoldableObject",  menuName = "ScriptableObjects/HoldableObject")]
    public class HoldableObjectData : ScriptableObject
    {
        [SerializeField, Range(0, 100)] private float weight = 50f;
        [SerializeField] private Vector3 offset = Vector3.zero;
        [SerializeField] private Vector3 rotation = Vector3.zero;
        [SerializeField] private float droppingForce = 400f;
        [SerializeField] private GameObject heldPrefab;

        public float Weight => weight;
        public Vector3 Offset => offset;
        public Vector3 Rotation => rotation;
        public float DroppingForce => droppingForce;
        public GameObject HeldPrefab => heldPrefab;
    }
}
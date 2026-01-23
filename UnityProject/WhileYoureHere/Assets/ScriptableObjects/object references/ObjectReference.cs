using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "ObjectReference", menuName = "Scriptable Objects/ObjectReference")]
    public class ObjectReference : ScriptableObject
    {
        public GameObject objectReference;
    }
}
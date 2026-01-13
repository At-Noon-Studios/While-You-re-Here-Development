using UnityEngine;

namespace ScriptableObjects.chopping_logs
{
    [CreateAssetMenu(fileName = "StumpDialogueData", menuName = "Scriptable Objects/StumpDialogueData")]
    public class StumpDialogueData : ScriptableObject
    {
        [SerializeField] private float range;
        public float Range => range;
    }
}
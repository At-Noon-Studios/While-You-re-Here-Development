using UnityEngine;

namespace ScriptableObjects.door
{
    [CreateAssetMenu(fileName = "DoorConfig", menuName = "ScriptableObjects/DoorConfig")]
    public class DoorConfig : ScriptableObject
    {
        [Header("Door Settings")]
        public float openAngle = 90f;
        public float openSpeed = 3f;

        [Header("Door Sounds")]
        public AudioClip openSound;
        public AudioClip closeSound;
        public AudioClip lockedSound;
    }
}
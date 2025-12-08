using UnityEngine;

namespace ScriptableObjects.Controls
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        [SerializeField] private float walkBobSpeed;
        [SerializeField] private float walkBobAmount;
        [SerializeField] private float movementSpeed;

        public float WalkBobSpeed => walkBobSpeed;
        public float WalkBobAmount => walkBobAmount;
        public float MovementSpeed => movementSpeed;
    }
}
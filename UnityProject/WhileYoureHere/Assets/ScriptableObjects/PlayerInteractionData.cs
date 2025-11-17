using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "PlayerInteractionData", menuName = "ScriptableObjects/PlayerInteractionData")]
    public class PlayerInteractionData : ScriptableObject
    {
        [SerializeField] private float interactionReach;

        public float InteractionReach => interactionReach;
    }
}
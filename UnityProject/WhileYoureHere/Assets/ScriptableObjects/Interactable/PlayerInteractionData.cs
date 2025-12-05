using UnityEngine;

namespace ScriptableObjects.Interactable
{
    [CreateAssetMenu(fileName = "PlayerInteractionData", menuName = "ScriptableObjects/Interaction/PlayerInteractionData")]
    public class PlayerInteractionData : ScriptableObject
    {
        [SerializeField] private float interactionReach;
        [SerializeField] private float interactionAssistRadius;

        public float InteractionReach => interactionReach;
        public float InteractionAssistRadius => interactionAssistRadius;
    }
}
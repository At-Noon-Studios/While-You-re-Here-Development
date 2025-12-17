using UnityEngine;

namespace ScriptableObjects.Interactable
{
    public enum InteractionType
    {
        Text,
        Image
    }

    [CreateAssetMenu(fileName = "InteractionData", menuName = "ScriptableObjects/Interaction/InteractionData")]
    public class InteractionData : ScriptableObject
    {
        [Header("Choose if interaction uses text or image")]
        [SerializeField]
        public InteractionType interactionType = InteractionType.Text;

        [Header("Shown when InteractionType = Text")]
        [SerializeField]
        public string interactionText;

        [Header("Shown when InteractionType = Image (PNG/Sprite)")]
        [SerializeField]
        public Sprite interactionIcon;
    }
}
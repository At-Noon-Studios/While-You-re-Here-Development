using System.Collections.Generic;
using ScriptableObjects.Dialogue;
using UnityEngine;

namespace ScriptableObjects.dialogue
{
    [CreateAssetMenu(
        fileName = "DialogueInteractionConfig",
        menuName = "ScriptableObjects/Dialogue/DialogueInteractionConfig")]
    public class DialogueInteractionConfig : ScriptableObject
    {
        [Header("Dialogue Nodes this settings apply to")]
        public List<DialogueNode> dialogueNodes;

        [Header("Player / Interaction Behavior")]
        public bool pausePlayerMovement = true;
        public bool pauseCameraMovement = true;
        public bool showCursor = true;

        [Header("Audio Settings")]
        [Range(0f, 1f)]
        public float dialogueVolume = 1f;
    }
}
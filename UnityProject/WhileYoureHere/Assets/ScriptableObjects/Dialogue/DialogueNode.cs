using System.Collections.Generic;
using UnityEngine;
using dialogue;

namespace ScriptableObjects.Dialogue
{
    [CreateAssetMenu(fileName = "DialogueNode", menuName = "ScriptableObjects/Dialogue/Node")]
    public class DialogueNode : ScriptableObject
    {
        [Header("Node Info")]
        public string nodeID;
        public string speakerName;

        [Header("Dialogue Sentences")]
        public List<DialogueSentence> sentences;

        [Header("Choices")]
        public List<DialogueChoice> choices;

        [Header("Next Node (if no choices)")]
        public string targetNodeID;
    }
}
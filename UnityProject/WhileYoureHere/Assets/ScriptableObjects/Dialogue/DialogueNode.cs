using System.Collections.Generic;
using dialogue;
using UnityEngine;

namespace ScriptableObjects.Dialogue
{
    [CreateAssetMenu(fileName = "DialogueNode", menuName = "Dialogue/Node")]
    public class DialogueNode : ScriptableObject
    {
        [Header("Unique Identifier for this dialogue node")]
        public string nodeID;

        [Header("Name of the character speaking")]
        public string speakerName;

        [Header("Text shown in the dialogue box")]
        [TextArea(3, 10)]
        public string dialogueText;

        [Header("Available choices the player can select")]
        public List<DialogueChoice> choices;
    }
}
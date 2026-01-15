using ScriptableObjects.Dialogue;
using UnityEngine;

namespace radio_interaction
{
    [CreateAssetMenu(fileName = "RadioTracks", menuName = "ScriptableObjects/RadioTracks")]
    public class RadioTracks : ScriptableObject
    {
        [Header("Radio nodes")] 
        public string nodeName;
        public DialogueNode dialogueNode;
    }
}

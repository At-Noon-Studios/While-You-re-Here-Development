using UnityEngine;

namespace ScriptableObjects.Dialogue
{
    [CreateAssetMenu(fileName = "DialogueSentence", menuName = "ScriptableObjects/Dialogue/Sentence")]
    public class DialogueSentence : ScriptableObject
    {
        [Header("Text of this sentence")]
        [TextArea(1, 5)]
        public string text;

        [Header("Voice line for this sentence")]
        public AudioClip audio;
        
        [Header("Audio source to play this on")]
        public string tagOfAudioSource;
    }
}
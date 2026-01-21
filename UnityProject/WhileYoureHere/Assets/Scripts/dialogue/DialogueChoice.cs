using UnityEngine;

namespace dialogue
{
    [System.Serializable]
    public class DialogueChoice
    {
        [Header("Choice Settings")]
        public string targetNodeID;

        [Header("Choice Sprites")]
        public Sprite normalSprite;
        public Sprite selectedSprite;
    }
}
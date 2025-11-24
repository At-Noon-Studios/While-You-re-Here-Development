using UnityEngine;

namespace time
{
    [System.Serializable]
    public class TimeTransition
    {
        [Range(1, 8)]
        public int day;
    
        [Range(0, 23)]
        public int hour;

        public Texture2D fromSkybox;
        public Texture2D toSkybox;

        public Gradient lightGradient;

        [Tooltip("How long the transition lasts in seconds")]
        public float duration = 20f;

        [Header("Sun Rotation")]
        [Tooltip("Starting rotation angle for the Global Light (in degrees)")]
        public float startSunRotation = 270f;

        [Tooltip("Ending rotation angle for the Global Light (in degrees)")]
        public float endSunRotation = 360f;
    }
}
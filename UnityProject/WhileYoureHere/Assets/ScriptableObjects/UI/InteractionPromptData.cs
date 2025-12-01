using UnityEngine;

namespace ScriptableObjects.UI
{
    [CreateAssetMenu(fileName = "InteractionPromptData", menuName = "ScriptableObjects/InteractionPromptData")]
    public class InteractionPromptData : ScriptableObject
    {
        [SerializeField] private Color interactionAllowedColor;
        [SerializeField, Range(0, 1)] private float interactionAllowedAlpha;
        [SerializeField] private Color interactionNotAllowedColor;
        [SerializeField, Range(0, 1)] private float interactionNotAllowedAlpha;
        [SerializeField] private float pulseRoutineDuration;
        [SerializeField] private float pulseRoutineTargetScale;
        [SerializeField, Range(0, 255)] private float pulseRoutineTargetAlpha;
        [SerializeField] private Color pulseRoutineTargetColor;

        public Color InteractionAllowedColor => interactionAllowedColor;
        public float InteractionAllowedAlpha => interactionAllowedAlpha;
        public Color InteractionNotAllowedColor => interactionNotAllowedColor;
        public float InteractionNotAllowedAlpha => interactionNotAllowedAlpha;
        public float PulseRoutineDuration => pulseRoutineDuration;
        public float PulseRoutineTargetScale => pulseRoutineTargetScale;
        public float PulseRoutineTargetAlpha => pulseRoutineTargetAlpha;
        public Color PulseRoutineTargetColor => pulseRoutineTargetColor;
    }
}
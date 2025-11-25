using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

namespace gamestate
{
    
    [System.Serializable]
    public class GameplayEvent : PropertyAttribute
    {
        public GameplayEventType type;
        public TriggeredBy triggeredBy;
        
        
        [Header("Optional fields")]
        public string unlockedField;
        public int hourOfDay;
        public AudioClip dialogueToPlay;
        public VideoClip cutsceneToPlay;
        public int triggerAfterSeconds;
        public string customEventName;
    }

    public enum GameplayEventType
    {
        Unlock,
        SkyboxChange,
        Cutscene,
        Dialogue,
        ProgressToNextActivity,
        CustomEvent
    }

    public enum TriggeredBy
    {
        StartOfActivity,
        AfterSetTime,
        OnComplete,
        ByExternalTrigger,
    }
}

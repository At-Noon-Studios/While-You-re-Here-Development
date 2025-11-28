using System;
using System.Collections.Generic;
using chore;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace gamestate
{
    
    [Serializable]
    public class GameplayEvent
    {
        public GameplayEventType type;
        public TriggeredBy triggeredBy;
        
        public bool _expanded = false;
        
        public string booleanToChange;
        public bool newValue;
        public int hourOfDay;
        public AudioClip dialogueToPlay;
        public VideoClip cutsceneToPlay;
        public UnityEvent eventToInvoke;
        
        public int triggerAfterSeconds;
        public List<SoChore> choresToComplete;
        public List<string> booleansToBeTrue;
    }

    public enum GameplayEventType
    {
        BooleanChange,
        SkyboxChange,
        Cutscene,
        Dialogue,
        ProgressToNextActivity,
        InvokeCustomEvent
    }

    public enum TriggeredBy
    {
        StartOfActivity,
        AfterSetTime,
        AfterFinishActivity,
        OnChoresCompleted,
        BooleansToTrue,
    }
}

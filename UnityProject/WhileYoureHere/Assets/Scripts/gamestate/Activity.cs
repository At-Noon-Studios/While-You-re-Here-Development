using System.Collections.Generic;
using UnityEngine;

namespace gamestate
{
    [System.Serializable]
    public class Activity
    {
        public string description;
        [Header("What events can be triggered during this activity")]
        public List<GameplayEvent> events = new List<GameplayEvent>();
        
    }
}
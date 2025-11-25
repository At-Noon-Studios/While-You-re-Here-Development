using System.Collections.Generic;
using UnityEngine;

namespace gamestate
{
    [System.Serializable]
    public class Activity
    {
        [Header("What events can be triggered during this activity")]
        public List<GameplayEvent> events = new List<GameplayEvent>();
        
    }
}
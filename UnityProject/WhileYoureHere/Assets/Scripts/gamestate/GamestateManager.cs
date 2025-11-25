using System.Collections.Generic;
using UnityEngine;

namespace gamestate
{
    public class GamestateManager : MonoBehaviour
    {
        [Header("List with activities of this day")]
        [SerializeField] public List<Activity>  activities = new List<Activity>();
        
        private Activity _currentActivity;
        
        private static GamestateManager _instance;

        private void Awake()
        {
            _instance = this;
        }
        
        private void Start()
        {
            _currentActivity = activities[0];
        }

        private void Update()
        {
        
        }

        public static GamestateManager GetInstance()
        {
            return _instance;
        }

        public void GoToNextActivity()
        {
            // trigger oncomplete events
            _currentActivity = activities[activities.IndexOf(_currentActivity) + 1];
        }
    }
}


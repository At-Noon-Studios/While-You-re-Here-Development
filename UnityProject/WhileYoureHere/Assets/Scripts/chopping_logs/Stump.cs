using picking_up_objects;
using UnityEngine;

namespace chopping_logs
{
    public class Stump : MonoBehaviour
    {
        private Pickable _currentLog;

        public void RecieveLog(Pickable log)
        {
            _currentLog = log;
            // Stump logic
        }
        
        public bool HasLog => _currentLog != null;
        public Pickable GetLog() => _currentLog;

        public void ClearLog()
        {
            _currentLog = null;
        }
    }
}
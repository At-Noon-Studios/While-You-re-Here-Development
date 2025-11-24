using picking_up_objects;
using UnityEngine;

namespace chopping_logs
{
    public class Stump : MonoBehaviour
    {
        
        private bool _isLogPlacedOnStump;
        private Pickable _currentLog;

        private void Update()
        {
            if (_isLogPlacedOnStump)
            {
                ReceiveLog(_currentLog);
            }
        }
        
        private void ReceiveLog(Pickable log)
        {
            _currentLog = log;
            // Stump logic
        }

        public void ClearLog()
        {
            _currentLog = null;
        }
        
        public Pickable GetLog()
        {
            return _currentLog;
            Debug.Log("Stump received log");
        }
    }
}
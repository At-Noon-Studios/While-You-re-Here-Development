using Interactable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace chopping_logs
{
    public class LogBehaviour : InteractableBehaviour
    {
        
        [SerializeField] private float damageAmount = 10f;
        [SerializeField] private float maxHits = 3f;
        
        private float _currentHits;

        private void Awake()
        {
            base.Awake();
        }

        public override void Interact()
        {
           
        }

        public void LogBehaviourUpdate()
        {
            if (_currentHits >= maxHits)
            {
                //Split the log
                Destroy(gameObject);
            }
        }
        
        private void PhysicsUpdate()
        {
         // Handle physics-relatedupdates here (How the logs fall/split)
        }

        private void OnPickUp(InputValue value)
        {
            var position =  value.Get<Vector2>();
            
            position.y  *= -1;
            position.x *= -1;
        }
        
        public void GetLogHit()
        {
            _currentHits += damageAmount;
        }

        protected void SplitLog()
        {
            // Logic to split the log into smaller pieces
        }
    }
}
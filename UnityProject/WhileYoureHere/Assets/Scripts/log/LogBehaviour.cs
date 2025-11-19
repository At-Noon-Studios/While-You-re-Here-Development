using Interactable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace log
{
    public class LogBehaviour : InteractableBehaviour
    {

        private void Awake()
        {
            base.Awake();
        }

        public override void Interact()
        {
           
        }

        public void LogBehaviourUpdate()
        {
            
        }

        private void OnPickUp(InputValue value)
        {
            var position =  value.Get<Vector2>();
            
            position.y  *= -1;
            position.x *= -1;
        }
    }
}
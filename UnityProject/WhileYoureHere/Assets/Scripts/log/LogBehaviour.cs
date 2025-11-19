using Interactable;
using UnityEngine;

namespace log
{
    public class LogBehaviour : InteractableBehaviour
    {

        private void Awake()
        {
            base.Awake();
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public override void Interact()
        {
           // Logic
           print("Interact");
        }

        public void LogBehaviourUpdate()
        {
            
        }
    }
}
using ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerControls
{
    public class PlayerInputProcessor : MonoBehaviour
    {
        [Header("Publish to")] 
        [SerializeField] private Vector2EventChannel look;
        [SerializeField] private Vector2EventChannel move;
        [SerializeField] private EventChannel interact;
        [SerializeField] private EventChannel drop;
        [SerializeField] private EventChannel mouthBlow;

        
        private void OnLook(InputValue inputValue)
        {
            look.Raise(inputValue.Get<Vector2>());
        }

        private void OnMove(InputValue inputValue)
        {
            move.Raise(inputValue.Get<Vector2>());
        }
    
        private void OnInteract()
        {
            interact.Raise();
        }

        private void OnDrop()
        {
            drop.Raise();
        }

        private void OnMouthBlow()
        {
            mouthBlow.Raise();
        }
    }
}
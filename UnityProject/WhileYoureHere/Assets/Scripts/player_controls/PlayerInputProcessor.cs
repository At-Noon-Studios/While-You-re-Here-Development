using EventChannels;
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
    }
}
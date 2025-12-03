using ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace player_controls
{
    public class PlayerInputProcessor : MonoBehaviour
    {
        [Header("Publish to")] 
        [SerializeField] private Vector2EventChannel look;
        [SerializeField] private Vector2EventChannel move;
        [SerializeField] private EventChannel interact;
        [SerializeField] private EventChannel drop;
        
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
    }
}
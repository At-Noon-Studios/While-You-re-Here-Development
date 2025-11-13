using EventChannels;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputProcessor : MonoBehaviour
{
    [Header("Publish to")] 
    [SerializeField] private Vector2EventChannel look;
    [SerializeField] private Vector2EventChannel move;
        
    private void OnLook(InputValue inputValue)
    {
        look.Raise(inputValue.Get<Vector2>());
    }

    private void OnMove(InputValue inputValue)
    {
        move.Raise(inputValue.Get<Vector2>());
    }
}

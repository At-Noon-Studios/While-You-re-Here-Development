using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputProcessor : MonoBehaviour
{
    [Header("Publish to")] 
    [SerializeField] private Vector2EventChannel look;
        
    private void OnLook(InputValue inputValue)
    {
        look.Raise(inputValue.Get<Vector2>());
    }
}

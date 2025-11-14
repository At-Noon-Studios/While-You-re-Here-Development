using UnityEngine;
using UnityEngine.InputSystem;

//This is where axe logic takes place, and to see if the player is holding the axe
public class AxeController : MonoBehaviour
{

    private static bool HoldingAxe => true; // Placeholder for actual holding axe logic
    private Animator _currentAnimator;

    private void Start()
    {
        _currentAnimator = GetComponent<Animator>();
    }

    public void OnSwing(InputValue context)
    {
        if (context.isPressed && HoldingAxe)
        {
            _currentAnimator.SetTrigger("Swing");
        }
    }
}


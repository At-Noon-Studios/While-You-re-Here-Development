using UnityEngine;
using UnityEngine.InputSystem;

namespace controller.axe
{
    public class AxeController : MonoBehaviour
    {
        private static bool HoldingAxe => true;
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
}
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

namespace controller.axe
{
    public class AxeController : MonoBehaviour
    {
        [Header("Weapon State")] [SerializeField]
        private bool isHoldingAxe;

        [SerializeField] private float swipeThreshold = 0.5f;

        [SerializeField] private Animator axeAnimator;

        public void OnSwing(InputValue value)
        {
            var delta = value.Get<Vector2>();

            if (!isHoldingAxe)
            {
                axeAnimator.SetBool("HoldingAxe", false);
                return; // Early exit if not holding axe
            }

            if (Mathf.Abs(delta.y) > swipeThreshold)
            {
                axeAnimator.SetTrigger("Swing");
            }
        }
    }
}
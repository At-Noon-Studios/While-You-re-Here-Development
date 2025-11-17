using UnityEngine;
using UnityEngine.InputSystem;

namespace controller.axe
{
    public class AxeController : MonoBehaviour
    {
        [Header("Weapon State")] 
        [SerializeField] private bool isHoldingAxe;
        [SerializeField] private float swipeThreshold = 0.5f;

        [SerializeField] private Animator axeAnimator;

        // Called every frame while mouse moves
        public void OnSwing(InputValue value)
        {
            if (!isHoldingAxe)
                return;

            var delta = value.Get<Vector2>();
            
            delta.Normalize();

            // Detect vertical stroke motion
            if (Mathf.Abs(delta.y) > swipeThreshold)
            {
                axeAnimator.SetTrigger("Swing");
                Debug.Log("Axe swing triggered!");
            }
            
            Debug.Log("Animation played");
        }
    }
}
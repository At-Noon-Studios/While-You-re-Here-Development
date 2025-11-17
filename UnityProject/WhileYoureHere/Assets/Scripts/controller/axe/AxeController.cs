using UnityEngine;
using UnityEngine.InputSystem;

namespace controller.axe
{
    public class AxeController : MonoBehaviour
    {

        [Header("Weapon State")]
        [SerializeField] private bool isHoldingAxe;
        private Animator _currentAnimator;
        private Animator _axeAnimator;
        [SerializeField] private float swipeThreshold = 0.5f;

        private void Start()
        {
            _currentAnimator = GetComponent<Animator>();
        }

        public void OnSwing(InputValue inputValue)
        {
            if (!isHoldingAxe || !inputValue.isPressed) return;

            var delta = inputValue.Get<Vector2>();

            // Detect vertical swipe
            if (Mathf.Abs(delta.y) > swipeThreshold)
            {
                _currentAnimator = _axeAnimator;
                _currentAnimator.SetTrigger("Swing");
            }
            
            Debug.Log("Swinging axe with delta: " + delta);
        }

    }
}
using UnityEngine;
using UnityEngine.InputSystem;

namespace controller.axe
{
    public class AxeController : MonoBehaviour
    {
        [Header("Weapon State")] 
        [SerializeField] private bool isHoldingAxe;
        [SerializeField] private float swingCooldown = 0.5f;
        [SerializeField] private float swipeThreshold = 0.5f;

        [SerializeField] private Animator axeAnimator;

        private const string IsHoldingAxeParam = "IsHoldingAxe";
        private const string SwingTrigger = "Swing";

        private float _previousMouseY;
        private float _lastSwingTime;


        private void Start()
        {
            if (axeAnimator == null)
            {
                Debug.LogError("Axe Animator is not assigned in the inspector.");
            }
        }

        private void OnSwing()
        {
            if (!isHoldingAxe)
            {
                axeAnimator.SetBool(IsHoldingAxeParam, false);
                axeAnimator.ResetTrigger("Swing");
                return;
            }

            var currentMouseY = Mouse.current.position.ReadValue().y;
            var deltaY = currentMouseY - _previousMouseY;

            if (Mathf.Abs(deltaY) > swipeThreshold && Time.time - _lastSwingTime > swingCooldown)
            {
                axeAnimator.SetTrigger(SwingTrigger);
                _lastSwingTime = Time.time;
            }


            _previousMouseY = currentMouseY;
        }
    }
}
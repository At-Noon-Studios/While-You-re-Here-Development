using UnityEngine;
using UnityEngine.InputSystem;

namespace chopping_logs
{
    public class AxeMinigameController : MonoBehaviour
    {
        [SerializeField] private Stump stump;
        [SerializeField] private float moveSpeed = 0.01f;
        [SerializeField] private float rotateSpeed = 1f;

        private Transform _axeTransform;

        private void Awake()
        {
            _axeTransform = transform;
        }

        private void Update()
        {
            if (stump == null || !stump.MinigameActive)
                return;

            var mouseDelta = Mouse.current.delta.ReadValue();

            _axeTransform.localPosition += new Vector3(0, 0, mouseDelta.y * moveSpeed);
            _axeTransform.localRotation *= Quaternion.Euler(mouseDelta.y * rotateSpeed, 0, 0);
        }

        public void SetStump(Stump s)
        {
            stump = s;
        }
    }
}
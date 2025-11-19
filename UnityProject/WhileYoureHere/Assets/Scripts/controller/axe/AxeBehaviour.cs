using Interactable;
using UnityEngine;
using UnityEngine.Rendering;

namespace controller.axe
{
    public class AxeBehaviour : InteractableBehaviour
    {
        [SerializeField] private AxeController axeController;
        [SerializeField] private Volume axeVolume;
        
        private void Start()
        {
            axeController = GetComponent<AxeController>();
            if (axeController == null)
            {
                Debug.Log("AxeController not found on the same GameObject.");
            }
        }

        private void Update()
        {
        }

        public override void Interact()
        {
            if (axeController != null)
            {
                axeController.SetHoldingAxe(true);
            }
        }
    }
}
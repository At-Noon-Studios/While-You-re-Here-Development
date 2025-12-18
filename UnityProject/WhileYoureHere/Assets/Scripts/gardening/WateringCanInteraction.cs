using Interactable;
using Interactable.Concrete.ObjectHolder;
using UnityEngine;

namespace gardening
{
    using UnityEngine;

    public class WateringCanInteraction : MonoBehaviour
    {
        [SerializeField] private Canvas interactionCanvas;
        [SerializeField] private Behaviour outlineBehaviour;

        private bool _lastState;

        private void Awake()
        {
            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (interactionCanvas == null || outlineBehaviour == null) return;

            bool hovered = outlineBehaviour.enabled;

            if (hovered != _lastState)
            {
                interactionCanvas.gameObject.SetActive(hovered);
                _lastState = hovered;
            }
        }
    }
}

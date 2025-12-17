using gamestate;
using Interactable;
using UnityEngine;

namespace TaskList
{
    public class InteractableTaskListObject : InteractableBehaviour
    {
        [Header("TaskList UI")]
        [SerializeField] private TaskListUI taskListUI;

        [Header("Interaction UI")]
        [SerializeField] private Canvas interactionCanvas;

        private bool _pickedUp;
        private Transform _playerCamera;

        protected override void Awake()
        {
            base.Awake();

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);

            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                var cam = player.GetComponentInChildren<Camera>();
                if (cam != null)
                    _playerCamera = cam.transform;
            }
        }

        private void Update()
        {
            if (interactionCanvas != null && interactionCanvas.gameObject.activeSelf && _playerCamera != null)
            {
                interactionCanvas.transform.LookAt(_playerCamera);
                interactionCanvas.transform.Rotate(0f, 180f, 0f);
            }
        }

        public override void Interact(IInteractor interactor)
        {
            if (blockInteraction) return;
            BlockInteraction(true);

            gameObject.SetActive(false);

            taskListUI.RegisterTaskList(transform.gameObject);

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);

            var flag = GamestateManager.GetInstance()
                .listOfFlags.Find(f => f.name == "NotebookPickedUpFlag");
            if (flag != null)
                flag.currentValue = true;
        }

        public override void OnHoverEnter(IInteractor interactor)
        {
            base.OnHoverEnter(interactor);

            var canInteract = !blockInteraction;
            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(canInteract);
        }

        public override void OnHoverExit(IInteractor interactor)
        {
            base.OnHoverExit(interactor);

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);
        }
    }
}

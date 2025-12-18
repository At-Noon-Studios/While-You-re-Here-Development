using gamestate;
using Interactable;
using UnityEngine;

namespace TaskList
{
    public class InteractableTaskListObject : InteractableBehaviour
    {
        [Header("TaskList UI")]
        [SerializeField] private TaskListUI taskListUI;

        private bool _pickedUp;
        private Transform _playerCamera;

        protected override void Awake()
        {
            base.Awake();

            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                var cam = player.GetComponentInChildren<Camera>();
                if (cam != null)
                    _playerCamera = cam.transform;
            }
        }

        public override void Interact(IInteractor interactor)
        {
            if (blockInteraction) return;
            BlockInteraction(true);

            gameObject.SetActive(false);

            taskListUI.RegisterTaskList(transform.gameObject);

            var flag = GamestateManager.GetInstance()
                .listOfFlags.Find(f => f.name == "NotebookPickedUpFlag");
            if (flag != null)
                flag.currentValue = true;
        }
    }
}

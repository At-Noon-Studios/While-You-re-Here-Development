using UnityEngine;

namespace TaskList
{
    public class TaskListHintController : MonoBehaviour
    {
        [Header("Hint Texts")]
        [SerializeField] private GameObject taskListHintTextOpen;
        [SerializeField] private GameObject taskListHintTextPutAway;

        private bool _hasNotebook;
        private bool _notebookOpen;

        private void Awake()
        {
            HideAll();
        }

        public void OnNotebookPickedUp()
        {
            _hasNotebook = true;
            _notebookOpen = false;
            UpdateHint();
        }

        public void OnNotebookOpened()
        {
            if (!_hasNotebook) return;

            _notebookOpen = true;
            UpdateHint();
        }

        public void OnNotebookClosed()
        {
            if (!_hasNotebook) return;

            _notebookOpen = false;
            UpdateHint();
        }

        private void UpdateHint()
        {
            HideAll();

            if (!_hasNotebook)
                return;

            if (_notebookOpen)
                taskListHintTextPutAway.SetActive(true);
            else
                taskListHintTextOpen.SetActive(true);
        }

        private void HideAll()
        {
            taskListHintTextOpen.SetActive(false);
            taskListHintTextPutAway.SetActive(false);
        }
    }
}
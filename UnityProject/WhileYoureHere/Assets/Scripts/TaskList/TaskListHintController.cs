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
        private bool _hideHints;

        private void Awake()
        {
            HideAll();
        }

        public void SetHintsHidden(bool hidden)
        {
            _hideHints = hidden;
            UpdateHint();
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

            if (_hideHints || !_hasNotebook)
                return;

            if (_notebookOpen)
                taskListHintTextPutAway.SetActive(true);
            else
                taskListHintTextOpen.SetActive(true);
        }

        private void HideAll()
        {
            if (taskListHintTextOpen != null) taskListHintTextOpen.SetActive(false);
            if (taskListHintTextPutAway != null) taskListHintTextPutAway.SetActive(false);
        }
    }
}
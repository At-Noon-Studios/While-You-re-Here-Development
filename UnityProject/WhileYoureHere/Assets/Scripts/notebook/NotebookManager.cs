using ScriptableObjects.Events;
using UnityEngine;


namespace notebook
{
    public class NotebookManager : MonoBehaviour
    {
        [Header("Listen to")]
        [SerializeField] private EventChannel notebook;

        private bool hasNotebook;
        public bool HasNotebook => hasNotebook;
        private bool isNotebookGrabbed = true;

        private void OnEnable()
        {
            notebook.OnRaise += OnNotebookInput;
        }

        private void OnDisable()
        {
            notebook.OnRaise -= OnNotebookInput;
        }

        private void OnNotebookInput()
        {
            if (!hasNotebook)
            {
                return;
            }
            {
                ToggleNotebook();
            }
        }
        
        public void ObtainNotebook()
        {
            hasNotebook = true;
        }

        private void ToggleNotebook()
        {
            isNotebookGrabbed = !isNotebookGrabbed;

            if (isNotebookGrabbed)
            {
                GrabNotebook();
            }
            else
            {
                PutAwayNotebook();
            }
        }

        private void GrabNotebook()
        {
            Debug.Log(isNotebookGrabbed + ", Notebook grabbed");
            // Add Grab Notebook Animation here.
        }

        private void PutAwayNotebook()
        {
            Debug.Log(isNotebookGrabbed + ", Notebook put away");
            // Add Put Away Notebook Animation here.
        }
    }
}

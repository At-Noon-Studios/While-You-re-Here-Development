using ScriptableObjects.Events;
using UnityEngine;


namespace notebook
{
    public class NotebookManager : MonoBehaviour
    {
        [Header("Notebook Settings")]
        [SerializeField] private GameObject notebook;
        
        [Header("Listen to")]
        [SerializeField] private EventChannel notebookEvent;

        private bool hasNotebook;
        public bool HasNotebook => hasNotebook;
        private bool isNotebookGrabbed = true;

        private void OnEnable()
        {
            notebookEvent.OnRaise += OnNotebookInput;
        }

        private void OnDisable()
        {
            notebookEvent.OnRaise -= OnNotebookInput;
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
            notebook.SetActive(true);
            Debug.Log(notebook.activeSelf);
        }

        private void PutAwayNotebook()
        {
            Debug.Log(isNotebookGrabbed + ", Notebook put away");
            // Add Put Away Notebook Animation here.
            notebook.SetActive(false);
            Debug.Log(notebook.activeSelf);
        }
    }
}

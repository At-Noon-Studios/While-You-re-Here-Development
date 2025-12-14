using UnityEngine;
using System.Collections;

namespace TaskList
{
    public class TaskListUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Canvas taskListCanvas;
        [SerializeField] private Transform handPosition;
        [SerializeField] private TaskListHintController hintController;

        [Header("Animation Settings")]
        [SerializeField] private Vector3 spawnOffset = new Vector3(0, -0.3f, -0.5f);
        [SerializeField] private Vector3 spawnRotation = new Vector3(45f, 0f, 0f);
        [SerializeField] private float spawnDuration = 0.5f;

        private GameObject _taskListObject;
        private Rigidbody _rigidbody;

        private bool _available;
        private bool _isOpen;

        public void RegisterTaskList(GameObject taskListObject)
        {
            _taskListObject = taskListObject;
            _available = true;
            _isOpen = false;

            _rigidbody = _taskListObject.GetComponent<Rigidbody>();
            if (_rigidbody != null)
            {
                _rigidbody.useGravity = false;
                _rigidbody.isKinematic = true;
            }

            taskListCanvas.gameObject.SetActive(false);
            _taskListObject.SetActive(false);

            hintController?.OnNotebookPickedUp();
        }

        public void ToggleTaskList()
        {
            if (!_available)
            {
                return;
            }

            if (_isOpen)
                CloseNotebook();
            else
                OpenNotebook();
        }

        private void OpenNotebook()
        {
            _isOpen = true;
            taskListCanvas.gameObject.SetActive(true);

            hintController?.OnNotebookOpened();

            if (_taskListObject == null || handPosition == null)
                return;

            _taskListObject.SetActive(true);
            _taskListObject.transform.SetParent(handPosition);

            if (_rigidbody != null)
            {
                _rigidbody.useGravity = false;
                _rigidbody.isKinematic = true;
            }

            StartCoroutine(AnimateSpawn(_taskListObject.transform));
        }

        private void CloseNotebook()
        {
            _isOpen = false;
            taskListCanvas.gameObject.SetActive(false);

            hintController?.OnNotebookClosed();

            if (_taskListObject != null)
                _taskListObject.SetActive(false);
        }

        private IEnumerator AnimateSpawn(Transform objTransform)
        {
            Vector3 startPos = spawnOffset;
            Quaternion startRot = Quaternion.Euler(spawnRotation);

            Vector3 targetPos = Vector3.zero;
            Quaternion targetRot = Quaternion.identity;

            float elapsed = 0f;

            objTransform.localPosition = startPos;
            objTransform.localRotation = startRot;

            while (elapsed < spawnDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / spawnDuration);

                objTransform.localPosition = Vector3.Lerp(startPos, targetPos, t);
                objTransform.localRotation = Quaternion.Slerp(startRot, targetRot, t);

                yield return null;
            }

            objTransform.localPosition = targetPos;
            objTransform.localRotation = targetRot;
        }
    }
}

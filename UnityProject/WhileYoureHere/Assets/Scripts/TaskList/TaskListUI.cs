using UnityEngine;
using System.Collections;

namespace TaskList
{
    public class TaskListUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Canvas taskListCanvas;
        [SerializeField] private Transform handPosition;
        [SerializeField] private TaskListSound taskListSound;
        [Header("Animation Settings")]
        [SerializeField] private Vector3 spawnOffset = new Vector3(0, -0.3f, -0.5f);
        [SerializeField] private Vector3 spawnRotation = new Vector3(45f, 0f, 0f);
        [SerializeField] private float spawnDuration = 0.5f;

        private GameObject _taskListObject;
        private Rigidbody _rigidbody;

        private bool _available;
        public bool isOpen;

        public void RegisterTaskList(GameObject taskListObject)
        {
            _taskListObject = taskListObject;
            _available = true;
            isOpen = false;

            _rigidbody = _taskListObject.GetComponent<Rigidbody>();
            if (_rigidbody != null)
            {
                _rigidbody.useGravity = false;
                _rigidbody.isKinematic = true;
            }

            taskListCanvas.gameObject.SetActive(false);
            _taskListObject.SetActive(false);
        }

        public void ToggleTaskList()
        {
            if (!_available)
            {
                return;
            }

            if (isOpen)
            {
                taskListSound.PlayTasklistGrabSound();
                CloseNotebook();
            }
            else
            {
                taskListSound.PlayTasklistCloseSound();
                OpenNotebook();
            }
        }

        private void OpenNotebook()
        {
            isOpen = true;
            taskListCanvas.gameObject.SetActive(true);

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
            isOpen = false;
            taskListCanvas.gameObject.SetActive(false);

            if (_taskListObject != null)
                _taskListObject.SetActive(false);
        }

        private IEnumerator AnimateSpawn(Transform objTransform)
        {
            var startPos = spawnOffset;
            var startRot = Quaternion.Euler(spawnRotation);

            var targetPos = Vector3.zero;
            var targetRot = Quaternion.identity;

            var elapsed = 0f;

            objTransform.localPosition = startPos;
            objTransform.localRotation = startRot;

            while (elapsed < spawnDuration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / spawnDuration);

                objTransform.localPosition = Vector3.Lerp(startPos, targetPos, t);
                objTransform.localRotation = Quaternion.Slerp(startRot, targetRot, t);

                yield return null;
            }

            objTransform.localPosition = targetPos;
            objTransform.localRotation = targetRot;
        }
    }
}
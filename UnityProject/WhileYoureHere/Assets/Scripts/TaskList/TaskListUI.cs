using UnityEngine;
using System.Collections;

namespace TaskList
{
    public class TaskListUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Canvas taskListCanvas;
        [SerializeField] private Transform handPosition;

        [Header("Animation Settings")]
        [SerializeField] private Vector3 spawnOffset = new Vector3(0, -0.3f, -0.5f);
        [SerializeField] private Vector3 spawnRotation = new Vector3(45f, 0f, 0f);
        [SerializeField] private float spawnDuration = 0.5f;

        private GameObject _taskListObject;
        private bool _available;
        private Rigidbody _rigidbody;

        public void RegisterTaskList(GameObject taskListObject)
        {
            _taskListObject = taskListObject;
            _available = true;

            _rigidbody = _taskListObject.GetComponent<Rigidbody>();
            if (_rigidbody != null)
            {
                _rigidbody.useGravity = false;
                _rigidbody.isKinematic = true;
            }

            taskListCanvas.gameObject.SetActive(false);
        }

        public void ToggleTaskList()
        {
            if (!_available)
            {
                Debug.LogWarning("TaskList not available!");
                return;
            }

            bool show = !taskListCanvas.gameObject.activeSelf;
            taskListCanvas.gameObject.SetActive(show);

            if (show)
            {
                if (_taskListObject != null && handPosition != null)
                {
                    _taskListObject.SetActive(true);
                    _taskListObject.transform.SetParent(handPosition);

                    if (_rigidbody != null)
                    {
                        _rigidbody.useGravity = false;
                        _rigidbody.isKinematic = true;
                    }

                    StartCoroutine(AnimateSpawn(_taskListObject.transform));
                }
            }
            else
            {
                if (_taskListObject != null)
                    _taskListObject.SetActive(false);
            }
        }

        private IEnumerator AnimateSpawn(Transform objTransform)
        {
            Vector3 startPos = spawnOffset;
            Vector3 startRot = spawnRotation;

            Vector3 targetPos = Vector3.zero;
            Quaternion targetRot = Quaternion.identity;

            float elapsed = 0f;

            objTransform.localPosition = startPos;
            objTransform.localRotation = Quaternion.Euler(startRot);

            while (elapsed < spawnDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / spawnDuration);

                objTransform.localPosition = Vector3.Lerp(startPos, targetPos, t);
                objTransform.localRotation = Quaternion.Slerp(Quaternion.Euler(startRot), targetRot, t);

                yield return null;
            }
            objTransform.localPosition = targetPos;
            objTransform.localRotation = targetRot;
        }
    }
}

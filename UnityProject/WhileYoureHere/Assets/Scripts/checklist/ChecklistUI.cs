using UnityEngine;

namespace checklist
{
    public class ChecklistUI : MonoBehaviour
    {
        public Canvas checklistCanvas;
        public GameObject panel;
        public Vector3 panelOffset = Vector3.zero;

        [Header("Optional: assign a hand position object")]
        public Transform handPosition;

        private bool _canShowChecklist = false;
        private Transform _objectTransform;
        private GameObject _objectToShow;
        private GameObject _uiInstance;

        public void SetChecklistAvailable(bool available, Transform objectTransform, GameObject objectToShow)
        {
            _canShowChecklist = available;
            _objectTransform = objectTransform;
            _objectToShow = objectToShow;
        }

        public void OnCheckListInput()
        {
            if (!_canShowChecklist || _objectTransform == null || _objectToShow == null) return;

            if (checklistCanvas.gameObject.activeSelf)
            {
                checklistCanvas.gameObject.SetActive(false);
                _objectToShow.SetActive(false);
                if (_uiInstance != null) Destroy(_uiInstance);
            }
            else
            {
                checklistCanvas.gameObject.SetActive(true);
                _objectToShow.SetActive(true);

                _uiInstance = Instantiate(panel, _objectTransform.position, Quaternion.identity);
                _uiInstance.transform.SetParent(_objectTransform);
                _uiInstance.transform.localPosition = panelOffset;

                if (handPosition != null)
                {
                    _objectToShow.transform.SetParent(handPosition);
                    _objectToShow.transform.localPosition = Vector3.zero;
                    _objectToShow.transform.localRotation = Quaternion.identity;
                }
            }
        }
    }
}

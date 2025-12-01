using UnityEngine;

namespace checklist
{
    public class ChecklistUI : MonoBehaviour
    {
        public Canvas checklistCanvas;
        public GameObject panel;
        public Vector3 uiOffset = new Vector3(100, 100, 0);

        private bool _canShowChecklist = false;
        private Transform _objectTransform;

        public void SetChecklistAvailable(bool available, Transform objectTransform)
        {
            _canShowChecklist = available;
            _objectTransform = objectTransform;
        }

        public void OnCheckListInput()
        {
            if (!_canShowChecklist) return;

            checklistCanvas.gameObject.SetActive(!checklistCanvas.gameObject.activeSelf);

            if (checklistCanvas.gameObject.activeSelf)
            {
                ShowPanelOnObject();
            }
            else
            {
                if (panel != null)
                    panel.SetActive(false);
            }
        }

        private void ShowPanelOnObject()
        {
            if (_objectTransform == null || panel == null) return;

            panel.SetActive(true);

            Vector3 screenPos = Camera.main.WorldToScreenPoint(_objectTransform.position);
            screenPos += uiOffset;
            panel.transform.position = screenPos;
        }
    }
}
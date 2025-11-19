using ScriptableObjects.CheckList;
using UnityEngine;
using TMPro;

namespace CheckList
{
    public class ChecklistManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Canvas checklistCanvas;
        [SerializeField] private Transform checklistPanel;
        [SerializeField] private GameObject checklistItemPrefab;

        [Header("Data")]
        [SerializeField] private ChecklistData checklistData;

        private void Start()
        {
            if (checklistPanel != null && checklistItemPrefab != null && checklistData != null)
            {
                foreach (string task in checklistData.tasks)
                {
                    GameObject item = Instantiate(checklistItemPrefab, checklistPanel);
                    TextMeshProUGUI text = item.GetComponentInChildren<TextMeshProUGUI>();

                    if (text != null)
                        text.text = task;
                }
            }
        }

        public void OnCheckList()
        {
            checklistCanvas.gameObject.SetActive(!checklistCanvas.gameObject.activeSelf);
        }
    }
}
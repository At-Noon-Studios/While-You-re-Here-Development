using ScriptableObjects.CheckList;
using UnityEngine;
using TMPro;
using System.Collections;

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

        private void Awake()
        {
            PopulateChecklist();
        }

        private void PopulateChecklist()
        {
            foreach (string task in checklistData.tasks)
            {
                GameObject item = Instantiate(checklistItemPrefab, checklistPanel);
                TextMeshProUGUI text = item.GetComponentInChildren<TextMeshProUGUI>();

                if (text != null)
                    text.text = task;

                Transform checkmark = item.transform.Find("Background/Checkmark");
                if (checkmark != null)
                {
                    checkmark.gameObject.SetActive(false);
                    StartCoroutine(EnableCheckmarkAfterSeconds(checkmark.gameObject, 3f));
                }
            }
        }

        private IEnumerator EnableCheckmarkAfterSeconds(GameObject checkmark, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            checkmark.SetActive(true);
        }

        public void OnCheckList()
        {
            checklistCanvas.gameObject.SetActive(!checklistCanvas.gameObject.activeSelf);
        }
    }
}

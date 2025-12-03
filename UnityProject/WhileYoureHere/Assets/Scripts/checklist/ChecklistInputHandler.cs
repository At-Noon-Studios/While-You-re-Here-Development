using UnityEngine;

namespace checklist
{
    public class ChecklistInputHandler : MonoBehaviour
    {
        public ChecklistUI checklistUI;

        public void OnCheckList()
        {
            checklistUI.OnCheckListInput();
        }
    }
}
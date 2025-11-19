using UnityEngine;

namespace ScriptableObjects.CheckList
{
    [CreateAssetMenu(fileName = "NewChecklistData", menuName = "ScriptableObjects/ChecklistData")]
    public class ChecklistData : ScriptableObject
    {
        [TextArea]
        public string[] tasks;
    }
}
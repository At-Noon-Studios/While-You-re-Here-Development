using System.Collections.Generic;
using component;
using UnityEngine;

namespace ScriptableObjects.chores
{
    [CreateAssetMenu(fileName = "Chore", menuName = "ChoreSystem/Chore", order = 0)]
    public class SoChore : ScriptableObject
    {
        [Header("Chore Settings")]
        public string choreName;
        public int id;
        public List<SoChoreComponent> components;
    }
}

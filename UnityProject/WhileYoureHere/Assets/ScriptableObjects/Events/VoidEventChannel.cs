using System;
using UnityEngine;

[CreateAssetMenu(fileName = "VoidEventChannel", menuName = "ScriptableObjects/Events/VoidEventChannel")]
public class VoidEventChannel : ScriptableObject
{
        public event Action OnRaise;

        public void Raise()
        {
                OnRaise?.Invoke();
        }
}

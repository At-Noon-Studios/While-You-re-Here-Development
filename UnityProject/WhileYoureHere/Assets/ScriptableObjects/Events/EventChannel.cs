using System;
using UnityEngine;

namespace ScriptableObjects.Events
{
    [CreateAssetMenu(fileName = "EventChannel", menuName = "ScriptableObjects/Events/EventChannel")]
    public class EventChannel : ScriptableObject
    {
        public Action OnRaise;

        public void Raise()
        {
            OnRaise?.Invoke();
        }
    }
}
using System;
using UnityEngine;

namespace EventChannels
{
    [CreateAssetMenu(fileName = "EventChannel", menuName = "ScriptableObjects/Events/EventChannel")]
    public class EventChannel : ScriptableObject
    {
        public Action OnRaise;
        public Action<bool> OnClick;
        public void Raise()
        {
            OnRaise?.Invoke();
        }

        public void Raise(bool b)
        {
            OnClick?.Invoke(b);
        }
    }
}
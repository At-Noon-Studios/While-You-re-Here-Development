using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Vector2EventChannel", menuName = "ScriptableObjects/Events/Vector2EventChannel")]
public class Vector2EventChannel : ScriptableObject
{ 
        public Action<Vector2> OnRaise;

        public void Raise(Vector2 value)
        { 
                OnRaise?.Invoke(value);
        }
}
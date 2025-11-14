using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInput", menuName = "Scriptable Objects/PlayerInput")]
public class PlayerInput : ScriptableObject
{

    public event Action<Vector2> OnMove = delegate { };

}

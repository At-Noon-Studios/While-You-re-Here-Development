using UnityEngine;

[CreateAssetMenu(fileName = "RadioData", menuName = "Scriptable Objects/RadioData")]
public class RadioData : ScriptableObject
{
    public int tuningWaitTime;
    public float sensitivity = 0.02f;
}

using UnityEngine;

[CreateAssetMenu(fileName = "ObjectData", menuName = "ScriptableObjects/Objects/ObjectData")]
public class ObjectData : ScriptableObject
{
    public GameObject worldPrefab;
    public GameObject handPrefab;
}

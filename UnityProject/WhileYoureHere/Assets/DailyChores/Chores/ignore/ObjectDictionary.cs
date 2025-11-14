using System.Collections.Generic;
using UnityEngine;

/*
public class ObjectDictionary : MonoBehaviour
{
    public List<Object> objectPrefabs;
    private Dictionary<int, GameObject> objectDictionary;

    private void Awake()
    {
        for (int i = 0; i < objectPrefabs.Count; i++)
        {
            if (objectPrefabs[i] != null)
            {
                objectPrefabs[i].ID = i + 1;
            }
        }

        foreach (Object obj in objectPrefabs)
        {
            objectDictionary[obj.ID] = obj.gameObject;
        }
    }

    public GameObject GetObjectPrefab(int objectID)
    {
        objectDictionary.TryGetValue(objectID, out GameObject prefab);
        if (prefab == null)
        {
            Debug.LogWarning($"Object with ID {objectID} not found in dictionary.");
        }
        return prefab;
    }
}
*/

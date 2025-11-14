using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private int itemID;

    private void OnMouseDown()
    {
        QuestEvents.TriggerItemCollected(itemID);
        Destroy(gameObject);
    }
}

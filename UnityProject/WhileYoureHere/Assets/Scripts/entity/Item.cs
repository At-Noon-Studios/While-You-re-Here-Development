using chore;
using Interactable;
using UnityEngine;

namespace entity
{
    public class Item : InteractableBehaviour
    {
        [Header("Item")]
        [SerializeField] private int itemID;

        public override void Interact()
        {
            ChoreEvents.TriggerItemCollected(itemID);
            Destroy(gameObject);
        }

        // public void OnMouseDown()
        // {
        //     ChoreEvents.TriggerItemCollected(itemID);
        //     Destroy(gameObject);
        // }
    }
}

using chore;
using UnityEngine;

namespace component.example
{
    /*public class CcItemCollected : ChoreComponent
    {
        private readonly int _itemID;
        private int _itemCount;
        private readonly int _itemAmountNeeded;

        public CcItemCollected(string name, string description, int itemID, int itemAmountNeeded) : base(name,
            description)
        {
            _itemID = itemID;
            _itemAmountNeeded = itemAmountNeeded;
            ComponentType = ChoreComponentType.ItemCollected;
        }

        public static ChoreComponent CreateFactory(SoChoreComponent soChoreComponent)
        {
            var localChoreComponent = soChoreComponent as SoCcItemCollected; // Check type

            if (localChoreComponent == null)
            {
                Debug.LogError($"Factory Error: Wrong SO type passed to CcItemCollected.CreateFactory");
                return null;
            }

            return new CcItemCollected(
                localChoreComponent.componentName,
                localChoreComponent.description,
                localChoreComponent.itemID,
                localChoreComponent.itemAmountNeeded);
        }

        public override void EnableComponent()
        {
            base.EnableComponent();
            _itemCount = 0;
            ChoreEvents.OnItemCollected += ItemCollected;
        }

        public override void MarkCompleted()
        {
            base.MarkCompleted();
            ChoreEvents.OnItemCollected -= ItemCollected;
            
            TriggerComponentCompleted(this);
        }

        private void ItemCollected(int itemID)
        {
            if (_itemID != itemID)
                return;

            _itemCount++;

            Debug.Log($"Component {ComponentName}: Item Type {itemID} was collected {_itemCount}/{_itemAmountNeeded}");

            if (_itemCount < _itemAmountNeeded)
                return;

            MarkCompleted();
        }
    }*/
}

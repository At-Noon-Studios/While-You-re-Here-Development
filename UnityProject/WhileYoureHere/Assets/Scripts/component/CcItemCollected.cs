using chore;
using UnityEngine;

namespace component
{
    public class CcItemCollected : ChoreComponent
    {
        private readonly int _itemID;
        private int _itemCount;
        private readonly int _itemAmountNeeded;

        public CcItemCollected(string name, string description, int itemID, int itemAmountNeeded) : base(name,
            description)
        {
            _itemID = itemID;
            _itemAmountNeeded = itemAmountNeeded;
            _itemCount = 0;
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
            ChoreEvents.OnItemCollected += ItemCollected;
        }

        public override void MarkCompleted()
        {
            base.MarkCompleted();
            ChoreEvents.OnItemCollected -= ItemCollected;
        }

        private void ItemCollected(int itemID)
        {
            if (_itemID != itemID)
                return;

            _itemCount++;

            Debug.Log($"{ComponentName}: Item Type {itemID} was collected {_itemCount}/{_itemAmountNeeded}");

            if (_itemCount < _itemAmountNeeded)
                return;

            MarkCompleted();
            TriggerComponentCompleted(this);
        }
    }
}

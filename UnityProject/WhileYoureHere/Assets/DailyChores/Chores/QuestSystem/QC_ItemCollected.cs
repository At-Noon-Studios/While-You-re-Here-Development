using UnityEngine;

public class QC_ItemCollected : QuestComponent
{
    private int _itemID;
    private int _itemCount;
    private int _itemAmountNeeded;

    public QC_ItemCollected(string name, string description, int itemID, int itemAmountNeeded) : base(name, description)
    {
        _itemID = itemID;
        _itemAmountNeeded = itemAmountNeeded;
        _itemCount = 0;
        ComponentType = QuestComponentType.ItemCollected;
    }

    public static QuestComponent CreateFactory(SO_QuestComponent so_questComponent)
    {
        SO_QC_ItemCollected localQuestComponent = so_questComponent as SO_QC_ItemCollected; // Check type

        if (localQuestComponent == null)
        {
            Debug.LogError($"Factory Error: Wrong SO type passed to QC_ItemCollected.CreateFactory");
            return null;
        }

        return new QC_ItemCollected(
            localQuestComponent.componentName,
            localQuestComponent.description,
            localQuestComponent.itemID,
            localQuestComponent.itemAmountNeeded);
    }

    public override void EnableComponent()
    {
        base.EnableComponent();
        QuestEvents.OnItemCollected += ItemCollected;
    }

    public override void MarkCompleted()
    {
        base.MarkCompleted();
        QuestEvents.OnItemCollected -= ItemCollected;
    }

    public void ItemCollected(int itemID)
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

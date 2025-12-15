using gamestate;
using Interactable;
using Interactable.Holdable;

public class NotebookHoldableBehaviour : HoldableObjectBehaviour
{
    private bool flagSet = false;

    public override void Interact(IInteractor interactor)
    {
        base.Interact(interactor);
        if (!flagSet) SetNotebookFlag();
    }

    private void SetNotebookFlag()
    {
        GamestateManager.GetInstance().listOfFlags.Find(flag => flag.name == "NotebookPickedUpFlag").currentValue = true;
        flagSet = true;
    }
}

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
        GamestateManager.GetInstance().notebookPickedUp = true;
        flagSet = true;
    }
}

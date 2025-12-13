using gamestate;
using Interactable;
using Interactable.Holdable;
using notebook;
using UnityEngine;

public class NotebookHoldableBehaviour : HoldableObjectBehaviour
{
    private bool flagSet = false;
    [SerializeField] private NotebookManager notebookManager;

    public override void Interact(IInteractor interactor)
    {
        base.Interact(interactor);
        if (!flagSet) SetNotebookFlag();
    }

    private void SetNotebookFlag()
    {
        GamestateManager.GetInstance().listOfFlags.Find(flag => flag.name == "NotebookPickedUpFlag").currentValue = true;
        flagSet = true;
        notebookManager.ObtainNotebook();
    }
}

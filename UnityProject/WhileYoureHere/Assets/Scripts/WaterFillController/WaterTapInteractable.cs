using Interactable;
using UnityEngine;

public class WaterTapInteractable : InteractableBehaviour
{
    public WaterTap tap;   // Referenz zum WaterTap-Skript

    protected override string InteractionText()
    {
        return tap.isRunning ? "Wasserhahn schließen" : "Wasserhahn öffnen";
    }

    public override void Interact()
    {
        tap.ToggleTap();
    }
}
using Interactable;
using UnityEngine;

public class StoveButtonInteractable : InteractableBehaviour
{
    [Header("Stove Object")]
    public GameObject stove;          // Der Herd

    private bool isOn = false;

    protected override string InteractionText()
    {
        return isOn ? "Herd ausschalten" : "Herd einschalten";
    }

    public override void Interact()
    {
        isOn = !isOn;

        // Herd ein-/ausschalten
        if (stove != null)
            stove.SetActive(isOn);

        Debug.Log("Stove is now: " + (isOn ? "ON" : "OFF"));
    }
}
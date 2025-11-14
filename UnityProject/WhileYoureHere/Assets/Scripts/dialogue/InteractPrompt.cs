using UnityEngine;

public class InteractPrompt : MonoBehaviour
{
    public Transform player;
    public float interactDistance = 2f;
    public GameObject promptUI;

    private bool isInteracting;
    private bool isVisible;

    void Update()
    {
        if (isInteracting) return;

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist <= interactDistance && !isVisible)
        {
            ShowPrompt();
        }
        else if (dist > interactDistance && isVisible)
        {
            HidePrompt();
        }
    }

    public void BeginInteraction()
    {
        isInteracting = true;
        HidePrompt();
    }

    public void EndInteraction()
    {
        isInteracting = false;
    }

    private void ShowPrompt()
    {
        promptUI.SetActive(true);
        isVisible = true;
    }

    private void HidePrompt()
    {
        promptUI.SetActive(false);
        isVisible = false;
    }
}
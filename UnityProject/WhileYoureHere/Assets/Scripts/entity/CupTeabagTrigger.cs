using UnityEngine;
using chore;

public class CupTeabagTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Teabag"))
        {
            ChoreEvents.TriggerTeabagAdded();
        }
    }
}
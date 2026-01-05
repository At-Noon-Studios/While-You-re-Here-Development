using chore;
using UnityEngine;

namespace making_tea
{
    public class CupTeabagTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Teabag")) return;
            ChoreEvents.TriggerTeabagAdded();
            Destroy(other.gameObject);
        }
    }
}
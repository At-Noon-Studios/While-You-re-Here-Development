using UnityEngine;

namespace chopping_logs
{
    public class TriggerTest : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("TRIGGER ENTER: " + other.name + " | Tag: " + other.tag);
        }
    }
}
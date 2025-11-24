using UnityEngine;

public class WaterFillZone : MonoBehaviour
{
    public WaterTap tap;

    void OnTriggerStay(Collider other)
    {
        KettleFill kettle = other.GetComponentInParent<KettleFill>();

        if (kettle != null)
        {
            if (tap.isRunning)
                kettle.StartFilling();
            else
                kettle.StopFilling();
        }
    }

    void OnTriggerExit(Collider other)
    {
        KettleFill kettle = other.GetComponentInParent<KettleFill>();
        if (kettle != null) kettle.StopFilling();
    }
}
using UnityEngine;

public class WaterLevelVisualizer : MonoBehaviour
{
    public KettleFill kettle;

    public float emptyY = 0f;
    public float fullY = 0.22f;

    [Header("Collider für Wasseroberfläche")]
    public Collider waterSurfaceCollider;

    void Start()
    {
        if (kettle == null)
            kettle = GetComponentInParent<KettleFill>();

        emptyY = transform.localPosition.y;
    }

    void Update()
    {
        if (kettle == null) return;

        float t = Mathf.Clamp01(kettle.fillAmount / kettle.maxFill);

        // visuelle Höhe updaten
        Vector3 pos = transform.localPosition;
        pos.y = Mathf.Lerp(emptyY, fullY, t);
        transform.localPosition = pos;

        // Überlauf-Logik: Collider toggeln
        if (kettle.fillAmount >= kettle.maxFill)
            waterSurfaceCollider.isTrigger = false;   // Partikel prallen ab → Überlaufen
        else
            waterSurfaceCollider.isTrigger = true;    // Partikel fallen in die Kanne
    }
}
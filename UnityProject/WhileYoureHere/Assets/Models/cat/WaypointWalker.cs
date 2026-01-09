using UnityEngine;

public class WaypointWalker : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 0.8f;
    public float turnSpeed = 5f;
    public float reachDistance = 0.3f;

    public float animationSpeedMultiplier = 1.3f;

    int currentWaypoint = 0;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypoint];
        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        // Soepel draaien
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );

        // Bewegen
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        // Animatie snelheid aanpassen
        animator.speed = moveSpeed * animationSpeedMultiplier;

        // Volgend waypoint
        if (direction.magnitude < reachDistance)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }
}
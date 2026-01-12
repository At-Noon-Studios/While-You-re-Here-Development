using UnityEngine;

public class WaypointWalker : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 1f;
    public float turnSpeed = 5f;
    public float reachDistance = 0.3f;

    [Header("Animation")]
    public float animationSpeedMultiplier = 1.2f;

    [Header("Sitting")]
    public float minTimeBetweenSits = 12f;
    public float maxTimeBetweenSits = 24f;
    public float sitDuration = 3.5f;

    [Header("Flee")]
    public Transform player;
    public float fleeDistance = 5f;
    public float fleeSpeed = 4f;

    int currentWaypoint = 0;
    Animator animator;
    float sitTimer;
    float nextSitTime;
    bool isSitting;

    void Start()
    {
        animator = GetComponent<Animator>();
        ScheduleNextSit();
    }

    void Update()
    {
        if (player == null || waypoints.Length == 0) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 🏃‍♂️ FLEE HEEFT ALTIJD PRIORITEIT
        if (distanceToPlayer < fleeDistance)
        {
            ForceStandUp();
            FleeFromPlayer();
            return;
        }

        // 🪑 ZITTEN
        if (isSitting)
        {
            sitTimer -= Time.deltaTime;
            if (sitTimer <= 0)
            {
                StopSitting();
            }
            return;
        }

        // 🚶‍♂️ NORMAAL LOPEN
        FollowWaypoints();

        // ⏱️ RANDOM ZIT MOMENT
        nextSitTime -= Time.deltaTime;
        if (nextSitTime <= 0)
        {
            StartSitting();
        }
    }

    void FollowWaypoints()
    {
        Transform target = waypoints[currentWaypoint];
        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        SmoothRotate(direction);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        animator.speed = moveSpeed * animationSpeedMultiplier;

        if (direction.magnitude < reachDistance)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }

    void FleeFromPlayer()
    {
        Vector3 direction = transform.position - player.position;
        direction.y = 0;

        SmoothRotate(direction);
        transform.position += transform.forward * fleeSpeed * Time.deltaTime;

        animator.speed = fleeSpeed * animationSpeedMultiplier;
    }

    void SmoothRotate(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }

    // 🪑 ZIT LOGICA
    void StartSitting()
    {
        isSitting = true;
        sitTimer = sitDuration;
        animator.SetBool("IsSitting", true);
    }

    void StopSitting()
    {
        isSitting = false;
        animator.SetBool("IsSitting", false);
        ScheduleNextSit();
    }

    void ForceStandUp()
    {
        if (!isSitting) return;

        isSitting = false;
        animator.SetBool("IsSitting", false);
        ScheduleNextSit();
    }

    void ScheduleNextSit()
    {
        nextSitTime = Random.Range(minTimeBetweenSits, maxTimeBetweenSits);
    }

    // 🔔 Animation Event (optioneel, maar mag blijven)
    public void OnSitFinished()
    {
        StopSitting();
    }
}

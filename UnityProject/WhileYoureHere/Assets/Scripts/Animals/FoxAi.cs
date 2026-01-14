using UnityEngine;

public class FoxAI : MonoBehaviour
{
    [Header("References")]
    public Transform[] waypoints;
    public Transform player;

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float rotationSpeed = 6f;
    public float gravity = -9.81f;

    [Header("Behaviour")]
    public float lookDistance = 15f;    // afstand waarop vos speler ziet en kijkt
    public float fleeDistance = 13f;    // afstand waarop vos weg rent
    public float safeDistance = 30f;   // afstand waarop vos stopt met rennen
    public float waitTimeAtWaypoint = 1f; // tijd die vos wacht op waypoint

    private CharacterController controller;
    private Animator animator;

    private int currentWaypointIndex;
    private Vector3 velocity;

    private bool isFleeing = false;     // true zolang de vos aan het rennen is
    private float waitTimer = 0f;       // timer voor wachten op waypoint

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        ApplyGravity();

        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            // --- FLEE LOGIC ---
            if (isFleeing)
            {
                if (distance > safeDistance)
                    isFleeing = false;

                animator.SetBool("IsFlee", true);
                animator.SetBool("IsAlert", false);

                Flee();
                return;
            }

            // Start flee als speler te dichtbij komt
            if (distance < fleeDistance)
            {
                isFleeing = true;
                animator.SetBool("IsFlee", true);
                animator.SetBool("IsAlert", false);

                Flee();
                return;
            }

            // --- LOOK LOGIC ---
            if (distance < lookDistance)
            {
                animator.SetBool("IsAlert", true);
                animator.SetBool("IsFlee", false);

                LookAtPlayer();
                return;
            }

            // Speler te ver weg → terug naar patrol
            animator.SetBool("IsAlert", false);
            animator.SetBool("IsFlee", false);
        }

        // --- PATROL LOGIC ---
        Patrol();
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;

        Vector3 target = waypoints[currentWaypointIndex].position;
        Vector3 direction = target - transform.position;
        direction.y = 0f;

        if (direction.magnitude < 0.1f)
        {
            // Wacht even op waypoint
            waitTimer += Time.deltaTime;
            animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), 0f, Time.deltaTime * 5f));

            if (waitTimer >= waitTimeAtWaypoint)
            {
                waitTimer = 0f;

                // Kies een nieuwe random waypoint (anders dan huidige)
                int nextIndex;
                do
                {
                    nextIndex = Random.Range(0, waypoints.Length);
                } while (nextIndex == currentWaypointIndex && waypoints.Length > 1);

                currentWaypointIndex = nextIndex;
            }
            return;
        }

        // Draai naar waypoint
        Quaternion targetRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);

        // Beweeg
        Vector3 move = direction.normalized * walkSpeed * Time.deltaTime + velocity * Time.deltaTime;
        controller.Move(move);

        // Animatie
        animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), 1f, Time.deltaTime * 5f));
    }

    void LookAtPlayer()
    {
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;

        if (lookDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }

        // Stop beweging tijdens kijken
        animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), 0f, Time.deltaTime * 5f));
    }

    void Flee()
    {
        Vector3 fleeDir = (transform.position - player.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(fleeDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);

        Vector3 move = fleeDir * runSpeed * Time.deltaTime + velocity * Time.deltaTime;
        controller.Move(move);

        animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), 1f, Time.deltaTime * 10f));
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
    }
}

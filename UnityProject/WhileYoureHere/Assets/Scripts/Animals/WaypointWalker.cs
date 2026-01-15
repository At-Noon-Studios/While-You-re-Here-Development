using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointWalker : MonoBehaviour
{
    [Header("Waypoints")]
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
    public float fleeDuration = 3f;         // hoe lang de kat flee't
    public float followPlayerDuration = 2f; // hoe lang de kat de speler volgt na flee

    [Header("Gravity")]
    public float gravity = -9.81f;

    // public List<AudioClip> miauws;
    // public AudioClip rareMiauw;
    private bool blockMiauw;

    // =========================
    // INTERN
    // =========================
    int currentWaypoint = 0;
    Animator animator;
    CharacterController controller;
    AudioSource _audioSource;

    float sitTimer;
    float nextSitTime;
    float verticalVelocity;

    bool isSitting;

    // flee / follow
    bool isFleeing = false;
    float fleeTimer = 0f;
    float followTimer = 0f;

    Vector3 moveDirection;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        _audioSource = GetComponent<AudioSource>();
        ScheduleNextSit();
    }

    void Update()
    {
        if (player == null || waypoints.Length == 0) return;

        HandleGravity();

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 🏃‍♂️ FLEE / FOLLOW PLAYER
        if (distanceToPlayer < fleeDistance || isFleeing || followTimer > 0)
        {
            ForceStandUp();

            if (!isFleeing && distanceToPlayer < fleeDistance)
            {
                // Start flee
                isFleeing = true;
                fleeTimer = fleeDuration;
            }

            if (isFleeing)
            {
                FleeFromPlayer();
                fleeTimer -= Time.deltaTime;
                if (fleeTimer <= 0)
                {
                    isFleeing = false;
                    followTimer = followPlayerDuration;
                }
            }
            else if (followTimer > 0)
            {
                FollowPlayer();
                followTimer -= Time.deltaTime;
            }

            ApplyMovement();
            return;
        }

        // 🪑 SITTING
        if (isSitting)
        {
            sitTimer -= Time.deltaTime;
            if (sitTimer <= 0)
            {
                StopSitting();
            }

            ApplyMovement();
            return;
        }

        // 🚶‍♂️ NORMAL WALK
        FollowWaypoints();

        // ⏱️ RANDOM SIT TIMER
        nextSitTime -= Time.deltaTime;
        if (nextSitTime <= 0)
        {
            StartSitting();
        }

        ApplyMovement();
    }

    // =========================
    // MOVEMENT
    // =========================

    void FollowWaypoints()
    {
        blockMiauw = false;
        Transform target = waypoints[currentWaypoint];
        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        SmoothRotate(direction);

        moveDirection = transform.forward * moveSpeed;
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
        if (!blockMiauw)
        {
            blockMiauw = true;
            // StartCoroutine(PlayMiauw());
        }

        moveDirection = transform.forward * fleeSpeed;
        animator.speed = fleeSpeed * animationSpeedMultiplier;
    }

    // private IEnumerator PlayMiauw()
    // {
    //     var rand = Random.Range(0, 100);
    //     AudioClip audioToPlay;
    //     // if (rand <= 1)
    //     // {
    //     //     audioToPlay = rareMiauw;
    //     // }
    //     // else
    //     // {
    //     //     audioToPlay = miauws[Random.Range(0, miauws.Count)];
    //     // }
    //     _audioSource.PlayOneShot(audioToPlay);
    //     yield return new WaitForSeconds(audioToPlay.length);
    // }

    void FollowPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        SmoothRotate(direction);

        moveDirection = transform.forward * moveSpeed;
        animator.speed = moveSpeed * animationSpeedMultiplier;
    }

    void ApplyMovement()
    {
        Vector3 finalMove = moveDirection;
        finalMove.y = verticalVelocity;

        controller.Move(finalMove * Time.deltaTime);
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

    // =========================
    // GRAVITY
    // =========================

    void HandleGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;
    }

    // =========================
    // SITTING
    // =========================

    void StartSitting()
    {
        isSitting = true;
        sitTimer = sitDuration;
        animator.SetBool("IsSitting", true);
        moveDirection = Vector3.zero;
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

    // 🔔 Animation Event (optional)
    public void OnSitFinished()
    {
        StopSitting();
    }
}

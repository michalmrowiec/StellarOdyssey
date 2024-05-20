using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    public PatrolPoint[] patrolPoints;
    private float wait = 0;
    private int patrolDestination = 0;
    public int PatrolDestination
    {
        get => patrolDestination;
        set
        {
            if (patrolPoints.Length == value)
            {
                patrolDestination = 0;
            }
            else
            {
                patrolDestination = value;
            }
        }
    }
    public float patrolSpeed = 1.5f;
    private bool isTurning;

    public Rigidbody2D rb;
    public float moveSpeed = 2f;
    public float rotationSpeed = 1f;
    public int healthPoints = 1;
    public WeaponOwner weaponOwner;
    private FieldOfView fov;
    public NavMeshAgent agent;
    public Animator animator;

    bool arrived = false;
    bool isWait = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        fov = GetComponent<FieldOfView>();
        weaponOwner = GetComponentInChildren<WeaponOwner>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        if (patrolPoints.Length > 0)
        {
            agent.destination = patrolPoints[PatrolDestination].patrolPoint.position;
        }
    }

    void Update()
    {
        arrived = !agent.pathPending && agent.remainingDistance < 0.5f;

        animator.SetFloat("Speed", (rb.velocity.magnitude + agent.velocity.magnitude));

        if (!fov.CanSeePlayer)
        {
            if (arrived)
            {
                float angle = Mathf.LerpAngle(rb.rotation, patrolPoints[PatrolDestination].rotateInDirection, Time.deltaTime * rotationSpeed);
                rb.rotation = angle;
            }

            if (arrived && !isWait)
            {
                wait = Time.time + patrolPoints[PatrolDestination].secondWait;
                isWait = true;
            }
            else if (arrived && isWait && Time.time > wait)
            {
                isWait = false;
                PatrolDestination++;
                GotoNextPoint();
            }

            // Obracanie obiektu w kierunku ruchu
            if (agent.velocity.magnitude > 0.1f
                && !arrived) // Jeœli obiekt siê porusza
            {
                float targetAngle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg - 90f;
                float angle = Mathf.LerpAngle(rb.rotation, targetAngle, Time.deltaTime * rotationSpeed);
                rb.rotation = angle;
            }

            if (agent.isStopped)
            {
                agent.isStopped = false;
            }
        }
        else
        {
            if (!agent.isStopped)
            {
                agent.isStopped = true;
            }

            NoticedPlayer();
        }
    }


    void GotoNextPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        agent.destination = patrolPoints[PatrolDestination].patrolPoint.position;

        //PatrolDestination++;

        return;
    }

    public void TurnInDirection(Vector3 targett, float rotationSpeedd)
    {
        isTurning = true;

        // Oblicz kąt między obecną pozycją wroga a punktem patrolowania
        Vector2 directionToTarget = (targett - transform.position).normalized;
        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg - 90;

        // Obróć wroga w kierunku punktu patrolowania
        float angle = Mathf.LerpAngle(rb.rotation, targetAngle, Time.deltaTime * rotationSpeedd);
        rb.rotation = angle;

        // Sprawdź, czy wróg jest już skierowany w kierunku punktu patrolowania
        if (Mathf.Abs(Mathf.DeltaAngle(rb.rotation, targetAngle)) < 0.1f)
        {
            isTurning = false;
        }
    }

    public void NoticedPlayer()
    {
        Vector2 directionToPlayer = (fov.playerRef.transform.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90;

        float angle = Mathf.LerpAngle(rb.rotation, targetAngle, Time.deltaTime * rotationSpeed);
        rb.rotation = angle;

        transform.position = Vector2.MoveTowards(transform.position, fov.playerRef.transform.position, moveSpeed * Time.deltaTime);

        if (weaponOwner.weapon != null)
        {
            weaponOwner.weapon.Fire();
        }
    }

    public void TakeDamage(int damage = 1)
    {
        healthPoints -= damage;

        if (healthPoints <= 0)
        {
            Destroy(gameObject);
        }
    }
}

[Serializable]
public class PatrolPoint
{
    [SerializeField]
    public Transform patrolPoint;
    [SerializeField]
    public float secondWait = 0;
    [SerializeField]
    public float rotateInDirection = 0;
}
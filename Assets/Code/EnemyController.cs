using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public List<Transform> patrolPoints;
    private int patrolDestination;
    public int PatrolDestination
    {
        get => patrolDestination;
        set
        {
            if (patrolPoints.Count == value)
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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        fov = GetComponent<FieldOfView>();
        weaponOwner = GetComponentInChildren<WeaponOwner>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        if (patrolPoints.Count > 0)
        {
            agent.destination = patrolPoints[PatrolDestination].position;
        }
    }

    void Update()
    {
        animator.SetFloat("Speed", (rb.velocity.magnitude + agent.velocity.magnitude));

        if (!fov.CanSeePlayer)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                GotoNextPoint();
            }

            // Obracanie obiektu w kierunku ruchu
            if (agent.velocity.magnitude > 0.1f) // Jeœli obiekt siê porusza
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
        if (patrolPoints.Count == 0)
            return;

        agent.destination = patrolPoints[PatrolDestination].position;
        PatrolDestination++;

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

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    public List<PatrolPoint> patrolPoints;
    private float wait = 0;
    private int patrolDestination = 0;
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
    public float timeToRememberPlayerPosition = 2f;
    private float rememberPlayerPositionClock = 0f;
    public float moveSpeed = 2f;
    public float rotationSpeed = 1f;
    public int healthPoints = 1;
    public Rigidbody2D rb;
    public WeaponOwner weaponOwner;
    private FieldOfView fov;
    public NavMeshAgent agent;
    public Animator animator;
    private bool arrived = false;
    private bool isWait = false;

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
            agent.destination = patrolPoints[PatrolDestination].patrolPoint.position;
        }
        else
        {
            GameObject point = new GameObject("patrolPoint");
            point.transform.position = transform.position;

            patrolPoints.Add(
                new PatrolPoint()
                {
                    patrolPoint = point.transform,
                    rotate = true,
                    rotateInDirection = transform.rotation.eulerAngles.z * -1,
                    secondWait = 0f
                });

            agent.destination = patrolPoints[PatrolDestination].patrolPoint.position;
        }
    }

    void Update()
    {
        animator.SetFloat("Speed", (rb.velocity.magnitude + agent.velocity.magnitude));

        if (!fov.CanSeePlayer
            && Time.time > rememberPlayerPositionClock)
        {
            GotoNextPoint();

            arrived = !agent.pathPending && agent.remainingDistance < 0.5f;

            agent.isStopped = false;

            if (arrived
                && patrolPoints[PatrolDestination].rotate)
            {
                float angle = Mathf.LerpAngle(
                    rb.rotation,
                    patrolPoints[PatrolDestination].rotateInDirection % 360 * -1,
                    Time.deltaTime * rotationSpeed);
                rb.rotation = angle;
            }

            if (arrived && !isWait) //&& patrolPoints[PatrolDestination].secondWait > 0
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

            if (agent.velocity.magnitude > 0.1f
                && !arrived)
            {
                float targetAngle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg - 90f;
                float angle = Mathf.LerpAngle(rb.rotation, targetAngle, Time.deltaTime * rotationSpeed);
                rb.rotation = angle;
            }
        }
        else if (fov.CanSeePlayer)
        {
            NoticedPlayerAgent();
        }
        else if (Time.time < rememberPlayerPositionClock)
        {
            agent.isStopped = false;

            if (agent.velocity.magnitude > 0.1f)
            {
                float targetAngle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg - 90f;
                float angle = Mathf.LerpAngle(rb.rotation, targetAngle, Time.deltaTime * rotationSpeed);
                rb.rotation = angle;
            }
        }
    }

    void GotoNextPoint()
    {
        if (patrolPoints.Count == 0)
            return;

        agent.speed = patrolSpeed;
        agent.destination = patrolPoints[PatrolDestination].patrolPoint.position;

        return;
    }

    public void NoticedPlayerAgent()
    {
        rememberPlayerPositionClock = Time.time + timeToRememberPlayerPosition;

        agent.speed = moveSpeed;
        agent.destination = fov.playerRef.transform.position;

        if (agent.remainingDistance <= weaponOwner.weapon.attackDistance)
        {
            agent.isStopped = true;
        }
        else
            agent.isStopped = false;

        if (agent.velocity.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg - 90f;
            float angle = Mathf.LerpAngle(rb.rotation, targetAngle, Time.deltaTime * rotationSpeed);
            rb.rotation = angle;
        }
        else
        {
            Vector2 directionToPlayer = (fov.playerRef.transform.position - transform.position).normalized;
            float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90;

            float angle = Mathf.LerpAngle(rb.rotation, targetAngle, Time.deltaTime * rotationSpeed);
            rb.rotation = angle;
        }

        if (weaponOwner.weapon != null
            && fov.CanSeePlayer)
        {

            weaponOwner.weapon.Fire(Color.green, "Enemy");
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
    public float secondWait = 0f;
    [SerializeField]
    public bool rotate = false;
    [SerializeField]
    [Range(0, 360)]
    public float rotateInDirection = 0f;
}
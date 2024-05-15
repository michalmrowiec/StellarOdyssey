using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControll : MonoBehaviour
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
    public float patrolSpeed;
    private bool isTurning;

    public Rigidbody2D rb;
    public float moveSpeed = 2f;
    public float rotationSpeed = 1f;
    public int healthPoints = 1;
    public Weapon weapon;
    private FieldOfView fov;
    private bool canSeePlayer;

    public NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        fov = gameObject.GetComponent<FieldOfView>();


        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.destination = patrolPoints[patrolDestination].position;


        // Oblicz k¹t miêdzy obecn¹ pozycj¹ wroga a pierwszym punktem patrolowania
        //Vector2 directionToFirstPatrolPoint = (patrolPoints[0].position - transform.position).normalized;
        //float targetAngle = Mathf.Atan2(directionToFirstPatrolPoint.y, directionToFirstPatrolPoint.x) * Mathf.Rad2Deg - 90;

        // Obróæ wroga w kierunku pierwszego punktu patrolowania
        //rb.rotation = targetAngle;

        //StartCoroutine(Patrol());
    }

    // Update is called once per frame
    void Update()
    {
        if(!fov.CanSeePlayer)
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
        }
        else
        {
            NoticedPlayer();
        }
    }

    void GotoNextPoint()
    {
        if (patrolPoints.Count == 0)
            return;
        agent.destination = patrolPoints[patrolDestination].position;
        patrolDestination = (patrolDestination + 1) % patrolPoints.Count;
    }

    IEnumerator Patrol()
    {
        while (true)
        {
            if (!fov.CanSeePlayer)
            {
                if (Vector2.Distance(transform.position, patrolPoints[PatrolDestination].position) < 0.1f)
                {
                    // Jeœli tak, przejdŸ do nastêpnego punktu patrolowania
                    PatrolDestination++;

                    // Zatrzymaj na chwilê
                    yield return new WaitForSeconds(1); // Zatrzymaj na 2 sekundy

                    // Zacznij obracaæ siê w kierunku nastêpnego punktu patrolowania
                    isTurning = true;
                }
                else if (isTurning)
                {
                    TurnInDirection(patrolPoints[PatrolDestination].position, rotationSpeed);
                }
                else
                {
                    // Jeœli nie, kontynuuj ruch do punktu patrolowania
                    transform.position = Vector2.MoveTowards(transform.position, patrolPoints[PatrolDestination].position, patrolSpeed * Time.deltaTime);
                }
            }
            else
            {
                TurnInDirection(fov.playerRef.transform.position, rotationSpeed);

                transform.position = Vector2.MoveTowards(transform.position, fov.playerRef.transform.position, patrolSpeed * Time.deltaTime);

                weapon.Fire();
            }

            yield return null;
        }
    }

    public void TurnInDirection(Vector3 targett, float rotationSpeedd)
    {
        isTurning = true;

        // Oblicz k¹t miêdzy obecn¹ pozycj¹ wroga a punktem patrolowania
        Vector2 directionToTarget = (targett - transform.position).normalized;
        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg - 90;

        // Obróæ wroga w kierunku punktu patrolowania
        float angle = Mathf.LerpAngle(rb.rotation, targetAngle, Time.deltaTime * rotationSpeedd);
        rb.rotation = angle;

        // SprawdŸ, czy wróg jest ju¿ skierowany w kierunku punktu patrolowania
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

        //rb.velocity = directionToPlayer * moveSpeed;

        weapon.Fire();
    }

    private void FixedUpdate()
    {
        canSeePlayer = fov.CanSeePlayer;

        if (fov.CanSeePlayer)
        {
            //NoticedPlayer();
        }
        else
        {
            //Vector2 direction = transform.up;
            //rb.velocity = direction * moveSpeed;
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

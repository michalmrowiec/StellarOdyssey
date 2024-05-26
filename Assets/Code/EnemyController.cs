using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        ChasePlayer,
        SearchForPlayer,
        InvestigateBody
    }

    [SerializeField]
    public List<PatrolPoint> patrolPoints;
    public float patrolSpeed = 1.5f;
    public float timeToRememberPlayerPosition = 2f;
    public float chaseSpeed = 2f;
    public float rotationSpeed = 1f;
    public int healthPoints = 1;
    public float investigateSpeed = 1.7f;
    public float timeToInvestigate = 2f;
    public Rigidbody2D rb;
    public WeaponOwner weaponOwner;
    public NavMeshAgent agent;
    public Animator animator;
    public EnemyState enemyState = EnemyState.Patrol;

    private float rememberPlayerPositionForTime = 0f;
    private float investigateForTime = 0f;
    private FieldOfView fov;
    private Dictionary<EnemyState, Action> stateMachine;
    private PatrolController patrolController;
    private List<GameObject> investigatedBodies = new();
    //private Dictionary<GameObject, bool> investigatedBodies = new();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        fov = GetComponent<FieldOfView>();
        weaponOwner = GetComponentInChildren<WeaponOwner>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        enemyState = EnemyState.Patrol;

        if (patrolPoints.Count == 0)
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
        }

        patrolController = new(
            patrolPoints: patrolPoints,
            patrolSpeed: patrolSpeed,
            rb: rb,
            agent: agent,
            rotationSpeed: rotationSpeed);

        stateMachine = new Dictionary<EnemyState, Action>()
        {
            { EnemyState.Patrol, patrolController.Patrol },
            { EnemyState.ChasePlayer, NoticedPlayerAgent },
            { EnemyState.SearchForPlayer, SearchPlayer },
            { EnemyState.InvestigateBody, InvestigateBody }
        };
    }

    void Update()
    {
        animator.SetFloat("Speed", (rb.velocity.magnitude + agent.velocity.magnitude));

        switch (enemyState)
        {
            case EnemyState.Patrol:
                break;

            case EnemyState.ChasePlayer:
                if (!fov.CanSeePlayer)
                    enemyState = EnemyState.SearchForPlayer;
                break;

            case EnemyState.SearchForPlayer:
                if (Time.time > rememberPlayerPositionForTime)
                    enemyState = EnemyState.Patrol;
                break;

            case EnemyState.InvestigateBody:
                if (agent.remainingDistance > 0.5f)
                    investigateForTime = Time.time + timeToInvestigate;
                else if (Time.time > investigateForTime)
                    enemyState = EnemyState.Patrol;
                break;
        }

        if (fov.CanSeePlayer)
            enemyState = EnemyState.ChasePlayer;
        else if (fov.CanSeeBodies.Except(investigatedBodies).Any())
        {
            Debug.Log("nie weszłeś!");
            var newBodies = fov.CanSeeBodies.Except(investigatedBodies).ToList();
            var closestBody = newBodies.Aggregate((currentClosest, nextBody) =>
                Vector2.Distance(transform.position, nextBody.transform.position) <
                Vector2.Distance(transform.position, currentClosest.transform.position) ? nextBody : currentClosest);

            investigatedBodies.Add(closestBody);
            //investigateForTime = Time.time + timeToInvestigate;

            enemyState = EnemyState.InvestigateBody;
        }

        stateMachine[enemyState].Invoke();
    }

    private void InvestigateBody()
    {
        agent.speed = investigateSpeed;
        agent.destination = investigatedBodies.Last().transform.position;

        if (agent.velocity.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg - 90f;
            float angle = Mathf.LerpAngle(rb.rotation, targetAngle, Time.deltaTime * rotationSpeed);
            rb.rotation = angle;
        }
    }

    private void SearchPlayer()
    {
        agent.isStopped = false;

        if (agent.velocity.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg - 90f;
            float angle = Mathf.LerpAngle(rb.rotation, targetAngle, Time.deltaTime * rotationSpeed);
            rb.rotation = angle;
        }
    }

    private void NoticedPlayerAgent()
    {
        rememberPlayerPositionForTime = Time.time + timeToRememberPlayerPosition;

        agent.speed = chaseSpeed;
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
            Die();
        }
    }

    void Die()
    {
        agent.enabled = false;
        rb.isKinematic = true;
        this.enabled = false;
        this.gameObject.tag = "DeadEnemy";
        this.gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        weaponOwner.weapon.GetComponent<PickUpController>().Drop();
        GetComponent<Animator>().SetTrigger("Dead");
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

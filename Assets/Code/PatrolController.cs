using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolController
{
    private readonly IList<PatrolPoint> _patrolPoints;
    private readonly float _patrolSpeed;
    private readonly float _rotationSpeed;
    private Rigidbody2D _rb;
    private NavMeshAgent _agent;

    private bool _arrived = false;
    private bool _isWait = false;
    private float _wait = 0;
    private int _patrolDestination = 0;
    public int PatrolDestination
    {
        get => _patrolDestination;
        set
        {
            if (_patrolPoints.Count == value)
            {
                _patrolDestination = 0;
            }
            else
            {
                _patrolDestination = value;
            }
        }
    }

    public PatrolController(IList<PatrolPoint> patrolPoints, float patrolSpeed, Rigidbody2D rb, NavMeshAgent agent, float rotationSpeed)
    {
        _patrolPoints = patrolPoints;
        _patrolSpeed = patrolSpeed;
        _rb = rb;
        _agent = agent;
        _rotationSpeed = rotationSpeed;
    }

    public void Patrol()
    {
        GotoNextPoint();

        _arrived = !_agent.pathPending && _agent.remainingDistance < 0.5f;

        _agent.isStopped = false;

        if (_arrived
            && _patrolPoints[PatrolDestination].rotate)
        {
            float angle = Mathf.LerpAngle(
                _rb.rotation,
                _patrolPoints[PatrolDestination].rotateInDirection % 360 * -1,
                Time.deltaTime * _rotationSpeed);
            _rb.rotation = angle;
        }

        if (_arrived && !_isWait)
        {
            _wait = Time.time + _patrolPoints[PatrolDestination].secondWait;
            _isWait = true;
        }
        else if (_arrived && _isWait && Time.time > _wait)
        {
            _isWait = false;
            PatrolDestination++;
            GotoNextPoint();
        }

        if (_agent.velocity.magnitude > 0.1f
            && !_arrived)
        {
            float targetAngle = Mathf.Atan2(_agent.velocity.y, _agent.velocity.x) * Mathf.Rad2Deg - 90f;
            float angle = Mathf.LerpAngle(_rb.rotation, targetAngle, Time.deltaTime * _rotationSpeed);
            _rb.rotation = angle;
        }
    }

    private void GotoNextPoint()
    {
        if (_patrolPoints.Count == 0)
            return;

        _agent.speed = _patrolSpeed;
        _agent.destination = _patrolPoints[PatrolDestination].patrolPoint.position;

        return;
    }
}
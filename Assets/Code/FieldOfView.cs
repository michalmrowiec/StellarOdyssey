using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float innerRadius = 0.5f;
    public float radius = 5f;
    [Range(1, 360)]
    public float angle = 45f;
    public LayerMask targetLayer;
    public LayerMask obstructionLayer;

    public GameObject playerRef;

    public bool CanSeePlayer { get; private set; }
    public List<GameObject> CanSeeBodies { get; private set; } = new();

    void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVCheck());
    }

    private IEnumerator FOVCheck()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FOV();
        }
    }

    private void FOV()
    {
        Collider2D[] rangeCheck = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer);
        Collider2D[] innerRangeCheck = Physics2D.OverlapCircleAll(transform.position, innerRadius, targetLayer);

        if (rangeCheck.Length > 0 || innerRangeCheck.Length > 0)
        {
            List<Collider2D> targets = rangeCheck.Where(x =>
                Vector2.Angle(transform.up, (Vector2)(x.transform.position - transform.position).normalized) < angle / 2).ToList();
            targets.AddRange(innerRangeCheck);
            targets = targets.Distinct().ToList();
            targets.Remove(targets.FirstOrDefault(x => x.gameObject == this.gameObject));

            if (targets.Any())
            {
                List<Collider2D> directTargets = new();

                foreach (var target in targets)
                {
                    RaycastHit2D[] hits = Physics2D.RaycastAll(
                            transform.position,
                            (target.transform.position - transform.position).normalized,
                            Vector2.Distance(transform.position, target.transform.position),
                            obstructionLayer);

                    if (hits.Length <= 1)
                    {
                        directTargets.Add(target);
                    }
                }

                CanSeeBodies.Clear();
                foreach (var target in directTargets)
                {
                    if (target.gameObject == playerRef)
                    {
                        CanSeePlayer = true;
                    }
                    else if (target.gameObject.CompareTag("DeadEnemy"))
                    {
                        CanSeeBodies.Add(target.gameObject);
                    }
                }
            }
            else
            {
                CanSeePlayer = false;
                CanSeeBodies.Clear();
            }
        }
        else if (CanSeePlayer)
        {
            CanSeePlayer = false;
            CanSeeBodies.Clear();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, radius);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, innerRadius);

        Vector3 angle01 = DirectionFromAngle(-transform.eulerAngles.z, -angle / 2);
        Vector3 angle02 = DirectionFromAngle(-transform.eulerAngles.z, angle / 2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + angle01 * radius);
        Gizmos.DrawLine(transform.position, transform.position + angle02 * radius);

        if (CanSeePlayer)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, playerRef.transform.position);
        }
    }

    private Vector2 DirectionFromAngle(float eulerY, float angleIndegrees)
    {
        angleIndegrees += eulerY;

        return new Vector2(Mathf.Sin(angleIndegrees * Mathf.Deg2Rad), Mathf.Cos(angleIndegrees * Mathf.Deg2Rad));
    }
#endif

}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FieldOfView : MonoBehaviour
{
    public float innerRadius = 0.5f;
    public float radius = 5f;
    [Range(1, 360)]
    public float angle = 45f;
    public LayerMask targetLayer;
    public LayerMask obstructionLayer;

    public GameObject playerRef;

    public List<Collider2D> objs;

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
            //Transform target = rangeCheck[0].transform;
            //Vector2 directionToTarget = (target.position - transform.position).normalized;

            //Transform[] targets = rangeCheck.Select(x => x.transform).ToArray();
            //Vector2[] directionToTargets = rangeCheck.Select(x => (Vector2)(x.transform.position - transform.position).normalized).ToArray();
            //var possibleSeeTargets = directionToTargets.Where(x => Vector2.Angle(transform.up, x) < angle / 2);


            List<Collider2D> targets = rangeCheck.Where(x =>
                Vector2.Angle(transform.up, (Vector2)(x.transform.position - transform.position).normalized) < angle / 2).ToList();
            targets.AddRange(innerRangeCheck);
            targets = targets.Distinct().ToList();
            targets.Remove(targets.FirstOrDefault(x => x.gameObject == this.gameObject));
            objs.Clear();
            objs.AddRange(targets);
            //Vector2.Angle(transform.up, directionToTarget) < angle / 2
            if (targets.Any())
            {
                //float distanceToTarget = Vector2.Distance(transform.position, target.position);

                //RaycastHit2D[] hits = Physics2D.RaycastAll((transform.position, directionToTarget, distanceToTarget, obstructionLayer);

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

                    //foreach (var hit in hits)
                    //{
                    //    // If the object notice itself as obstruction, skip them
                    //    if (hit.collider.gameObject != this.gameObject)
                    //    {
                    //        CanSeePlayer = false;
                    //        return;
                    //    }
                    //}

                    //if (hit.collider.gameObject == playerRef)
                    //{
                    //    CanSeePlayer = true;
                    //}
                    //else if (hit.collider.gameObject.CompareTag("DeadEnemy"))
                    //{
                    //    Debug.Log("See body");
                    //    CanSeeBodies.Add(hit.collider.gameObject);
                    //}
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
                        Debug.Log("See body");
                        CanSeeBodies.Add(target.gameObject);
                    }
                }


                //CanSeePlayer = true;
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
}

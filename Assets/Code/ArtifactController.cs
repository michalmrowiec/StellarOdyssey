using System;
using UnityEngine;

public class ArtifactController : MonoBehaviour
{
    public LayerMask targetLayer;
    public LayerMask obstructionLayer;
    public float pickUpRange = 1.5f;
    public GameObject playerRef;
    public static event Action OnPickUpChapter;

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        Collider2D[] rangeCheck = Physics2D.OverlapCircleAll(transform.position, pickUpRange, targetLayer);

        if (rangeCheck.Length > 0)
        {
            Transform closestTarget = null;
            float closestDistance = float.MaxValue;

            foreach (var collider in rangeCheck)
            {
                float distanceToTarget = Vector2.Distance(transform.position, collider.transform.position);
                if (distanceToTarget < closestDistance)
                {
                    closestDistance = distanceToTarget;
                    closestTarget = collider.transform;
                }
            }

            if (closestTarget != null)
            {
                Vector2 directionToTarget = (closestTarget.position - transform.position);

                if (!Physics2D.Raycast(transform.position, directionToTarget, closestDistance, obstructionLayer))
                    if (directionToTarget.sqrMagnitude <= pickUpRange
                                        && Input.GetKeyDown(KeyCode.E))
                    {
                        PickUpARtifact();
                    }
            }
        }
    }

    public void PickUpARtifact()
    {
        OnPickUpChapter();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, pickUpRange);
    }
#endif
}

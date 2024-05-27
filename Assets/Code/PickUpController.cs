using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public Weapon weapon;
    //public Transform weaponContainer;
    public WeaponOwner owner;
    public float pickUpRange = 1.5f;
    public bool equiped = false;
    public LayerMask targetLayer;
    public LayerMask obstructionLayer;

    public Vector3 weaponScaleOffset = Vector3.one;
    public Vector3 weaponPositionOffset = Vector3.zero;
    public Quaternion weaponRotationOffset = Quaternion.Euler(Vector3.zero);

    private void Start()
    {
        weapon = GetComponent<Weapon>();

        owner = gameObject.GetComponentInParent<WeaponOwner>();
        owner.weapon = weapon;

        if (GetComponentInParent<PlayerController>() != null)
        {
            SlowMotion.OnSlowMotionChanged += weapon.UpdateFireRate;
        }
    }

    private void OnDisable()
    {
        SlowMotion.OnSlowMotionChanged -= weapon.UpdateFireRate;
    }

    private void Update()
    {
        if (!equiped)
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
                    Vector2 directionToTarget = (closestTarget.position - transform.position).normalized;

                    if (!Physics2D.Raycast(transform.position, directionToTarget, closestDistance, obstructionLayer))
                        if (directionToTarget.sqrMagnitude <= pickUpRange
                                            && Input.GetKeyDown(KeyCode.E)
                                            && closestTarget.gameObject.GetComponent<WeaponOwner>().weapon == null)
                        {
                            owner = closestTarget.gameObject.GetComponent<WeaponOwner>();
                            PickUp();
                        }
                }
            }
        }


        if (equiped
            && owner.weapon != null
            && owner.OwnerIsPlayer
            && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }
    }

    private void PickUp()
    {
        if (owner.OwnerIsPlayer)
        {
            weapon.UpdateFireRate(SlowMotion.isSlowMotionActive);
            SlowMotion.OnSlowMotionChanged += weapon.UpdateFireRate;
        }

        weapon.GetComponent<Rigidbody2D>().isKinematic = true;
        equiped = true;
        owner.weapon = weapon;

        weapon.transform.SetParent(owner.weaponContainer);
        weapon.transform.localScale = weapon.transform.localScale;
        weapon.transform.localPosition = weaponPositionOffset;
        weapon.transform.localRotation = weaponRotationOffset;
    }

    public void Drop()
    {
        if (owner.OwnerIsPlayer)
        {
            SlowMotion.OnSlowMotionChanged -= weapon.UpdateFireRate;
            weapon.UpdateFireRate(false);
        }

        weapon.GetComponent<Rigidbody2D>().isKinematic = false;

        weapon.transform.SetParent(null);

        owner.weapon = null;
        owner = null;
        equiped = false;
        weapon.GetComponent<Rigidbody2D>().AddForce(weapon.transform.position * 5f);
    }

    private void OnDrawGizmos()
    {
        if (!equiped)
        {
            Gizmos.color = Color.white;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, pickUpRange);
        }
    }
}

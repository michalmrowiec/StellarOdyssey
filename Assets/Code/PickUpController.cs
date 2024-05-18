using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public Weapon weapon;
    //public Transform weaponContainer;
    public WeaponOwner owner;
    public float pickUpRange = 1.5f;
    public bool equiped;
    public LayerMask targetLayer;
    public LayerMask obstructionLayer;

    public Vector3 weaponScaleOffset = Vector3.one;
    public Vector3 weaponPositionOffset = Vector3.zero;
    public Quaternion weaponRotationOffset = Quaternion.Euler(Vector3.zero);

    void Start()
    {
        weapon = GetComponent<Weapon>();

        if(equiped)
        {
            //weaponContainer = gameObject.GetComponentInParent<WeaponOwner>().weaponContainer;
            owner = gameObject.GetComponentInParent<WeaponOwner>();
            owner.weapon = weapon;
        }
    }

    private void Update()
    {
        if (!equiped)
        {
            Collider2D[] rangeCheck = Physics2D.OverlapCircleAll(transform.position, pickUpRange, targetLayer);

            if (rangeCheck.Length > 0)
            {
                Transform target = rangeCheck[0].transform;
                Vector2 directionToTarget = (target.position - transform.position).normalized;


                float distanceToTarget = Vector2.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionLayer))
                    if (directionToTarget.sqrMagnitude <= pickUpRange
                                        && Input.GetKeyDown(KeyCode.E))
                    {
                        owner = target.gameObject.GetComponent<WeaponOwner>();
                        PickUp();
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
        equiped = true;
        owner.weapon = weapon;

        //weaponContainer = owner.transform.Find("WeaponContainer");

        //owner.weaponContainer = owner.weaponContainer;

        weapon.transform.SetParent(owner.weaponContainer);
        weapon.transform.localScale = weapon.transform.localScale;
        weapon.transform.localPosition = weaponPositionOffset;
        weapon.transform.localRotation = weaponRotationOffset;

        //transform.SetParent(owner.weaponContainer);
        //transform.localScale = weapon.transform.localScale;
        //transform.localPosition = weaponPositionOffset;
        //transform.localRotation = weaponRotationOffset;
    }

    public void Drop()
    {
        weapon.transform.SetParent(null);
        //transform.SetParent(null);
        //owner.weaponContainer = null;

        owner.weapon = null;
        owner = null;
        equiped = false;
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

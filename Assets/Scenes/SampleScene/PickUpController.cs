using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PickUpController : MonoBehaviour
{
    public Weapon weapon;
    public GameObject owner;
    public float pickUpRange = 1f;
    public bool equiped;
    public LayerMask targetLayer;
    public LayerMask obstructionLayer;
    public Transform weaponContainer;

    public Vector3 weaponScaleOffset = Vector3.one;
    public Vector3 weaponPositionOffset = Vector3.zero;
    public Quaternion weaponRotationOffset = Quaternion.Euler(Vector3.zero);

    void Start()
    {
        weapon = GetComponent<Weapon>();
        Debug.Log(owner);
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
                        owner = target.gameObject;
                        PickUp();
                    }
            }
        }

        if (equiped && weaponContainer != null && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }
    }

    private void PickUp()
    {
        equiped = true;
        Debug.Log("PickUp");
        Debug.Log(owner.gameObject.name);

        weaponContainer = owner.transform.Find("WeaponContainer");

        Debug.Log(weaponContainer.gameObject.name);

        transform.SetParent(weaponContainer.transform);
        transform.localScale = weapon.transform.localScale; //weaponScaleOffset;
        transform.localPosition = weaponPositionOffset;
        transform.localRotation = weaponRotationOffset;

        owner.GetComponent<PlayerController>().weapon = weapon;
    }

    private void Drop()
    {
        transform.SetParent(null);
        weaponContainer = null;

        owner.GetComponent<PlayerController>().weapon = null;
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

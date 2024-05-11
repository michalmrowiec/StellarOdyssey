using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public Weapon weapon;
    public GameObject owner;
    public float pickUpRange = 1f;
    public bool equiped;
    public LayerMask targetLayer;
    public Transform weaponContainer;

    void Start()
    {
        weapon = GetComponent<Weapon>();
        //playerRef = GameObject.FindGameObjectWithTag("Player");
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
                if (directionToTarget.sqrMagnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E))
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
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);

        owner.GetComponent<PlayerController>().weapon = weapon;
    }

    private void Drop()
    {
        Debug.Log("Drop");
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

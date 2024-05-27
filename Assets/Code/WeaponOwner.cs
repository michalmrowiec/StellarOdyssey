using System;
using TMPro;
using UnityEngine;

public class WeaponOwner : MonoBehaviour
{
    public Weapon weapon;
    public Transform weaponContainer;
    public bool OwnerIsPlayer { get; private set; }
    public TextMeshProUGUI ammoText;

    void Start()
    {
        if (gameObject.GetComponentInParent<PlayerController>() != null)
        {
            OwnerIsPlayer = true;
        }

        weaponContainer = gameObject.transform.Find("WeaponContainer");
    }

    void Update()
    {
        if (OwnerIsPlayer && ammoText != null)
        {
            if (weapon != null)
            {
                ammoText.text = $"{weapon.currentAmmo}/{weapon.maxAmmo}";
            }
            else
            {
                ammoText.text = "";
            }
        }
    }
}

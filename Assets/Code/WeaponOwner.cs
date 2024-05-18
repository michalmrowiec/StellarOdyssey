using UnityEngine;

public class WeaponOwner : MonoBehaviour
{
    public Weapon weapon;
    public Transform weaponContainer;
    //private bool ownerIsPlayer = false;
    public  bool OwnerIsPlayer { get; private set; }

    void Start()
    {
        if(gameObject.GetComponentInParent<PlayerController>() != null)
        {
            OwnerIsPlayer = true;
        }

        weaponContainer = gameObject.transform.Find("WeaponContainer");
    }
}

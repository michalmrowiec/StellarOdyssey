using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControll : MonoBehaviour
{
    public int health = 1;
    public Weapon weapon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FieldOfView fov = gameObject.GetComponent<FieldOfView>();
        if (fov.CanSeePlayer)
        {
            weapon.Fire();
        }

    }

    public void TakeDamage(int damage = 1)
    {
        health -= damage;

        if(health <= 0)
        {
            Destroy(gameObject);
        }    
    }
}

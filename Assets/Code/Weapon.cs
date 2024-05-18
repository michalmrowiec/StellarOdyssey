using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePoint;
    public float fireForce;
    public float fireRate = 1;
    public float nextFireTime = 0;

    public void Fire()
    {
        if(Time.time > nextFireTime)
        {
            GameObject projectile = Instantiate(bullet, firePoint.position, firePoint.rotation);
            projectile.GetComponent<Rigidbody2D>().AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);

            nextFireTime = Time.time + fireRate;
        }

    }
}

using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Weapon : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePoint;
    public float fireForce;
    public float fireRate = 1;
    public float nextFireTime = 0;

    public void Fire(Color bulletColor)
    {
        if(Time.time > nextFireTime)
        {
            bullet.GetComponent<Light2D>().color = bulletColor;

            GameObject projectile = Instantiate(bullet, firePoint.position, firePoint.rotation);
            projectile.GetComponent<Rigidbody2D>().AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);

            nextFireTime = Time.time + fireRate;
        }

    }
}

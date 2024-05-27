using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Weapon : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePoint;
    public float fireForce;
    public float fireRate = 1f;
    public float slowMotionFireRate = 2f;
    public float nextFireTime = 0f;
    public float attackDistance = 2f;
    public bool drawWeaponLaser = false;
    public int currentAmmo = 10;
    public int maxAmmo = 10;
    private float fireRateOriginal;

    private void Start()
    {
        fireRateOriginal = fireRate;
    }

    public void Fire(Color bulletColor, string shootBy = "")
    {
        if (Time.time > nextFireTime
            && currentAmmo > 0)
        {
            bullet.GetComponentInChildren<Light2D>().color = bulletColor;
            bullet.GetComponent<Bullet>().shootBy = shootBy;

            GameObject projectile = Instantiate(bullet, firePoint.position, firePoint.rotation);
            projectile.GetComponent<Rigidbody2D>().AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);

            nextFireTime = Time.time + fireRate;
            currentAmmo--;
        }
    }

    public void UpdateFireRate(bool slowMotionActive)
    {
        if (slowMotionActive)
        {
            fireRate = slowMotionFireRate;
        }
        else
        {
            fireRate = fireRateOriginal;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (drawWeaponLaser)
        {
            Gizmos.color = Color.red;
            Vector3 direction = firePoint.up * 10f;
            Gizmos.DrawRay(firePoint.position, direction);
        }
    }
#endif
}

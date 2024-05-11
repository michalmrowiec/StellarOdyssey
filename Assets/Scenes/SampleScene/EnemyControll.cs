using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyControll : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed = 2f;
    public float rotationSpeed = 1f;
    public int healthPoints = 1;
    public Weapon weapon;
    private FieldOfView fov;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        fov = gameObject.GetComponent<FieldOfView>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NoticedPlayer()
    {
        Vector2 directionToPlayer = (fov.playerRef.transform.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90;

        float angle = Mathf.LerpAngle(rb.rotation, targetAngle, Time.deltaTime * rotationSpeed);
        rb.rotation = angle;

        rb.velocity = directionToPlayer * moveSpeed;

        weapon.Fire();
    }

    private void FixedUpdate()
    {
        if (fov.CanSeePlayer)
        {
            NoticedPlayer();
        }
        else
        {
            Vector2 direction = transform.up;
            rb.velocity = direction * moveSpeed;
        }
    }

    public void TakeDamage(int damage = 1)
    {
        healthPoints -= damage;

        if (healthPoints <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            int randomDirection = Random.Range(0, 1);
            if (randomDirection == 1)
                transform.Rotate(0, 0, -transform.rotation.z);
            else
                transform.Rotate(0, 0, transform.rotation.z - 45);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            int randomDirection = Random.Range(0, 1);
            if (randomDirection == 1)
                transform.Rotate(0, 0, -transform.rotation.z);
            else
                transform.Rotate(0, 0, transform.rotation.z - 45);
        }
    }
}

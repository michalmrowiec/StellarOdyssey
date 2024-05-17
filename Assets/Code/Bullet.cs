using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody2D rb;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Wall":
                Destroy(gameObject);
                break;
            case "Enemy":
                collision.gameObject.GetComponent<EnemyControll>().TakeDamage(1);
                Destroy(gameObject);
                break;
            case "Player":
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(1);
                Destroy(gameObject);
                break;
        }
    }
}

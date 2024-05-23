using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject lightFlashPrefab;
    public string shootBy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //GameObject lightFlash = Instantiate(lightFlashPrefab, transform.position, Quaternion.identity);

        switch (collision.gameObject.tag)
        {
            case "Wall":
                Destroy(Instantiate(lightFlashPrefab, transform.position, Quaternion.identity), 0.02f);
                Destroy(gameObject);
                break;
            case "Enemy":
                if (shootBy != "Enemy")
                    collision.gameObject.GetComponent<EnemyController>().TakeDamage(1);
                Destroy(gameObject);
                break;
            case "Player":
                Destroy(Instantiate(lightFlashPrefab, transform.position, Quaternion.identity), 0.02f);
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(1);
                Destroy(gameObject);
                break;
        }

        //Destroy(Instantiate(lightFlashPrefab, transform.position, Quaternion.identity), 0.02f);
        //Destroy(lightFlash, 0.02f);
    }
}

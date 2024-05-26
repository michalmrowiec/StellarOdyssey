using UnityEngine;
using UnityEngine.Apple;

public class SlowMotion : MonoBehaviour
{
    public float slowMotionTimeScale = 0.1f;
    public float slowMotionPlayerFireRate = 0.01f;
    public float slowMotionPlayerMoveSpeed = 10f;
    public GameObject player;
    private bool isSlowMotionActive = false;
    private float originalFixedDeltaTime;
    private float originalPlayerFireRate;
    private float originalPlayerMoveSpeed;
    public Vector2 playerVelocity;

    void Update()
    {
        if (isSlowMotionActive)
        {
            player.GetComponent<PlayerController>().moveSpeed = slowMotionPlayerMoveSpeed;
            player.GetComponent<PlayerController>().weaponOwner.weapon.fireRate = slowMotionPlayerFireRate;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isSlowMotionActive)
            {
                player.GetComponent<PlayerController>().moveSpeed = originalPlayerMoveSpeed;
                player.GetComponent<PlayerController>().weaponOwner.weapon.fireRate = originalPlayerFireRate;
                ResumeTime();
            }
            else
            {
                ActivateSlowMotion();
            }
        }
    }

    void ActivateSlowMotion()
    {
        originalFixedDeltaTime = Time.fixedDeltaTime;
        originalPlayerFireRate = player.GetComponent<PlayerController>().weaponOwner.weapon.fireRate;
        originalPlayerMoveSpeed = player.GetComponent<PlayerController>().moveSpeed;

        isSlowMotionActive = true;
        Time.timeScale = slowMotionTimeScale;
        Time.fixedDeltaTime = originalFixedDeltaTime * slowMotionTimeScale;
    }

    void ResumeTime()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixedDeltaTime;
        isSlowMotionActive = false;
    }

    private void FixedUpdate()
    {
        playerVelocity = player.GetComponent<PlayerController>().rb.velocity;
    }
}

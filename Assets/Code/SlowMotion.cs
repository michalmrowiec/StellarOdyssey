using UnityEngine;

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isSlowMotionActive)
            {
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

        player.GetComponent<PlayerController>().moveSpeed = slowMotionPlayerMoveSpeed;
        player.GetComponent<PlayerController>().weaponOwner.weapon.fireRate = slowMotionPlayerFireRate;
    }

    void ResumeTime()
    {
        isSlowMotionActive = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixedDeltaTime;

        player.GetComponent<PlayerController>().moveSpeed = originalPlayerMoveSpeed;
        player.GetComponent<PlayerController>().weaponOwner.weapon.fireRate = originalPlayerFireRate;
    }

    private void FixedUpdate()
    {
        playerVelocity = player.GetComponent<PlayerController>().rb.velocity;
    }
}

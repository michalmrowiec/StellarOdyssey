using System;
using UnityEngine;

public class SlowMotion : MonoBehaviour
{
    public float slowMotionTimeScale = 0.1f;
    public float slowMotionPlayerMoveSpeed = 10f;
    public GameObject player;
    public static bool isSlowMotionActive = false;
    private float originalFixedDeltaTime;
    private float originalPlayerMoveSpeed;
    public Vector2 playerVelocity;

    public static event Action<bool> OnSlowMotionChanged;

    private void OnEnable()
    {
        Menu.OnRestartGame += ResumeTime;
    }

    void Update()
    {
        if (isSlowMotionActive)
        {
            player.GetComponent<PlayerController>().moveSpeed = slowMotionPlayerMoveSpeed;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isSlowMotionActive)
            {
                player.GetComponent<PlayerController>().moveSpeed = originalPlayerMoveSpeed;
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
        originalPlayerMoveSpeed = player.GetComponent<PlayerController>().moveSpeed;

        isSlowMotionActive = true;
        Time.timeScale = slowMotionTimeScale;
        Time.fixedDeltaTime = originalFixedDeltaTime * slowMotionTimeScale;
        OnSlowMotionChanged(true);
    }

    void ResumeTime()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixedDeltaTime;
        isSlowMotionActive = false;
        OnSlowMotionChanged(false);
    }

    private void FixedUpdate()
    {
        playerVelocity = player.GetComponent<PlayerController>().rb.velocity;
    }
}

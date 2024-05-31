using System;
using TMPro;
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
    public TextMeshProUGUI slowMotionTimeText;
    public float slowMotionTime = 5f;
    private bool gamePaused = false;

    public static event Action<bool> OnSlowMotionChanged;

    private void OnEnable()
    {
        Menu.OnRestartGame += ResumeTime;
        Menu.OnPauseGame += GamePausedUpdate;
    }

    private void OnDisable()
    {
        Menu.OnRestartGame -= ResumeTime;
        Menu.OnPauseGame += GamePausedUpdate;
    }

    private void Start()
    {
        slowMotionTimeText.text = Mathf.Round(slowMotionTime).ToString();
    }

    private void GamePausedUpdate(bool isPaused)
    {
        gamePaused = isPaused;
    }

    void Update()
    {

        if (slowMotionTime <= 0)
        {
            player.GetComponent<PlayerController>().moveSpeed = originalPlayerMoveSpeed;
            ResumeTime();
            return;
        }

        if (isSlowMotionActive && !gamePaused)
        {
            player.GetComponent<PlayerController>().moveSpeed = slowMotionPlayerMoveSpeed;

            slowMotionTime -= Time.unscaledDeltaTime;
            slowMotionTimeText.text = Mathf.Round(slowMotionTime).ToString();
        }

        if (Input.GetKeyDown(KeyCode.Space) && slowMotionTime > 0)
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

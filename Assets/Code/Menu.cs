using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public GameObject playUi;
    public GameObject winMenu;
    public bool gamePoused = false;
    public static event Action OnRestartGame;
    public static event Action<bool> OnPauseGame;
    private AudioMenager audioMenager;
    private float timeScaleTemp = 1f;

    private void Start()
    {
        Time.timeScale = 1f;
        ArtifactController.OnPickUpChapter += ChapterCompleted;
        audioMenager = FindObjectOfType<AudioMenager>();
    }

    private void OnDisable()
    {
        ArtifactController.OnPickUpChapter -= ChapterCompleted;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.activeInHierarchy)
            {
                ResumeGame();
            }
            else
            {
                gamePoused = true;
                PauseGame();
            }
        }
        else if(Input.GetKeyDown(KeyCode.R) && (pauseMenu.activeInHierarchy || gameOverMenu.activeInHierarchy))
        {
            RestartGame();
        }

        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().healthPoints <= 0)
        {
            gamePoused = true;
            GameOver();
        }
    }

    public void ChapterCompleted()
    {
        gamePoused = true;
        OnPauseGame(true);
        Time.timeScale = 0;
        playUi.SetActive(false);
        winMenu.SetActive(true);
    }

    public void MainMenu()
    {
        SceneManager.LoadSceneAsync(0);
        Time.timeScale = 1;
        timeScaleTemp = 1f;
    }

    public void PauseGame()
    {
        timeScaleTemp = Time.timeScale;
        OnPauseGame(true);
        Time.timeScale = 0;
        playUi.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        playUi.SetActive(false);
        gameOverMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        OnPauseGame(false);
        //Time.timeScale = 1;
        Time.timeScale = timeScaleTemp;
        pauseMenu.SetActive(false);
        gamePoused = false;
        playUi.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        OnRestartGame();
        //Time.timeScale = 1;
        //gamePoused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public bool gamePoused = false;

    private void Start()
    {
        Time.timeScale = 1f;
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

    public void MainMenu()
    {
        SceneManager.LoadSceneAsync(0);
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        gameOverMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        gamePoused = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

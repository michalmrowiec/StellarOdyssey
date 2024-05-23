using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject pauseMenu; // Referencja do obiektu panelu menu

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
                PauseGame();
            }
        }
        else if(Input.GetKeyDown(KeyCode.R) && pauseMenu.activeInHierarchy)
        {
            RestartGame();
        }

        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().healthPoints <= 0)
        {
            PauseGame();
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadSceneAsync(0);
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        Time.timeScale = 0; // Zatrzymuje czas w grze
        pauseMenu.SetActive(true); // Wyœwietla menu
    }

    public void ResumeGame()
    {
        Time.timeScale = 1; // Wznawia czas w grze
        pauseMenu.SetActive(false); // Ukrywa menu
    }

    public void QuitGame()
    {
        Application.Quit(); // Zamyka grê
    }

    public void RestartGame()
    {
        Time.timeScale = 1; // Wznawia czas w grze
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Restartuje aktualn¹ scenê
    }
}

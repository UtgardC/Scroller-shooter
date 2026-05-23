using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private bool hideScreensOnStart = true;

    private void Start()
    {
        Time.timeScale = 1f;

        if (!hideScreensOnStart)
        {
            return;
        }

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }

        if (winScreen != null)
        {
            winScreen.SetActive(false);
        }
    }

    public void SetLives(int lives)
    {
        if (livesText != null)
        {
            livesText.text = lives.ToString();
        }
    }

    public void DoGameOver()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void DoWin()
    {
        if (winScreen != null)
        {
            winScreen.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene activeScene = SceneManager.GetActiveScene();

        if (activeScene.buildIndex >= 0)
        {
            SceneManager.LoadScene(activeScene.buildIndex);
            return;
        }

        SceneManager.LoadScene(activeScene.name);
    }
}

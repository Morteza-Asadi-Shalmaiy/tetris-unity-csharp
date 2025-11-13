using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public TextMeshProUGUI scoresText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI levelText;

    public void Setup(int score, int highScore, int level)
    {
        gameObject.SetActive(true);
        scoresText.text = "Score : " + score.ToString();
        levelText.text = "Level : " + level.ToString();
        highScoreText.text = "High Score : " + highScore.ToString();

    }

    public void Restart()
    {
        SceneManager.LoadSceneAsync(1);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBackgroundMusic(); // Resume music
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
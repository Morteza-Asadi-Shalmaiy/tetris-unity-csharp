// Modify MainMenu.cs to include options panel control
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel; // Reference to your options panel

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBackgroundMusic();
        }
        
        // Ensure options panel is hidden at start
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }
    
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(2);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void ShowOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
    }
    
    public void HideOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }
}
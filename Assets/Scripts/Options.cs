using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    
    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            musicVolumeSlider.value = AudioManager.Instance.musicVolume;
            sfxVolumeSlider.value = AudioManager.Instance.sfxVolume;
        }
        
        musicVolumeSlider.onValueChanged.AddListener(delegate { UpdateMusicVolume(); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { UpdateSFXVolume(); });
    }
    
    private void UpdateMusicVolume()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(musicVolumeSlider.value);
        }
    }
    
    private void UpdateSFXVolume()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(sfxVolumeSlider.value);
        }
    }
}
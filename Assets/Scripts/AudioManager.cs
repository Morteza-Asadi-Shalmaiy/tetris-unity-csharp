using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Music")]
    public AudioClip backgroundMusic;
    public AudioClip gameOverMusic;
    private AudioSource musicSource;
    
    [Header("Sound Effects")]
    public AudioClip lineClearSound;
    public AudioClip tetrominoRotateSound;
    public AudioClip tetrominoMoveSound;
    public AudioClip tetrominoLandSound;
    public AudioClip tetrominoHardDropSound;
    private AudioSource sfxSource;
    
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.7f;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Create two audio sources
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.volume = sfxVolume;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic == null) return;
        
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }
    
    public void PlayGameOverMusic()
    {
        if (gameOverMusic == null) return;
        
        musicSource.Stop();
        musicSource.clip = gameOverMusic;
        musicSource.loop = false;
        musicSource.Play();
    }
    
    public void PlaySoundEffect(AudioClip clip)
    {
        if (clip == null) return;
        
        sfxSource.PlayOneShot(clip);
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = volume;
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = volume;
    }
    public void AdjustMusicTempo(float speedMultiplier)
    {
        musicSource.pitch = Mathf.Clamp(1.0f + (speedMultiplier - 1) * 0.5f, 0.8f, 1.5f);
    }
}
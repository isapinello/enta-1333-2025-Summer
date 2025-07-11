using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip buildingPlaceSound;
    [SerializeField] private AudioClip unitSelectSound;
    [SerializeField] private AudioClip backgroundMusic;

    [Header("Pooling")]
    [SerializeField] private AudioSource pooledSourcePrefab;
    [SerializeField] private int poolSize = 10;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;

    private AudioSource[] pool;
    private int poolIndex = 0;


    private void Awake()
    {

        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        pool = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            var source = Instantiate(pooledSourcePrefab, transform);
            pool[i] = source;
        }
        PlayBackgroundMusic();
    }
    public void PlayBackgroundMusic(AudioClip clip = null)
    {
        if (clip != null) backgroundMusic = clip;

        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
    private void PlaySound(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;

        pool[poolIndex].clip = clip;
        pool[poolIndex].transform.position = position;
        pool[poolIndex].Play();

        poolIndex = (poolIndex + 1) % poolSize;
    }

    // Public shortcuts
    public void PlayButtonSound()
    {
        PlaySound(buttonClickSound, Camera.main.transform.position);
    }

    public void PlayBuildingPlaceSound()
    {
        PlaySound(buildingPlaceSound, Camera.main.transform.position); 
    }

    public void PlayUnitSelectSound()
    {
        PlaySound(unitSelectSound, Camera.main.transform.position);
    }
}

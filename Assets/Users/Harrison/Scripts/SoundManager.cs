using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[RequireComponent(typeof(AudioSource))]

public class SoundManager : MonoBehaviour
{
    public static SoundManager INSTANCE;
    private AudioSource _audioSource;
    public enum MusicContexts
    {
        Menu,
        GameLost,
        GameWon,
        Gameplay
    }
    [SerializeField] private SerializedDictionary<MusicContexts, AudioClip> _musicDictionary = new SerializedDictionary<MusicContexts, AudioClip>();
    private void Awake()
    {
        if (INSTANCE != null)
        {
            Destroy(gameObject);
            return;
        }
        INSTANCE = this;
        _audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }
    public void PlaySound(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }

    public void PlayMusic(MusicContexts context)
    {
        _audioSource.resource = _musicDictionary[context];
        _audioSource.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using static UnityEditor.Timeline.TimelinePlaybackControls;

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
        Gameplay,
    }

    public enum GameState
    {
        Menu,
        Paused, 
        GamePlay,
        Won,
        Lost, 
    }

    public enum SFX
    {
        buying,
        selling,
        switching,
        buttonClick,
    }
    public AudioClip buying, selling, switching, buttonClick;

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

    public void PlaySFX(SFX sfx)
    {
        switch(sfx)
        {
            case SFX.buying:
                PlaySound(buying);
                break;
            case SFX.selling:
                PlaySound(selling);
                break;
            case SFX.switching:
                PlaySound(switching);
                break;
            case SFX.buttonClick:
                PlaySound(buttonClick);
                break;
        }
    }

    public void PlayMusic(MusicContexts context)
    {
        _audioSource.resource = _musicDictionary[context];
        _audioSource.Play();
    }

    public void StateChange(GameState state)
    {
        switch (state)
        {
            case GameState.GamePlay:
                PlayMusic(MusicContexts.Gameplay);
                break;
            case GameState.Menu:
                PlayMusic(MusicContexts.Menu);
                break;
            case GameState.Lost:
                PlayMusic(MusicContexts.GameLost);
                break;

        }
    }
}

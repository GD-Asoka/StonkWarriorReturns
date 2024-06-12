using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[RequireComponent(typeof(AudioSource))]

public class SoundManager : MonoBehaviour
{
    public float waitTime = 0.5f;
    public static SoundManager INSTANCE;
    private AudioSource _audioSource;
    public enum MusicContexts
    {
        Menu,
        GameLost,
        GameWon,
        Gameplay,
        News,
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
        illegalAction,
        buttonClickYes,
        buttonClickNo,
        news,
    }
    public AudioClip buying, selling, switching, cantBuy, news, firstNews, gameOver;

    [SerializeField] private SerializedDictionary<MusicContexts, AudioClip> _musicDictionary = new SerializedDictionary<MusicContexts, AudioClip>();
    private void Awake()
    {
        //if (INSTANCE != null)
        //{
        //    Destroy(gameObject);
        //    return;
        //}
        INSTANCE = this;
        _audioSource = GetComponent<AudioSource>();
        //DontDestroyOnLoad(gameObject);
    }

    private bool canPlay = true;

    //private void Update()
    //{
    //    if(Input.GetKey(KeyCode.Backspace) && canPlay)
    //    {
    //        StartCoroutine(PlaySound(selling));
    //    }
    //}

    private IEnumerator PlaySound(AudioClip clip)
    {
        canPlay = false;
        _audioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length * waitTime);
        canPlay = true;
    }

    public void PlaySFX(SFX sfx)
    {
        if (!canPlay) return;
        switch(sfx)
        {
            case SFX.buying:
                StartCoroutine(PlaySound(buying));
                break;
            case SFX.selling:
                StartCoroutine(PlaySound(selling));
                break;
            case SFX.switching:
                StartCoroutine(PlaySound(switching));
                break;
            case SFX.illegalAction:
                StartCoroutine(PlaySound(cantBuy));
                break;
            case SFX.buttonClickYes:
                StartCoroutine(PlaySound(buying));
                break;
            case SFX.buttonClickNo:
                StartCoroutine(PlaySound(selling));
                break;
        }
    }

    public void PlayMusic(MusicContexts context)
    {
        _audioSource.resource = _musicDictionary[context];
        _audioSource.Play();
    }

    public void PlayFirstNews()
    {
        StartCoroutine(FirstNews());
    }
    
    public void PlayNews()
    {
        _audioSource.PlayOneShot(news);
    }
    
    public void GameOver()
    {
        _audioSource.resource = gameOver;
        _audioSource.Play();
    }

    private IEnumerator FirstNews()
    {
        var temp = _audioSource.resource;
        _audioSource.resource = firstNews;
        _audioSource.Play();
        yield return new WaitForSeconds(firstNews.length);
        _audioSource.resource = temp;
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

    public void ToggleAudio()
    {
        if(_audioSource.volume <= 0)   
            _audioSource.volume = 1;
        else if(_audioSource.volume >= 1)
            _audioSource.volume = 0;
    }
}

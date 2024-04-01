using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    public AudioMixer audioMixer;
    private float BGMVol;
    private float SFXVol;

    public bool isPlayingChase = false;
    public AudioClip lastPlayedBGM;

    private float timer = 0;
    private bool isPlayingSomething = true;
    public int chaseCounter = 0;

    /* Experimental */
    private float bpm = 120f;
    private float bps;
    private float timeTillNextBeat;


    [Header("Global Audio Sources")]
    [SerializeField] public AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Global Audio Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip levelOneMusic;
    public AudioClip levelTwoMusic;
    public AudioClip gameLose;
    public AudioClip gameWin;
    public AudioClip clickSfx;
    public AudioClip hoverSfx;
    public AudioClip scrollSfx;
    public AudioClip openInventory;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        GetInitialMixerVol();
        bps = bpm / 60f;

        switch (GameController.instance.currentScene)
        {
            case "Level 1":
            case "Level 2":
                PlayBGM(levelOneMusic);
                break;
            default:
                PlayBGM(mainMenuMusic);
                break;
        }
    }

    private void Update()
    {
/*        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (!isPlayingSomething && timer <= 0)
        {
            isPlayingSomething = true;
            CrossFadeBGM(levelOneMusic);
        }*/

        switch (chaseCounter)
        {
            case int n when (n <= 0):
                if (isPlayingChase)
                {
                    timeTillNextBeat = musicSource.time % bps;
                    Debug.Log(timeTillNextBeat);
                    if (timeTillNextBeat < 0.1f) timeTillNextBeat = 0;
                    if (timeTillNextBeat == 0)
                    {
                        CrossFadeBGM(levelOneMusic);
                        isPlayingChase = false;
                        isPlayingSomething = false;
                        timer = 10;
                    }
                }
                break;
            case int n when (n >= 1):
                if (!isPlayingChase)
                {
                    timer = 0;
                    isPlayingSomething = true;
                    CrossFadeBGM(levelTwoMusic);
                    isPlayingChase = true;
                }
                break;
            default:
                break;
        }
    }

    public void GetInitialMixerVol()
    {
        audioMixer.GetFloat("BGM", out BGMVol);
        audioMixer.GetFloat("SFX", out SFXVol);
    }

    public void CrossFadeBGM(AudioClip clip)
    {
        if (!isPlayingChase)
        {
            DetermineLastPlayedBGM(musicSource.clip);
        }
        StartCoroutine(XFade(clip, 1f));
    }

    public void DetermineLastPlayedBGM(AudioClip clip)
    {
        lastPlayedBGM = clip;
    }

    IEnumerator XFade(AudioClip clip, float volume)
    {

        ///Add new audiosource and set it to all parameters of original audiosource
        AudioSource fadeOutSource;
        if (gameObject.GetComponent<AudioSource>())
        {
            fadeOutSource = gameObject.GetComponent<AudioSource>();
        }
        else
        {
            fadeOutSource = gameObject.AddComponent<AudioSource>();
        }
        fadeOutSource.clip = gameObject.transform.Find("BGM").gameObject.GetComponent<AudioSource>().clip;
        fadeOutSource.time = gameObject.transform.Find("BGM").gameObject.GetComponent<AudioSource>().time;
        fadeOutSource.volume = gameObject.transform.Find("BGM").gameObject.GetComponent<AudioSource>().volume;
        fadeOutSource.outputAudioMixerGroup = gameObject.transform.Find("BGM").gameObject.GetComponent<AudioSource>().outputAudioMixerGroup;

        //make it start playing
        fadeOutSource.Play();

        //set original audiosource volume and clip
        gameObject.transform.Find("BGM").gameObject.GetComponent<AudioSource>().volume = 0f;
        gameObject.transform.Find("BGM").gameObject.GetComponent<AudioSource>().clip = clip;
        float t = 0;
        float v = fadeOutSource.volume;
        gameObject.transform.Find("BGM").gameObject.GetComponent<AudioSource>().Play();

        //begin fading in original audiosource with new clip as we fade out new audiosource with old clip
        while (t < 0.98f)
        {
            t = Mathf.Lerp(t, 1f, Time.deltaTime * 0.5f);
            if (fadeOutSource)
            {
                fadeOutSource.volume = Mathf.Lerp(v, 0f, t);
            }
            gameObject.transform.Find("BGM").gameObject.GetComponent<AudioSource>().volume = Mathf.Lerp(0f, volume, t);
            yield return null;
        }
        gameObject.transform.Find("BGM").gameObject.GetComponent<AudioSource>().volume = volume;
        //destroy the fading audiosource
        Destroy(fadeOutSource);
        yield break;
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayBGM(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }
}

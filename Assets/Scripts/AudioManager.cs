using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

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

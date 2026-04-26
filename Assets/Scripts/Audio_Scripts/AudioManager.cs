using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    [Header("Player SFX")]
    public AudioClip jumpSFX;
    public AudioClip dashSFX;
    public AudioClip runSFX;
    public AudioClip attackSFX;
    public AudioClip takeDamageSFX;
    public AudioClip healSFX;

    [Header("Combat SFX")]
    public AudioClip hitEnemySFX;
    public AudioClip killEnemySFX;
    public AudioClip projectileShootSFX;
    public AudioClip projectileImpactSFX;
    public AudioClip wallBreakSFX;

    [Header("World SFX")]
    public AudioClip coinPickupSFX;
    public AudioClip npcVoiceSFX;
    public AudioClip dialogueBlipSFX;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        PlayMusic(gameMusic);
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
            PlayMusic(menuMusic);
        else
            PlayMusic(gameMusic);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        if (sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource == null) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
using UnityEngine;

public class BackgroundMusicController : MonoBehaviour
{
    [SerializeField] private AudioClip backgroundMusic;
    private AudioSource audioSource;

    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.5f;

    [SerializeField] private bool playOnAwake = true;
    [SerializeField] private bool dontDestroyOnLoad = true;

    private static BackgroundMusicController instance;

private void Awake()
{
    audioSource = gameObject.AddComponent<AudioSource>();
    audioSource.loop = true;
    audioSource.volume = musicVolume;

    audioSource.clip = backgroundMusic != null
        ? backgroundMusic
        : Resources.Load<AudioClip>("musica");

    if (playOnAwake && audioSource.clip != null)
    {
        audioSource.Play();
    }
}

    public void PlayMusic()
    {
        if (audioSource != null && audioSource.clip != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp01(volume);
        }
    }
}
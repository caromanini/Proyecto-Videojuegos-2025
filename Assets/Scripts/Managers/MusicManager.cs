using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip winMusic;
    [SerializeField] private AudioClip loseMusic;

    [Header("Volume Settings")]
    [SerializeField][Range(0f, 1f)] private float bgmVolume = 0.3f;

    void Start()
    {
        InitializeAudio();
        SubscribeToEvents();
        StartBGM();
    }

    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void InitializeAudio()
    {
        if (bgmAudioSource == null)
        {
            bgmAudioSource = gameObject.AddComponent<AudioSource>();
        }

        bgmAudioSource.loop = true;
        bgmAudioSource.volume = bgmVolume;
        bgmAudioSource.playOnAwake = false;
    }

    private void SubscribeToEvents()
    {
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.OnPlayerWin += OnPlayerWin;
            GameEventManager.Instance.OnPlayerLose += OnPlayerLose;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.OnPlayerWin -= OnPlayerWin;
            GameEventManager.Instance.OnPlayerLose -= OnPlayerLose;
        }
    }

    public void StartBGM()
    {
        if (bgmAudioSource == null) return;

        if (gameplayMusic != null)
        {
            bgmAudioSource.clip = gameplayMusic;
        }

        if (!bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Play();
        }
    }

    public void StopBGM()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
        }
    }

    // Me falta en Unity elegir un tipo de win music
    private void OnPlayerWin()
    {
        StopBGM();

        if (winMusic != null)
        {
            PlayOneShot(winMusic);
        }
    }

    // Me falta en Unity elegir un tipo de lose music
    private void OnPlayerLose()
    {
        StopBGM();

        if (loseMusic != null)
        {
            PlayOneShot(loseMusic);
        }
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (bgmAudioSource != null && clip != null)
        {
            bgmAudioSource.PlayOneShot(clip);
        }
    }

    public void SetVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = bgmVolume;
        }
    }

    public void PauseBGM()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Pause();
        }
    }

    public void ResumeBGM()
    {
        if (bgmAudioSource != null && !bgmAudioSource.isPlaying)
        {
            bgmAudioSource.UnPause();
        }
    }
}
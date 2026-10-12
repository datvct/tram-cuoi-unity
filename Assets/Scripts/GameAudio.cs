using UnityEngine;

public class GameAudio : MonoBehaviour
{
    public static GameAudio Instance { get; private set; }

    [Header("Âm thanh")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip enemyDeathSound;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;

    [Header("Âm lượng")]
    [SerializeField, Range(0f, 1f)]
    private float effectsVolume = 0.6f;

    [SerializeField, Range(0f, 1f)]
    private float uiVolume = 0.7f;

    private AudioSource effectsSource;
    private AudioSource uiSource;

    private void Awake()
    {
        // Chỉ giữ một bộ quản lý khi tải lại scene.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        effectsSource = gameObject.AddComponent<AudioSource>();
        ConfigureSource(effectsSource);

        uiSource = gameObject.AddComponent<AudioSource>();
        ConfigureSource(uiSource);

        // Nút giao diện vẫn có tiếng khi game tạm dừng.
        uiSource.ignoreListenerPause = true;

        AudioListener.pause = false;
    }

    private void ConfigureSource(AudioSource source)
    {
        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f;
        source.pitch = 1f;
        source.volume = 1f;
    }

    private void PlayEffect(AudioClip clip)
    {
        if (clip != null)
            effectsSource.PlayOneShot(clip, effectsVolume);
    }

    public void PlayHit()
    {
        PlayEffect(hitSound);
    }

    public void PlayEnemyDeath()
    {
        PlayEffect(enemyDeathSound);
    }

    public void PlayClick()
    {
        if (clickSound != null)
            uiSource.PlayOneShot(clickSound, uiVolume);
    }

    public void PlayResult(bool won)
    {
        // Kết thúc trận: bỏ các tiếng trúng đạn đang chồng nhau.
        effectsSource.Stop();

        AudioClip clip = won ? winSound : loseSound;

        if (clip != null)
            uiSource.PlayOneShot(clip, uiVolume);
    }

    public void StopGameplaySounds()
    {
        effectsSource.Stop();
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        Instance = null;
        AudioListener.pause = false;
    }
}
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource sfxSource;

    [Header("SFX")]
    public AudioClip jumpClip;
    public AudioClip dashClip;
    public AudioClip menuClickClip;
    public AudioClip enemyHitClip;
    public AudioClip enemyDeathClip;
    public AudioClip projectileClip;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Play(AudioClip clip, float volume = 1f)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip, volume);
    }

    public void Jump() => Play(jumpClip);
    public void Dash() => Play(dashClip);
    public void MenuClick() => Play(menuClickClip);
    public void EnemyHit() => Play(enemyHitClip);
    public void EnemyDeath() => Play(enemyDeathClip);
    public void ProjectileShot() => Play(projectileClip);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAudioManager : MonoBehaviour
{
    public static BattleAudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource uiSource;

    [Header("Music")]
    public AudioClip battleMusic;
    public AudioClip victoryMusic;
    public AudioClip defeatMusic;
    [Range(0f, 1f)] public float musicVolume = 0.7f;

    [Header("SFX")]
    public AudioClip[] hitSounds;
    public AudioClip[] critSounds;
    public AudioClip healSound;
    public AudioClip skillActivateSound;
    public AudioClip ultimateActivateSound;
    public AudioClip turnStartSound;
    public AudioClip enemyDeathSound;
    public AudioClip playerDeathSound;
    public AudioClip weaknessBreakSound;
    public AudioClip buttonClickSound;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        PlayBattleMusic();
    }

    #region Music

    public void PlayBattleMusic()
    {
        if (musicSource == null || battleMusic == null) return;
        musicSource.clip = battleMusic;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void PlayVictoryMusic()
    {
        if (musicSource == null) return;
        StartCoroutine(FadeMusicAndPlay(victoryMusic, false));
    }

    public void PlayDefeatMusic()
    {
        if (musicSource == null) return;
        StartCoroutine(FadeMusicAndPlay(defeatMusic, false));
    }

    private IEnumerator FadeMusicAndPlay(AudioClip newClip, bool loop)
    {
        if (musicSource == null) yield break;
        float startVol = musicSource.volume;
        float t = 0;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVol, 0, t / 0.5f);
            yield return null;
        }
        if (newClip != null)
        {
            musicSource.clip = newClip;
            musicSource.loop = loop;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    #endregion

    #region SFX

    public void PlayHitSound(bool isCrit = false)
    {
        if (isCrit && critSounds.Length > 0)
            PlayRandomSFX(critSounds);
        else if (hitSounds.Length > 0)
            PlayRandomSFX(hitSounds);
    }

    public void PlayHealSound() => PlaySFX(healSound);
    public void PlaySkillSound() => PlaySFX(skillActivateSound);
    public void PlayUltimateSound() => PlaySFX(ultimateActivateSound);
    public void PlayTurnStartSound() => PlaySFX(turnStartSound);
    public void PlayEnemyDeathSound() => PlaySFX(enemyDeathSound);
    public void PlayPlayerDeathSound() => PlaySFX(playerDeathSound);
    public void PlayWeaknessBreakSound() => PlaySFX(weaknessBreakSound);
    public void PlayButtonClick() => PlayUI(buttonClickSound);

    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }

    private void PlayUI(AudioClip clip)
    {
        if (uiSource != null && clip != null)
            uiSource.PlayOneShot(clip);
    }

    private void PlayRandomSFX(AudioClip[] clips)
    {
        if (clips.Length == 0) return;
        PlaySFX(clips[Random.Range(0, clips.Length)]);
    }

    #endregion
}

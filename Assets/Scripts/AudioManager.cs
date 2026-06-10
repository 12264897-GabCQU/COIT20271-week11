using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Source")]
    public AudioSource sfxSource;

    [Header("Sound Effects")]
    public AudioClip shootSound;
    public AudioClip playerHitSound;
    public AudioClip scoreSound;
    public AudioClip gameOverSound;
    public AudioClip ballMoveSound;

    private bool ballMovePlaying = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayShoot()
    {
        PlaySound(shootSound);
    }

    public void PlayPlayerHit()
    {
        PlaySound(playerHitSound);
    }

    public void PlayScore()
    {
        PlaySound(scoreSound);
    }

    public void PlayGameOver()
    {
        PlaySound(gameOverSound);
    }

    public void PlayBallMove()
    {
        if (ballMoveSound == null || sfxSource == null || ballMovePlaying) return;

        sfxSource.clip = ballMoveSound;
        sfxSource.loop = true;
        sfxSource.Play();

        ballMovePlaying = true;
    }

    public void StopBallMove()
    {
        if (sfxSource == null || !ballMovePlaying) return;

        sfxSource.Stop();
        sfxSource.clip = null;
        sfxSource.loop = false;

        ballMovePlaying = false;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.PlayOneShot(clip);
    }
}
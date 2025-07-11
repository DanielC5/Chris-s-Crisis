using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private AudioSource audioPlayer;

    public AudioMixer gameMixer;

    public AudioClip deathSFX;
    public AudioClip respawnSFX;
    public AudioClip gameOverSFX;
    public AudioClip buttonSFX;
    public AudioClip carStartSFX;
    public AudioClip pickupSFX;
    // public AudioClip enemyDeathSFX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        audioPlayer = GetComponent<AudioSource>();
    }


    // SFX functions
    public void DeathSFX()
    {
        audioPlayer.PlayOneShot(deathSFX);
    }

    public void RespawnSFX()
    {
        audioPlayer.PlayOneShot(respawnSFX);
    }

    public void GameOverSFX()
    {
        audioPlayer.PlayOneShot(gameOverSFX);
    }

    public void ButtonSFX()
    {
        audioPlayer.PlayOneShot(buttonSFX);
    }

    public void CarStartSFX()
    {
        audioPlayer.PlayOneShot(carStartSFX);
    }

    public void PickupSFX()
    {
        audioPlayer.PlayOneShot(pickupSFX);
    }

    /*
    public void EnemyDeathSFX()
    {
        audioPlayer.PlayOneShot(enemyDeathSFX);
    }
    */

    // Mixer functions
    public void NotPlayingSnapshot()
    {
        gameMixer.FindSnapshot("Not Playing").TransitionTo(0f);
    }

    public void IsPlayingSnapshot()
    {
        gameMixer.FindSnapshot("Is Playing").TransitionTo(0f);
    }
}

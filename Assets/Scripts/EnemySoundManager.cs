using UnityEngine;
using UnityEngine.Audio;

public class EnemySoundManager : MonoBehaviour
{
    public static EnemySoundManager Instance;

    private AudioSource audioPlayer;

    public AudioMixer gameMixer;

    public AudioClip enemyDeathSFX;

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
    
    public void EnemyDeathSFX()
    {
        audioPlayer.PlayOneShot(enemyDeathSFX);
    }

    /*
    // Mixer functions
    public void NotPlayingSnapshot()
    {
        gameMixer.FindSnapshot("Not Playing").TransitionTo(0f);
    }

    public void IsPlayingSnapshot()
    {
        gameMixer.FindSnapshot("Is Playing").TransitionTo(0f);
    }
    */
}

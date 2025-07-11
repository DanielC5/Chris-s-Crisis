using UnityEngine;

[RequireComponent(typeof(PolarTransform), typeof(SpriteRenderer))]
public class BossController : MonoBehaviour
{
    public float fadeInTime = 1.5f;

    public int health = 50;

    public int scoreValue;

    [Header("Spawn Minion")]
    public GameObject minionPrefab;
    public float spawnInterval = 2.5f;

    PolarTransform polar;
    SpriteRenderer sr;

    float fadeTimer;
    bool fadeDone;
    float spawnTimer;

    private AudioSource src;
    public AudioClip bossSpawnSFX;

    void Awake()
    {
        src = GetComponent<AudioSource>();

        polar = GetComponent<PolarTransform>();
        polar.radius = 0f;
        sr = GetComponent<SpriteRenderer>();

        Color c = sr.color; c.a = 0f; sr.color = c;
    }

    void Update()
    {
        if (!fadeDone)
        {
            fadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(fadeTimer / fadeInTime);
            Color c = sr.color; c.a = t; sr.color = c;
            if (t >= 1f)
            {
                src.PlayOneShot(bossSpawnSFX);
                fadeDone = true;
            }    
            return;
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnMinion();
        }
    }

    void SpawnMinion()
    {
        if (!minionPrefab)
            return;

        GameObject m = Instantiate(minionPrefab, transform.position, Quaternion.identity);
    }

    public void Damage()
    {
        health -= 1;
        if (health <= 0)
        {
            //make some noise perhaps

            //manually set level clear

            GameManager.Instance.addScore(scoreValue);
            Debug.Log("hitting boss!");

            Destroy(gameObject);
        }
    }

}
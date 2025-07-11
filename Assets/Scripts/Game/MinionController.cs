using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 进入 -> 圆周对位 -> 呼吸 & 旋转停留(带抖动) -> (FIFO) 退出
/// </summary>
[RequireComponent(typeof(PolarTransform))]
public class MinionController : MonoBehaviour
{
    [Header("Radius Parameters")]
    public float startRadius = 10f;
    public float targetRadius = 3f;   // RingRadius

    [Header("Angle Curves")]
    public AnimationCurve enterAngleCurve = AnimationCurve.Linear(0, 0, 2, 0);
    public AnimationCurve exitAngleCurve = AnimationCurve.Linear(0, 0, 2, 0);

    [Header("Time")]
    public float enterTime = 2.0f;
    public float exitTime = 2.0f;

    [Header("Align")]
    public float alignSpeedDeg = 120f;
    public float alignThreshold = 1f;

    [Header("Jitter")]
    public float jitterRadius = 0.15f;
    public float jitterAngleDeg = 2f;
    public float jitterFrequency = 3f;

    [Header("Zoom")]
    public float minScale = 0.3f;
    public float maxScale = 1.0f;

    [Header("Points")]
    public int pointsGiven;

    [Header("Attack")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 1.5f;
    public float bulletLife = 5f;

    enum Phase { Enter, Align, Stay, Exit, Done }
    Phase phase = Phase.Exit;

    PolarTransform polar;
    Vector3 initLocalScale;
    float timer;

    float bulletTimer;
    float baseAngleDeg;

    int slotIndex = -1;
    float slotBaseAngleDeg = 0f;

    float jitterOffset;

    Animator animator;


    // sound :)
    private AudioSource src;
    public AudioClip enemyMovementSFX;
    public AudioClip enemyShootSFX;
    void Awake()
    {
        polar = GetComponent<PolarTransform>();
        initLocalScale = transform.localScale;

        baseAngleDeg = polar.angleDeg + Random.Range(0f,360f);
        polar.radius = targetRadius;

        jitterOffset = Random.Range(0f, 2f * Mathf.PI);
        UpdateScale();

        src = GetComponent<AudioSource>();

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bulletTimer += Time.deltaTime;
        switch (phase)
        {
            case Phase.Exit: ExitStep(); break;
        }

        animator.SetFloat("radius", polar.radius);
    }


    void ExitStep()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / Mathf.Pow(exitTime, 2));

        polar.radius = Mathf.Lerp(targetRadius, startRadius, t);
        polar.angleDeg = baseAngleDeg + exitAngleCurve.Evaluate(t) * 360f;

        UpdateScale();
        if (t >= 1f) { phase = Phase.Done; Destroy(gameObject); }
    }

    void UpdateScale()
    {
        float s = Mathf.Lerp(minScale, maxScale, polar.radius / startRadius);
        transform.localScale = initLocalScale * s;
    }

    public void SetSlot(int index)
    {
        slotIndex = index;
        slotBaseAngleDeg = 360f * index / InGameManager.Instance.stackCapacity;
        targetRadius = InGameManager.RingRadius;
    }

    public void TriggerExit()
    {
        src.PlayOneShot(enemyMovementSFX);

        if (phase != Phase.Stay) return;
        phase = Phase.Exit;
        timer = 0f;
        InGameManager.Instance?.ReleaseSlot(gameObject);
    }

    void OnDestroy()
    {
        if (InGameManager.Instance) InGameManager.Instance.ReleaseSlot(gameObject);
        EnemySoundManager.Instance.EnemyDeathSFX();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && (GameManager.Instance.CurrentState == GameManager.GameState.Playing) && collision.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.OnHit();
        }
    }

    void FireBullet()
    {
        if (bulletPrefab == null) return;
        
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // SFX code
        // src.PlayOneShot(enemyShootSFX);

        PolarTransform playerPol = GetComponent<PolarTransform>();
        PolarTransform bulPol = bullet.GetComponent<PolarTransform>();
        bulPol.center = playerPol.center;
        bulPol.FromWorldPosition(transform.position);
        
        BulletBehaviour bb = bullet.GetComponent<BulletBehaviour>();
        bb.Init(BulletBehaviour.Motion.RadialOut, bulletSpeed, bulletLife);
    }
}
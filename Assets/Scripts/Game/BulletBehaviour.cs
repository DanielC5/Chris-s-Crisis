using UnityEngine;

/// <summary>
/// 极坐标空间运动
/// RadialIn 沿半径朝中心飞行，半径越小速度越慢（r->0 时 v->0）
/// RadialOut 沿半径远离中心飞行
/// TangentialCW / CCW 在固定半径上做圆周运动（半径越小线速度越慢）
/// Init() 指定模式、基础线速度、生命周期
/// </summary>
[RequireComponent(typeof(PolarTransform))]
public class BulletBehaviour : MonoBehaviour
{
    public enum Motion { RadialIn, RadialOut, TangentialCW, TangentialCCW }

    [Header("Movement")]
    public Motion mode = Motion.RadialIn;
    public float baseSpeed = 12f;
    [Range(1f, 4f)]
    public float speedExponent = 2f;
    public float lifeTime = 3f;

    [Header("Scale")]
    public float minScale = 0.3f;
    public float maxScale = 1.0f;

    [Header("Source")]
    public string source;

    private PolarTransform polar;
    private float initialRadius;
    private Vector3 startLocalScale;

    void Awake()
    {
        polar = GetComponent<PolarTransform>();
        initialRadius = Mathf.Max(1e-4f, polar.radius);


        startLocalScale = transform.localScale;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        switch (mode)
        {
            case Motion.RadialIn: RadialStep(-1); break;
            case Motion.RadialOut: RadialStep(+1); break;
            case Motion.TangentialCW:
            case Motion.TangentialCCW: TangentialStep(); break;
        }

        UpdateScale();
    }

    void RadialStep(int dir /* +1 / -1 */)
    {
        float speed = baseSpeed * Mathf.Pow(polar.radius / initialRadius, speedExponent);
        polar.radius = Mathf.Max(0f, polar.radius + dir * speed * Time.deltaTime);

        if (polar.radius <= 1e-3f) Destroy(gameObject);
    }

    void TangentialStep()
    {
        float angSpeedDeg = baseSpeed;
        float sign = (mode == Motion.TangentialCW) ? +1f : -1f;
        polar.angleDeg += sign * angSpeedDeg * Time.deltaTime;
    }

    void UpdateScale()
    {
        float scaleFactor = 1;
        if (mode == Motion.RadialIn)
        {
            float t = polar.radius / initialRadius;
            scaleFactor = Mathf.Lerp(minScale, maxScale, t);
        }
        else if (mode == Motion.RadialOut)
        {
            float t = initialRadius / polar.radius;
            scaleFactor = Mathf.Lerp(maxScale, minScale, t);
        }
        
        transform.localScale = startLocalScale * scaleFactor;
        //Debug.Log($"{t} + {scaleFactor} + {initialRadius}");
    }

    public void Init(Motion m, float v, float life, float exp = 1.3f)
    {
        mode = m; baseSpeed = v; lifeTime = life; speedExponent = exp;

        initialRadius = Mathf.Max(1e-4f, polar.radius);
        startLocalScale = transform.localScale;

        CancelInvoke();
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (((collision.CompareTag("Enemy") && source == "player") || (collision.CompareTag("Player") && source == "enemy")) && collision.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.OnHit();
            if (collision.CompareTag("Enemy"))
            {
                if (collision.gameObject.TryGetComponent<EnemyController>(out EnemyController enController))
                {
                    float minPoints = enController.minPointsGiven, maxPoints = enController.maxPointsGiven;
                    float points = Mathf.Lerp(maxPoints, minPoints, enController.timeInExistence / 6f);
                    GameManager.Instance.addScore((int)System.Math.Round(points / 10.0) * 10);
                }
                else if (collision.gameObject.TryGetComponent<MinionController>(out MinionController minController))
                {
                    GameManager.Instance.addScore(minController.pointsGiven);
                }

                if (Random.value < 0.05)
                {
                    GameObject.Find("PickupManager").GetComponent<PickupManager>().SpawnPickupWave(Random.Range(0, 2));
                }
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Boss"))
        {
            collision.gameObject.GetComponent<BossController>().Damage();
            Destroy(gameObject);
        }
    }
}
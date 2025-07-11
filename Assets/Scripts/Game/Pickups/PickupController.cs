using System;
using UnityEngine;

/// <summary>
/// 瞬间生成 -> 淡入 -> 圆环内旋转运动 -> 飞出
/// </summary>
[RequireComponent(typeof(PolarTransform), typeof(SpriteRenderer))]
public class PickupController : MonoBehaviour
{
    [Header("Fade")]
    public float fadeInTime = 0.5f;

    [Header("In-Field Movement")]
    public float inFieldTime = 3f;
    public AnimationCurve inFieldAngleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float inFieldAngleSpanDeg = 120f;

    [Header("Fly-Out")]
    public float flyOutTime = 1f;
    public AnimationCurve flyOutRadiusCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float flyOutExtraRadius = 8f;

    [Header("Zoom")]
    public float minScale = 0.2f;
    public float maxScale = 0.8f;

    [Header("Effect")]
    public string effect;
    public int pointsGiven;

    enum Phase
    {
        FadeIn, InField, FlyOut, Done
    }
    Phase phase = Phase.FadeIn;

    PolarTransform polar;
    SpriteRenderer sr;

    float timer;
    float startAngleDeg;
    float ringRadius => 4; //InGameManager.RingRadius;

    void Awake()
    {
        polar = GetComponent<PolarTransform>();
        sr = GetComponent<SpriteRenderer>();

        startAngleDeg = UnityEngine.Random.Range(0f, 360f);

        polar.radius = ringRadius;

        polar.angleDeg = startAngleDeg;

        Color c = sr.color; c.a = 0f; sr.color = c;

        UpdateScale();
    }

    void Update()
    {
        switch (phase)
        {
            case Phase.FadeIn: FadeInStep(); break;
            case Phase.InField: InFieldStep(); break;
            case Phase.FlyOut: FlyOutStep(); break;
        }
    }

    void FadeInStep()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / fadeInTime);

        Color c = sr.color; c.a = t; sr.color = c;
        UpdateScale();

        if (t >= 1f)
        {
            phase = Phase.InField;
            timer = 0f;
        }
    }

    void InFieldStep()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / inFieldTime);

        polar.radius = ringRadius;
        float extraAngle = inFieldAngleCurve.Evaluate(t) * inFieldAngleSpanDeg;
        polar.angleDeg = (startAngleDeg + InGameManager.RingOffsetDeg + extraAngle) % 360f;

        UpdateScale();

        if (t >= 1f) { phase = Phase.FlyOut; timer = 0f; }
    }

    void FlyOutStep()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / flyOutTime);

        float targetR = ringRadius + flyOutExtraRadius;
        polar.radius = Mathf.Lerp(ringRadius, targetR, flyOutRadiusCurve.Evaluate(t));
        polar.angleDeg = (startAngleDeg + InGameManager.RingOffsetDeg + inFieldAngleSpanDeg) % 360f;

        UpdateScale();

        if (t >= 1f)
        {
            Destroy(gameObject);
            phase = Phase.Done;
        }
    }

    void UpdateScale()
    {
        float s = Mathf.Lerp(minScale, maxScale, polar.radius / (ringRadius + flyOutExtraRadius));
        transform.localScale = Vector3.one * s;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SoundManager.Instance.PickupSFX(); // pickup sfx
            if (effect == "speed")
            {
                GameObject Player = collision.gameObject;
                if (!Player.GetComponent<PlayerController>().spedUp)
                {
                    collision.gameObject.GetComponent<PlayerController>().rotateSpeed *= 1.5f;
                    Player.GetComponent<PlayerController>().spedUp = true;
                }
                else
                {
                    GameManager.Instance.addScore(pointsGiven);
                }
            }
            else if (effect == "health")
            {
                GameManager.Instance.GainLife();
                GameManager.Instance.addScore(pointsGiven);
            }

            Destroy(gameObject);
        }
    }
}
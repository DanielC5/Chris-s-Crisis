using System;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{


    public float angularSpeedDeg;

    [Header("Scale")]
    public float minScale = 0.3f;
    public float maxScale = 1.0f;

    private PolarTransform polar;
    private float thetaDeg;

    private float initialRadius;

    private Vector3 startLocalScale;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        polar = GetComponent<PolarTransform>();
        thetaDeg = polar.angleDeg;
        angularSpeedDeg = 90f;
        initialRadius = Mathf.Max(1e-4f, polar.radius);
        startLocalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        thetaDeg += angularSpeedDeg * Time.deltaTime;
        float thetaRad = thetaDeg * Mathf.Deg2Rad;
        //calculate r
        float radius = 1/(1.1f + Mathf.Sin(thetaRad));  //polar function

        polar.angleDeg = thetaDeg;
        polar.radius = radius;
        UpdateScale();
    }
    
    void UpdateScale()
    {
        float t = Mathf.Abs(polar.radius / initialRadius);
        float scaleFactor = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = startLocalScale * scaleFactor;
        Debug.Log($"{t} + {scaleFactor} + {initialRadius}");
    }
}

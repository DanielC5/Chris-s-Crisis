using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public class PolarTransform : MonoBehaviour
{
    [Header("Polar Coordinate")]
    public float radius = 1f;

    [Tooltip("Unit: Degree (NOT RAD!!!)")]
    public float angleDeg = 0f;
    public float height = 0f;

    [Header("Options")]
    public Vector3 center = Vector3.zero;
    public bool useXZ = false;

    /// <summary>
    /// Per frame update coord
    /// </summary>
    void Update()
    {
        UpdateTransform();
    }

    /// <summary>
    /// Transformation
    /// </summary>
    void UpdateTransform()
    {
        float angleRad = angleDeg * Mathf.Deg2Rad;
        Vector3 pos;

        if (useXZ)
        {
            pos = new Vector3(
                center.x + radius * Mathf.Cos(angleRad),
                height,
                center.z + radius * Mathf.Sin(angleRad)
            );
        }
        else
        {
            pos = new Vector3(
                center.x + radius * Mathf.Cos(angleRad),
                center.y + radius * Mathf.Sin(angleRad),
                height
            );
        }

        // TEMPORARY SOLUTION to circumvent the annoying infinity error
        if (pos.magnitude > 250000f)
        {
            Destroy(gameObject);
        }

        transform.position = pos;
    }
    
    public void SetCenter(Vector3 newCenter)
    {
        center = newCenter;
    }

    /// <summary>
    /// (Reverse)
    /// </summary>
    public void FromWorldPosition(Vector3 worldPos)
    {
        Vector3 diff = worldPos - center;

        if (useXZ)
        {
            radius = new Vector2(diff.x, diff.z).magnitude;
            angleDeg = Mathf.Atan2(diff.z, diff.x) * Mathf.Rad2Deg;
            height = diff.y;
        }
        else
        {
            radius = new Vector2(diff.x, diff.y).magnitude;
            angleDeg = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            height = diff.z;
        }
    }
}
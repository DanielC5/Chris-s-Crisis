using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public void OnHit()
    {
        Destroy(gameObject);
    }    
}

using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public void OnHit()
    {
        if (!GameManager.Instance.playerIsImmune)
        {
            GameManager.Instance.SetState(GameManager.GameState.Died);
        }
    }
}

using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [SerializeField, Min(1)] private int damage = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DamagePlayer(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        DamagePlayer(collision);
    }

    private void DamagePlayer(Collision2D collision)
    {
        PlayerHealth playerHealth =
            collision.collider.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }
}
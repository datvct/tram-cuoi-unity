using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField, Min(1)] private int damage = 1;

    private Rigidbody2D body;
    private CircleCollider2D bulletCollider;
    private bool hasHit;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector2 direction, Collider2D shooterCollider)
    {
        if (shooterCollider != null)
        {
            Physics2D.IgnoreCollision(bulletCollider, shooterCollider);
        }

        body.linearVelocity = direction.normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit || other.GetComponent<Projectile>() != null)
        {
            return;
        }

        hasHit = true;
        bulletCollider.enabled = false;

        Health targetHealth = other.GetComponentInParent<Health>();

        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
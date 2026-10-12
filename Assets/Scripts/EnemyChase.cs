using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            body.linearVelocity = Vector2.zero;
            return;
        }
        PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();

        if (playerHealth != null && playerHealth.IsDead)
        {
            body.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = target.position - transform.position;

        body.linearVelocity = direction.normalized * moveSpeed;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
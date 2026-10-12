using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerHealth : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxHealth = 5;
    [SerializeField, Min(0f)] private float invulnerabilityDuration = 1f;

    private int currentHealth;
    private float nextDamageTime;

    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0;

    private void Awake()
    {
        currentHealth = maxHealth;

        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (IsDead)
        {
            return;
        }

        if (Time.time < nextDamageTime)
        {
            // Đổi màu luân phiên trong thời gian miễn sát thương.
            bool showWhite = Mathf.FloorToInt(Time.time * 12f) % 2 == 0;
            spriteRenderer.color = showWhite ? Color.white : originalColor;
        }
        else
        {
            spriteRenderer.color = originalColor;
        }
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || amount <= 0 || Time.time < nextDamageTime)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - amount);
        nextDamageTime = Time.time + invulnerabilityDuration;
        if (!IsDead)
        {
            GameAudio.Instance?.PlayHit();
            GetComponent<HitFlash>()?.PlayFlash();
        }
        Debug.Log($"Player còn {currentHealth}/{maxHealth} máu.");

        if (IsDead)
        {
            Die();
        }
    }

    private void Die()
    {
        GetComponent<PlayerMovement>().enabled = false;

        foreach (PlayerWeapon weapon in GetComponentsInChildren<PlayerWeapon>())
        {
            weapon.enabled = false;
        }

        foreach (PlayerAim aim in GetComponentsInChildren<PlayerAim>())
        {
            aim.enabled = false;
        }

        body.linearVelocity = Vector2.zero;
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        spriteRenderer.color = Color.gray;

        Debug.Log("Bạn đã thua!");
    }
}
using UnityEngine;
using System;
public class Health : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxHealth = 2;

    private int currentHealth;
    public event Action<Health> Died;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || currentHealth <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - amount);

        Debug.Log($"{gameObject.name} còn {currentHealth} máu.");

        if (currentHealth == 0)
        {
            GameAudio.Instance?.PlayEnemyDeath();
            Died?.Invoke(this);
            Destroy(gameObject);
        }else
        {
            GameAudio.Instance?.PlayHit();
            GetComponent<HitFlash>()?.PlayFlash();
        }
    }
}
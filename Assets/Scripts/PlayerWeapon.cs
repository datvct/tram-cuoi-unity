using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireInterval = 0.25f;

    [SerializeField] private AudioSource shootAudioSource;
    [SerializeField] private AudioClip shootSound;
    [SerializeField, Range(0f, 1f)] private float shootVolume = 0.7f;

    private Collider2D shooterCollider;
    private float nextFireTime;

    private void Awake()
    {
        shooterCollider = GetComponentInParent<Collider2D>();
    }

    private void LateUpdate()
    {
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Mouse.current == null ||
            projectilePrefab == null ||
            firePoint == null)
        {
            return;
        }

        if (Mouse.current.leftButton.isPressed &&
            Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireInterval;
        }
    }

    private void Shoot()
    {
        Projectile bullet = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        bullet.Initialize(firePoint.right, shooterCollider);

        if (shootAudioSource != null && shootSound != null)
        {
            shootAudioSource.PlayOneShot(shootSound, shootVolume);
        }
    }
}
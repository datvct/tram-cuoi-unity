using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HitFlash : MonoBehaviour
{
    [SerializeField] private Color flashColor = Color.white;

    [SerializeField, Min(0.01f)]
    private float flashDuration = 0.12f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashRoutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void PlayFlash()
    {
        // Nếu đang lóe, bắt đầu lại thời gian từ lần trúng mới.
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        spriteRenderer.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        spriteRenderer.color = originalColor;
        flashRoutine = null;
    }

    private void OnDisable()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }
}
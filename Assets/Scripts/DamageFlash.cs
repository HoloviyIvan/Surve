using UnityEngine;

public sealed class DamageFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private Color flashColor = new(1f, 0.2f, 0.2f, 1f);
    [SerializeField, Min(0.01f)] private float duration = 0.12f;

    private Color originalColor;
    private float remainingTime;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<SpriteRenderer>();

        if (targetRenderer != null)
            originalColor = targetRenderer.color;
    }

    private void Update()
    {
        if (remainingTime <= 0f)
            return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
            RestoreColor();
    }

    public void Play()
    {
        if (targetRenderer == null)
            return;

        targetRenderer.color = flashColor;
        remainingTime = duration;
    }

    private void OnDisable()
    {
        remainingTime = 0f;
        RestoreColor();
    }

    private void RestoreColor()
    {
        if (targetRenderer != null)
            targetRenderer.color = originalColor;
    }

    private void OnValidate()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<SpriteRenderer>();
    }
}

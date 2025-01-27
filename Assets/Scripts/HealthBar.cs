using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] public Transform bar;
    [SerializeField] private float smoothSpeed = 5f; // Controls animation speed

    private Vector3 offset;
    private Camera mainCamera;
    private float maxHealth;
    private Transform target;
    private float currentDisplayedHealth;
    private SpriteRenderer spriteRenderer;
    private Coroutine smoothUpdateCoroutine;

    public void Setup(Transform target, float maxHealth)
    {
        this.maxHealth = maxHealth;
        this.target = target;
        currentDisplayedHealth = maxHealth;
        UpdateBar(maxHealth);
    }

    public void UpdateBar(float newValue)
    {
        // Ensure maxHealth is valid
        if (maxHealth <= 0)
        {
            Debug.LogWarning("Max health is zero or negative. Cannot update health bar.");
            return;
        }

        // Calculate target health percentage
        float targetHealthPercentage = Mathf.Clamp01(newValue / maxHealth);

        // Stop any existing animation
        if (smoothUpdateCoroutine != null)
        {
            StopCoroutine(smoothUpdateCoroutine);
        }

        // Start new smooth transition
        smoothUpdateCoroutine = StartCoroutine(SmoothBarUpdate(targetHealthPercentage));
    }

    private void Start()
    {
        mainCamera = Camera.main;
        offset = new Vector3(0, 2.1f, 0);
        spriteRenderer = bar.GetComponent<SpriteRenderer>();

        a_Champion champion = GetComponentInParent<a_Champion>();
        if (champion != null)
        {
            maxHealth = champion.Health.GetHealth();
            currentDisplayedHealth = maxHealth;
        }
        else
        {
            Debug.LogError("Champion component not found in parent objects.");
        }
    }

    private void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                        mainCamera.transform.rotation * Vector3.up);
    }

    private IEnumerator SmoothBarUpdate(float targetPercentage)
    {
        if (spriteRenderer == null || spriteRenderer.drawMode != SpriteDrawMode.Tiled)
        {
            Debug.LogWarning("Sprite Renderer is not in Tiled mode or is missing. Cannot update size.");
            yield break;
        }

        Vector2 currentSize = spriteRenderer.size;
        float currentPercentage = currentSize.x;

        while (!Mathf.Approximately(currentPercentage, targetPercentage))
        {
            // Smoothly interpolate between current and target percentage
            currentPercentage = Mathf.Lerp(currentPercentage, targetPercentage, Time.deltaTime * smoothSpeed);

            // Update sprite size
            Vector2 newSize = spriteRenderer.size;
            newSize.x = currentPercentage;
            spriteRenderer.size = newSize;

            // If we're very close to the target, snap to it
            if (Mathf.Abs(currentPercentage - targetPercentage) < 0.01f)
            {
                currentPercentage = targetPercentage;
                newSize.x = targetPercentage;
                spriteRenderer.size = newSize;
                break;
            }

            yield return null;
        }
    }
}

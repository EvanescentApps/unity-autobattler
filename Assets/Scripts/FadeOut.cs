using UnityEngine;
using System.Collections;

public class FadeOut : MonoBehaviour
{
    private Renderer objectRenderer;
    private Color originalColor;
    private float fadeDuration = 0.5f;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
            SetMaterialTransparent();
            StartCoroutine(FadeOutCoroutine());
        }
        else
        {
            Debug.LogError("Renderer component not found on the GameObject.");
        }
    }

    private void SetMaterialTransparent()
    {
        Material material = objectRenderer.material;
        material.SetFloat("_Mode", 3);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }

    private IEnumerator FadeOutCoroutine()
    {
        float startTime = Time.time;
        float endTime = startTime + fadeDuration;

        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / fadeDuration;
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(originalColor.a, 0, t);
            objectRenderer.material.color = newColor;
            yield return null;
        }

        // Ensure the object is completely faded out
        objectRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        Destroy(gameObject); // Optionally destroy the GameObject after fading out
    }
}
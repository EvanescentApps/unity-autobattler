using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] public Transform bar;
    public Vector3 offset; // Offset to position the health bar above the champion

    private float maxHealth;
    public Transform target;

    public void Setup(Transform target, float maxHealth)
    {
        UpdateBar(maxHealth);
        this.target = target;
    }

    public void UpdateBar(float newValue)
    {
        float newScale = newValue / 100;
        Vector3 scale = bar.transform.localScale;
        scale.x = newScale;
        bar.transform.localScale = scale;
    }

    private void Start()
    {
        a_Champion champion = GetComponentInParent<a_Champion>();
        if (champion != null)
        {
            maxHealth = champion.Health.GetHealth();
        }
        else
        {
            Debug.LogError("Champion component not found in parent objects.");
        }
    }

    private void Update()
    {
        if(target != null){
            transform.position = target.position + offset;
        }
    }
}

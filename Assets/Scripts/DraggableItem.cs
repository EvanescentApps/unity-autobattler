using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCamera;
    private Vector3 initialPosition;

    [SerializeField] private float hoverHeight = 1.5f;
    [SerializeField] private float dragHeight = 1.5f;
    [SerializeField] private float raycastRadius = 0.5f;

    private GameObject lastHoveredUnit;
    private Vector3 originalScale;
    private Color originalColor;
    private Material lastHoveredMaterial;
    [SerializeField] private Color hoverColor = new Color(1f, 0.8f, 0f, 1f);
    [SerializeField] private float hoverScaleMultiplier = 1.1f;

    [SerializeField] private GameObject haloEffectPrefab;

    [System.Serializable]
    public class ItemUpgrade
    {
        public string attributeName;  
        public int upgradeLevel;     
        public int price;
    }

    [SerializeField] private List<ItemUpgrade> itemAttributes = new List<ItemUpgrade>();
    private void Start()
    {
        mainCamera = Camera.main;
        initialPosition = transform.position;
    }

    private void OnMouseDown()
    {
        isDragging = true;
        Vector3 mousePos = GetMouseWorldPosition();
        mousePos.y = dragHeight;
        offset = transform.position - mousePos;
        offset.y = 0;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 targetPos = GetMouseWorldPosition();
            targetPos.y = dragHeight;
            targetPos += offset;
            transform.position = targetPos;

            Collider[] hits = Physics.OverlapSphere(transform.position, raycastRadius);
            GameObject currentHover = null;

            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    currentHover = hit.gameObject;
                    break;
                }
            }

            if (currentHover != lastHoveredUnit)
            {
                RemoveHoverEffect();
                if (currentHover != null) ApplyHoverEffect(currentHover);
                lastHoveredUnit = currentHover;
            }
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        RemoveHoverEffect();

        bool appliedToChampion = false;
        Collider[] hits = Physics.OverlapSphere(transform.position, raycastRadius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                a_Champion champion = hit.GetComponent<a_Champion>();
                if (champion != null)
                {
                    // Créer et ajouter l'item
                    a_Item newItem = new BasicItem(champion.Entity, itemAttributes);
                    int totalPrice = itemAttributes.Sum(item => item.price);
                    if (GameManager.Instance.CanAfford(totalPrice))
                    {
                        champion.Items.Add(newItem);
                        newItem.RunUpgrades(champion);
                        GameManager.Instance.SpendMoney(totalPrice);
                        AddHaloEffect(hit.gameObject);
                        appliedToChampion = true;

                        Debug.Log($"Item with {itemAttributes.Count} upgrades applied to {champion.Entity.name}");
                    }
                }
            }
        }

        transform.position = initialPosition;
        if (!appliedToChampion) Debug.Log("Item was not dropped on a champion.");
    }

    private Vector3 GetMouseWorldPosition()
    {
        Plane plane = new Plane(Vector3.up, new Vector3(0, dragHeight, 0));
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        return plane.Raycast(ray, out float enter) ? ray.GetPoint(enter) : transform.position;
    }

    private void UpgradeChampion(a_Champion champion)
    {
        foreach (var item in champion.Items) item.RunUpgrades(champion);
    }

    private void ApplyHoverEffect(GameObject unit)
    {
        originalScale = unit.transform.localScale;
        unit.transform.localScale = originalScale * hoverScaleMultiplier;

        Renderer rend = unit.GetComponent<Renderer>();
        if (rend != null)
        {
            lastHoveredMaterial = rend.material;
            originalColor = lastHoveredMaterial.color;
            rend.material.color = hoverColor;
        }
    }

    private void RemoveHoverEffect()
    {
        if (lastHoveredUnit == null) return;

        lastHoveredUnit.transform.localScale = originalScale;
        if (lastHoveredMaterial != null)
        {
            Renderer rend = lastHoveredUnit.GetComponent<Renderer>();
            if (rend != null) rend.material.color = originalColor;
        }
        lastHoveredUnit = null;
    }

    private void AddHaloEffect(GameObject target)
    {
        if (haloEffectPrefab == null) return;
        GameObject halo = Instantiate(haloEffectPrefab, target.transform);
        halo.transform.localPosition = Vector3.zero;
        Destroy(halo, 2f);
    }

    private void OnDestroy() => RemoveHoverEffect();
}
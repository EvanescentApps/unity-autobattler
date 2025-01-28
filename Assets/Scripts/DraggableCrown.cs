using UnityEngine;

public class DraggableCrown : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCamera;
    private a_Champion currentKing;
    [SerializeField] private float hoverHeight = 1.5f;
    [SerializeField] private float dragHeight = 1.5f; // Height at which crown moves while dragging

    private GameObject lastHoveredUnit;
    private Vector3 originalScale;
    private Color originalColor;
    private Material lastHoveredMaterial;
    [SerializeField] private Color hoverColor = new Color(1f, 0.8f, 0f, 1f);
    [SerializeField] private float hoverScaleMultiplier = 1.2f;
    [SerializeField] private float raycastHeight = 1f;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnMouseDown()
    {
        isDragging = true;

        if (currentKing != null)
        {
            currentKing.SetKingStatus(false);
            currentKing = null;
        }

        // Calculate offset but maintain constant height
        Vector3 mousePos = GetMouseWorldPosition();
        mousePos.y = dragHeight; // Force constant height
        offset = transform.position - mousePos;
        offset.y = 0; // Ignore vertical offset
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 targetPos = GetMouseWorldPosition();
            targetPos.y = dragHeight;
            targetPos += offset;

            transform.position = targetPos;

            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * raycastHeight, Vector3.down, out hit, raycastHeight * 2f))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    GameObject hoveredUnit = hit.collider.gameObject;

                    if (hoveredUnit != lastHoveredUnit)
                    {
                        RemoveHoverEffect();
                        ApplyHoverEffect(hoveredUnit);
                        lastHoveredUnit = hoveredUnit;
                    }
                }
                else
                {
                    RemoveHoverEffect();
                }
            }
            else
            {
                RemoveHoverEffect();
            }
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        RemoveHoverEffect();

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * raycastHeight, Vector3.down, out hit, raycastHeight * 2f))
        {
            if (hit.collider.CompareTag("Player"))
            {
                a_Champion champion = hit.collider.GetComponent<a_Champion>();
                if (champion != null)
                {
                    if (currentKing != null && currentKing != champion)
                    {
                        currentKing.SetKingStatus(false);
                    }

                    currentKing = champion;
                    champion.SetKingStatus(true);

                    Vector3 newPos = champion.transform.position + Vector3.up * hoverHeight;
                    transform.position = newPos;
                }
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Plane dragPlane = new Plane(Vector3.up, new Vector3(0, dragHeight, 0));
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        float enter;

        if (dragPlane.Raycast(ray, out enter))
        {
            return ray.GetPoint(enter);
        }

        return transform.position; // Fallback to current position if raycast fails
    }

    private void ApplyHoverEffect(GameObject unit)
    {
        if (unit == null) return;

        originalScale = unit.transform.localScale;
        unit.transform.localScale = originalScale * hoverScaleMultiplier;

        Renderer renderer = unit.GetComponent<Renderer>();
        if (renderer != null && renderer.sharedMaterial != null)
        {
            lastHoveredMaterial = renderer.sharedMaterial;
            originalColor = renderer.sharedMaterial.color;
            renderer.sharedMaterial.color = hoverColor;
        }
    }

    private void RemoveHoverEffect()
    {
        if (lastHoveredUnit != null)
        {
            lastHoveredUnit.transform.localScale = originalScale;

            Renderer renderer = lastHoveredUnit.GetComponent<Renderer>();
            if (renderer != null && lastHoveredMaterial != null)
            {
                renderer.sharedMaterial.color = originalColor;
            }

            lastHoveredUnit = null;
        }
    }

    private void Update()
    {
        if (isDragging)
        {
            transform.Rotate(Vector3.up, 90f * Time.deltaTime);
        }
    }

    private void OnDestroy()
    {
        RemoveHoverEffect();
    }
}

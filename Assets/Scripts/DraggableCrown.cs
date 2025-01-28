using UnityEngine;

public class DraggableCrown : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCamera;
    private a_Champion currentKing;
    [SerializeField] private float hoverHeight = 1.5f;
    [SerializeField] private float dragHeight = 1.5f;

    private GameObject lastHoveredUnit;
    private Vector3 originalScale;
    private Color originalColor;
    private Material lastHoveredMaterial;
    [SerializeField] private Color hoverColor = new Color(1f, 0.8f, 0f, 1f);
    [SerializeField] private float hoverScaleMultiplier = 1.2f;
    [SerializeField] private float raycastDistance = 2f; // Increased detection distance
    [SerializeField] private float raycastRadius = 0.5f; // Added radius for spherecast

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

            // Using SphereCast instead of Raycast for better detection
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, raycastRadius, Vector3.down, out hit, raycastDistance))
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
                // Fallback to OverlapSphere if SphereCast fails
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, raycastRadius);
                bool foundPlayer = false;
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Player"))
                    {
                        GameObject hoveredUnit = hitCollider.gameObject;
                        if (hoveredUnit != lastHoveredUnit)
                        {
                            RemoveHoverEffect();
                            ApplyHoverEffect(hoveredUnit);
                            lastHoveredUnit = hoveredUnit;
                        }
                        foundPlayer = true;
                        break;
                    }
                }
                if (!foundPlayer)
                {
                    RemoveHoverEffect();
                }
            }
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        RemoveHoverEffect();

        // Using OverlapSphere for more reliable detection when dropping the crown
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, raycastRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                a_Champion champion = hitCollider.GetComponent<a_Champion>();
                if (champion != null)
                {
                    if (currentKing != null && currentKing != champion)
                    {
                        currentKing.SetKingStatus(false);
                    }

                    currentKing = champion;
                    champion.SetKingStatus(true);
                    Debug.Log($"The champion {champion.Entity.name} becomes the king");
                    Vector3 newPos = champion.transform.position + Vector3.up * hoverHeight;
                    transform.position = newPos;
                    break;
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

        return transform.position;
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
        if (currentKing != null)
        {
            transform.position = currentKing.transform.position + Vector3.up * hoverHeight;
        }
    }

    private void OnDestroy()
    {
        RemoveHoverEffect();
    }
}

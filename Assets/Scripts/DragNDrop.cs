// using System.Collections;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;

// public class DragNDrop : MonoBehaviour
// {
//     [SerializeField] private PlayerData playerManager;

//     [SerializeField] private string championType;
//     private Vector3 offset;
//     private float yPosition; // Fixed Y position for the object
//     private float zCoordinate; // Z coordinate for proper depth calculation
//     private bool isDragging = false; // Track whether the object is being dragged
//     private Vector3 originalPosition; // Store the original position of the object
//     public Vector3 playgroundMin = new Vector3(-10,0,-10); // Minimum bounds of the playground
//     public Vector3 playgroundMax = new Vector3(10,0,10); // Maximum bounds of the playground
//     public GameObject objectPrefab; // Prefab of the object to instantiate
//     // private bool wasOutsidePlaygroundAtStart; // Track whether the object started outside the playground
//     private Quaternion targetRotation; // Target rotation for the object
//     private bool isRotating = false; // Track whether the object is rotating
//     public float rotationSpeed = 50.0f; // Speed of the rotation

//     public bool gameDirection = false;
//     private float rotationDuration = 0.2f; // Duration of the rotation
//     private float rotationTime = 0; // Time elapsed for the rotation
//     private Rigidbody rb;
//     private bool activated = false;

//     private float resetDuration = 0.5f; // Duration of the reset animation


//     private Renderer objectRenderer;
//     private Color originalColor;
//     private Color invalidColor = Color.red;

//     private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();

//     private void Start()
//     {
//         originalPosition = transform.position; // Store the original position

//         targetRotation = transform.rotation; // Initialize target rotation
//         gameDirection = false;
//         rb = GetComponent<Rigidbody>();
//         Renderer[] renderers = GetComponentsInChildren<Renderer>();
//         foreach (Renderer renderer in renderers)
//         {
//             originalColors[renderer] = renderer.material.color;
//         }
//         // wasOutsidePlaygroundAtStart = IsOutsidePlayground(originalPosition);
//     }

//     private void Update()
//     {
//         // Handle mouse click
//         if (Input.GetMouseButtonDown(0)) // Left mouse button
//         {

//             if (isDragging)
//             {
//                 isDragging = false;

//                 CheckDropZoneAlternate();

//             } else {
//                 // Raycast to check if the object is clicked
//                 Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//                 RaycastHit hit;
//                 if (Physics.Raycast(ray, out hit) && hit.transform == transform)
//                 {
//                     // Start dragging
//                     isDragging = true;
                   
//                     // Store the object's current Y position and increase it by 0.4
//                     yPosition = transform.position.y + 0.2f;

//                     // Get the Z coordinate for depth consistency
//                     zCoordinate = Camera.main.WorldToScreenPoint(transform.position).z;

//                     // Calculate the offset between the mouse and the object's position
//                     offset = transform.position - GetWorldPoint(Input.mousePosition);

//                     targetRotation = transform.rotation * Quaternion.Euler(0, 180, 0);
//                     isRotating = true;
//                     rotationTime = 0; // Reset the rotation time
//                 } else {
//                     isRotating = false;
//                 }
//             }
//         }

//         // Move the object if it's being dragged
//         if (isDragging)
//         {
//             Vector3 targetPosition = GetWorldPoint(Input.mousePosition) + offset;
//             transform.position = new Vector3(targetPosition.x, yPosition, targetPosition.z);
//             CheckHoverZone();

//         }

//         if (gameDirection)
//         {
//             transform.rotation = Quaternion.Euler(0, 180, 0);
//         }

//         if (isRotating && gameDirection == false)
//         {
//             rotationTime += Time.deltaTime;
//             float t = rotationTime / rotationDuration;
//             transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);
//             if (t >= 1) {
//                 transform.rotation = targetRotation;
//                 isRotating = false;
//                 gameDirection = true;
//             }
//         }
//     }

//      private void CheckHoverZone()
//     {
//         // Cast a ray downward from the object's position
//         Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
//         if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
//         {
//             string hitTag = hit.collider.tag;
//             Vector3 hitPosition = hit.point;

//             // Check if the hit position is in the lower half of the playground
//             if (hitPosition.z < 0 || hitTag == "Hole" || hitTag == "Obstacle")
//             {
//                 ShowInvalidDropZoneContinue();
//             }
//             else
//             {
//                 ResetColor();
//             }
//         }
//         else
//         {
//             ShowInvalidDropZoneContinue();
//         }
//     }


//     private Vector3 GetWorldPoint(Vector3 screenPoint)
//     {
//         screenPoint.z = zCoordinate; // Maintain the depth (Z coordinate)
//         return Camera.main.ScreenToWorldPoint(screenPoint);
//     }

//     private void PurchaseChampionAndActivate(string championType)
//     {
//         gameObject.tag = "Player";
//         Debug.Log("Purchased & Activated Champion : " + championType);
//         activated = true;
//         //playerManager.SpendMoney(100); // Deduct the cost of the champion

//         // TODO GameManager add entity to player entities
//         GameManager.Instance.RespawnEntityInStore(championType);
//         // TODO : ADD TO PLAYER ENTITIES
//     }

//     private void ShowInvalidDropZone()
//     {
//         Renderer[] renderers = GetComponentsInChildren<Renderer>();
//         if (renderers.Length > 0)
//         {
//             foreach (Renderer renderer in renderers)
//             {
//                 renderer.material.color = invalidColor;
//             }
//             Invoke("ResetColor", 0.5f); // Reset color after 0.5 seconds
//         }
//         else
//         {
//             Debug.LogError("Renderer component not found on the object.");
//         }
//     }

//     private void ShowInvalidDropZoneContinue()
//     {
//         Renderer[] renderers = GetComponentsInChildren<Renderer>();
//         if (renderers.Length > 0)
//         {
//             foreach (Renderer renderer in renderers)
//             {
//                 renderer.material.color = invalidColor;
//             }
//         }
//         else
//         {
//             Debug.LogError("Renderer component not found on the object.");
//         }
//     }

//     private void ResetColor()
//     {
//         Renderer[] renderers = GetComponentsInChildren<Renderer>();
//         if (renderers.Length > 0)
//         {
//             foreach (Renderer renderer in renderers)
//             {
//                 if (originalColors.TryGetValue(renderer, out Color originalColor))
//                 {
//                     renderer.material.color = originalColor;
//                 }
//             }
//         }
//     }


//     private void DestroyObject()
//     {
//         Debug.Log("Destroying the current object.");
//         Destroy(gameObject);
//     }


//     private void ResetPosition()
//     {
//         isDragging = false; // Ensure dragging is stopped
      
//         StartCoroutine(AnimateResetPosition());
//     }

//     private IEnumerator AnimateResetPosition()
//     {
//         float elapsedTime = 0f;
//         Vector3 startingPosition = transform.position;
//         // Quaternion startingRotation = transform.rotation;

//         while (elapsedTime < resetDuration)
//         {
//             transform.position = Vector3.Lerp(startingPosition, originalPosition, elapsedTime / resetDuration);
//             // transform.rotation = Quaternion.Lerp(startingRotation, originalRotation, elapsedTime / resetDuration);
//             elapsedTime += Time.deltaTime;
//             yield return null;
//         }

//         // transform.position = originalPosition;
//         // transform.rotation = originalRotation;
//         Debug.Log("Resetting the object to the original position: " + originalPosition);
//     }

//     private void CheckDropZoneAlternate()
//     {
//         // Cast a ray downward from the object's position
    
//         Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
//         //  if (Physics.Raycast(ray, out hit, 10f))
//         if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
//         {
//             string hitTag = hit.collider.tag;

//             Vector3 hitPosition = hit.point;
//             if (hitPosition.z < 0)
//             {
//                 Debug.Log("Dropped outside the lower half of the playground");
//                 // TODO: Maybe show visual cues to indicate the invalid drop zone
//                 ResetPosition();
//                 return;
//             }

//             switch (hitTag)
//             {
//                 case "Sand" or "ClassicTile":
//                     if (!activated)
//                     {
//                         PurchaseChampionAndActivate(championType);
//                     } // Else the champion has just moved
//                     originalPosition = transform.position;
//                     break;
//                 case "Store":
//                     ShowInvalidDropZone();
//                     ResetPosition();
//                     // TODO : UNPURCHASE
//                     break;
//                 case "Hole" or "Obstacle":
//                     Debug.Log("Dropped on an invalid zone, "+ hitTag);
//                     ShowInvalidDropZone();
//                     ResetPosition();
//                     break;
//                 default:
//                     // If dropped on an untagged or differently tagged surface
//                     Debug.Log("Dropped on an invalid zone, "+ hitTag);
//                     ShowInvalidDropZone();
//                     ResetPosition();
//                     break;
//             }
//         }
//         else
//         {
//             // If the ray didn't hit anything, return to original position
//             Debug.Log("Dropped outside valid zone");
//             ShowInvalidDropZone();
//             ResetPosition();
//             //ReturnToOriginalPosition();
//         }
//     }

// }

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DragNDrop : MonoBehaviour
{
    [SerializeField] private PlayerData playerManager;
    [SerializeField] private string championType;
    private Vector3 offset;
    private float yPosition; // Fixed Y position for the object
    private float zCoordinate; // Z coordinate for proper depth calculation
    private bool isDragging = false; // Track whether the object is being dragged
    private Vector3 originalPosition; // Store the original position of the object
    public Vector3 playgroundMin = new Vector3(-10, 0, -10); // Minimum bounds of the playground
    public Vector3 playgroundMax = new Vector3(10, 0, 10); // Maximum bounds of the playground
    public GameObject objectPrefab; // Prefab of the object to instantiate
    private Quaternion targetRotation; // Target rotation for the object
    private bool isRotating = false; // Track whether the object is rotating
    public float rotationSpeed = 50.0f; // Speed of the rotation
    public bool gameDirection = false;
    private float rotationDuration = 0.2f; // Duration of the rotation
    private float rotationTime = 0; // Time elapsed for the rotation
    private Rigidbody rb;
    private bool activated = false;
    private float resetDuration = 0.5f; // Duration of the reset animation
    private Renderer objectRenderer;
    private Color invalidColor = Color.red;
    private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();

    private void Start()
    {
        originalPosition = transform.position; // Store the original position
        targetRotation = transform.rotation; // Initialize target rotation
        rb = GetComponent<Rigidbody>();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            originalColors[renderer] = renderer.material.color;
        }
    }

    private void Update()
    {
        HandleMouseInput();
        if (isDragging)
        {
            DragObject();
            CheckHoverZone();
        }
        HandleRotation();
        CheckHover();

    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            if (isDragging)
            {
                isDragging = false;
                CheckDropZoneAlternate();
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
                {
                    StartDragging(hit);
                    OnHover();
                }
                else
                {
                    isRotating = false;
                }
            }
        }
    }

    private void CheckHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
        {
            OnHover();
        }
    }

    private void OnHover()
    {
        //Debug.Log(championType + " hovered");
    }

    private void StartDragging(RaycastHit hit)
    {
        isDragging = true;
        yPosition = transform.position.y + 0.2f;
        zCoordinate = Camera.main.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetWorldPoint(Input.mousePosition);
        targetRotation = transform.rotation * Quaternion.Euler(0, 180, 0);
        isRotating = true;
        rotationTime = 0; // Reset the rotation time
    }

    private void DragObject()
    {
        Vector3 targetPosition = GetWorldPoint(Input.mousePosition) + offset;
        transform.position = new Vector3(targetPosition.x, yPosition, targetPosition.z);
    }

    private void HandleRotation()
    {
        if (gameDirection)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        if (isRotating && !gameDirection)
        {
            rotationTime += Time.deltaTime;
            float t = rotationTime / rotationDuration;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);
            if (t >= 1)
            {
                transform.rotation = targetRotation;
                isRotating = false;
                gameDirection = true;
            }
        }
    }

    private void CheckHoverZone()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            string hitTag = hit.collider.tag;
            Vector3 hitPosition = hit.point;

            if (hitPosition.z < 0 || hitTag == "Hole" || hitTag == "Obstacle")
            {
                ShowInvalidDropZone();
            }
            else
            {
                ResetColor();
            }
        }
        else
        {
            ShowInvalidDropZone();
        }
    }

    private Vector3 GetWorldPoint(Vector3 screenPoint)
    {
        screenPoint.z = zCoordinate; // Maintain the depth (Z coordinate)
        return Camera.main.ScreenToWorldPoint(screenPoint);
    }

    private void PurchaseChampionAndActivate(string championType)
    {
        gameObject.tag = "Player";
        Debug.Log("Purchased & Activated Champion : " + championType);
        activated = true;
        //playerManager.SpendMoney(100); // Deduct the cost of the champion
        a_Champion champion = GetComponent<a_Champion>();
        GameManager.Instance.AddEntityToPlayerEntities(champion);
        // TODO GameManager add entity to player entities
        GameManager.Instance.RespawnEntityInStore(championType);
        // TODO : ADD TO PLAYER ENTITIES
    }

    private void ShowInvalidDropZone()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.material.color = invalidColor;
            }
        }
        else
        {
            Debug.LogError("Renderer component not found on the object.");
        }
    }

    private void ResetColor()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            foreach (Renderer renderer in renderers)
            {
                if (originalColors.TryGetValue(renderer, out Color originalColor))
                {
                    renderer.material.color = originalColor;
                }
            }
        }
    }

    private void DestroyObject()
    {
        Debug.Log("Destroying the current object.");
        Destroy(gameObject);
    }

    private void ResetPosition()
    {
        isDragging = false; // Ensure dragging is stopped
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; // Reset velocity
            rb.angularVelocity = Vector3.zero; // Reset angular velocity
        }
        StartCoroutine(AnimateResetPosition());
        ResetColor();
    }

    private IEnumerator AnimateResetPosition()
    {
        float elapsedTime = 0f;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < resetDuration)
        {
            transform.position = Vector3.Lerp(startingPosition, originalPosition, elapsedTime / resetDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
        Debug.Log("Resetting the object to the original position: " + originalPosition);
    }

    private void CheckDropZoneAlternate()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            string hitTag = hit.collider.tag;
            Vector3 hitPosition = hit.point;

            if (hitPosition.z < 0)
            {
                Debug.Log("Dropped outside the lower half of the playground");
                // TODO: Maybe show visual cues to indicate the invalid drop zone
                ResetPosition();
                return;
            }

            switch (hitTag)
            {
                case "Sand":
                case "ClassicTile":
                    if (!activated)
                    {
                        PurchaseChampionAndActivate(championType);
                    } // Else the champion has just moved
                    originalPosition = transform.position;
                    break;
                case "Store":
                    ResetPosition();
                    // TODO : UNPURCHASE
                    break;
                case "Hole":
                case "Obstacle":
                    Debug.Log("Dropped on an invalid zone, " + hitTag);
                    ResetPosition();
                    break;
                default:
                    Debug.Log("Dropped on an invalid zone, " + hitTag);
                    ResetPosition();
                    break;
            }
        }
        else
        {
            Debug.Log("Dropped outside valid zone");
            ResetPosition();
        }
    }
}
using UnityEditor;
using UnityEngine;

public class DragNDrop : MonoBehaviour
{
    [SerializeField] private GameObject championDescription;

    [SerializeField] private PlayerData playerManager;

    private Vector3 offset;
    private float yPosition; // Fixed Y position for the object
    private float zCoordinate; // Z coordinate for proper depth calculation
    private bool isDragging = false; // Track whether the object is being dragged
    private Vector3 originalPosition; // Store the original position of the object
    public Vector3 playgroundMin; // Minimum bounds of the playground
    public Vector3 playgroundMax; // Maximum bounds of the playground
    public GameObject objectPrefab; // Prefab of the object to instantiate
    private bool wasOutsidePlaygroundAtStart; // Track whether the object started outside the playground

    private Quaternion targetRotation; // Target rotation for the object
    private bool isRotating = false; // Track whether the object is rotating
    public float rotationSpeed = 50.0f; // Speed of the rotation

    public bool gameDirection = false;

    private bool objectDestroyed = false;
    private float rotationDuration = 0.2f; // Duration of the rotation
    private float rotationTime = 0; // Time elapsed for the rotation

    private void Start()
    {
        originalPosition = transform.position; // Store the original position
        targetRotation = transform.rotation; // Initialize target rotation
        gameDirection = false;

        wasOutsidePlaygroundAtStart = IsOutsidePlayground(originalPosition);
    }

    private void Update()
    {
        // Handle mouse click
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {

            if (isDragging)
            {
                // Drop the object
                isDragging = false;
                //championDescription.SetActive(false);
                CheckDropZone();

            } else {
                // Raycast to check if the object is clicked
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit) && hit.transform == transform)
                {
                    // Start dragging
                    isDragging = true;
                    //championDescription.SetActive(true);
                    // Store the object's current Y position and increase it by 0.4
                    yPosition = transform.position.y + 0.4f;

                    // Get the Z coordinate for depth consistency
                    zCoordinate = Camera.main.WorldToScreenPoint(transform.position).z;

                    // Calculate the offset between the mouse and the object's position
                    offset = transform.position - GetWorldPoint(Input.mousePosition);

                    targetRotation = transform.rotation * Quaternion.Euler(0, 180, 0);
                    isRotating = true;
                    rotationTime = 0; // Reset the rotation time
                } else {
                    isRotating = false;
                }
            }
        }

        // Move the object if it's being dragged
        if (isDragging)
        {
            Vector3 targetPosition = GetWorldPoint(Input.mousePosition) + offset;
            transform.position = new Vector3(targetPosition.x, yPosition, targetPosition.z);
        }

        if (gameDirection)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        if (isRotating && gameDirection == false)
        {
            rotationTime += Time.deltaTime;
            float t = rotationTime / rotationDuration;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);
            if (t >= 1) {
                transform.rotation = targetRotation;
                isRotating = false;
                gameDirection = true;
            }
        }
    }

    private bool IsOutsidePlayground(Vector3 position)
    {
        return position.x < playgroundMin.x || position.x > playgroundMax.x ||
               position.z < playgroundMin.z || position.z > playgroundMax.z;
    }

    private Vector3 GetWorldPoint(Vector3 screenPoint)
    {
        screenPoint.z = zCoordinate; // Maintain the depth (Z coordinate)
        return Camera.main.ScreenToWorldPoint(screenPoint);
    }


    private void ReinstanciateObject()
    {
       // Instantiate a new object at the original position
        Debug.Log($"Recreating object at original position: {originalPosition}");
        GameObject newObject = Instantiate(objectPrefab, originalPosition, Quaternion.identity);
        newObject.GetComponent<DragNDrop>().objectPrefab = objectPrefab; // Ensure the prefab is assigned to the new instance                }
    }

    private bool IsOverHoleTile()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            return hit.collider.CompareTag("Hole");
        }
        return false;
    }

    private bool IsOverObstacleTile()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            return hit.collider.CompareTag("Obstacle");
        }
        return false;
    }

    private void PurchaseChampionAndActivate()
    {
        Debug.Log("Purchase champion");
        gameObject.tag = "Player";
        // TODO : CREATE CHAMPION AND ADD TO PLAYER ENTITIES
    }

    private void CheckDropZone() {

                // Check if the object is outside the playground
                if (IsOutsidePlayground(transform.position))
                {
                    // Destroy the object
                    Debug.Log("dropped outside; destroying the current object.");
                    Destroy(gameObject);
                    objectDestroyed = true;
                }
                else
                {

                    if (IsOverHoleTile() || IsOverObstacleTile()){
                        Debug.Log("Obstacle or Hole, Destroying the current object.");
                        Destroy(gameObject);
                        objectDestroyed = true;
                    } else {
                        PurchaseChampionAndActivate();
                        // TODO: Know Which champion it is
                    }
                }

                var isObjectPlaced = wasOutsidePlaygroundAtStart;
                // Check if the object started outside the playground
                if (isObjectPlaced || objectDestroyed)
                {
                    ReinstanciateObject();
                    objectDestroyed = false;
                // else Object started inside the playground. No new object will be created.
                }

                // Update the flag to reflect the original position status
                wasOutsidePlaygroundAtStart = IsOutsidePlayground(transform.position);
    }

    // private void CheckDropZoneAlternate()
    // {
    //     // Cast a ray downward from the object's position
    //     Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
    //     RaycastHit hit;

    //     if (Physics.Raycast(ray, out hit, 10f))
    //     {
    //         string hitTag = hit.collider.tag;

    //         switch (hitTag)
    //         {
    //             case "Playground":
    //                 Debug.Log("Dropped on playground");
    //                 PurchaseChampion();
    //                 break;

    //             case "Respawn":
    //                 Debug.Log("Dropped on Respawn plane");
    //                 ReSpawnChampionInStore();
    //                 break;

    //             default:
    //                 // If dropped on an untagged or differently tagged surface
    //                 break;
    //         }
    //     }
    //     else
    //     {
    //         // If the ray didn't hit anything, return to original position
    //         Debug.Log("Dropped outside valid zone");
    //         ReturnToOriginalPosition();
    //     }
    // }

}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class DragNDrop : MonoBehaviour
{
    // [SerializeField] private PlayerData playerManager;
    [SerializeField] private ChampionsDatabaseSO championsDatabase;

    [SerializeField] private string championType;
    ChampionsDatabaseSO.ChampionData championData;

    private Vector3 offset;
    private float yPosition; // Fixed Y position for the object
    private float zCoordinate; // Z coordinate for proper depth calculation
    private bool isDragging = false; // Track whether the object is being dragged
    private Vector3 previousPos; // Store the original position of the object
    public GameObject objectPrefab; // Prefab of the object to instantiate
    private Quaternion targetRotation; // Target rotation for the object
    private bool isRotating = false; // Track whether the object is rotating
    private bool gameDirection = false;
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
        // Load the ChampionsDatabaseSO asset from the Resources folder
        championsDatabase = Resources.Load<ChampionsDatabaseSO>("Champions Database");
        championData = championsDatabase.allChampions.Find(c => c.entityStats.name == championType);
        if (championsDatabase == null)
        {
            Debug.LogError("ChampionsDatabaseSO not found in Resources folder!");
            return;
        }

        previousPos = transform.position; // Store the original position
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
        bool gameStarted = GameManager.Instance.gameStarted;
        if (!gameStarted) {
            HandleMouseInput();
            if (isDragging)
            {
                gameObject.GetComponent<NavMeshAgent>().enabled = false;
                DragObject();
                CheckHoverZone();
            }
            HandleRotation();
            CheckHover();
        }
        
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            if (isDragging) // dropping
            {
                isDragging = false;
                Vector3 targetPosition = GetWorldPoint(Input.mousePosition) + offset;
                transform.position = new Vector3(targetPosition.x, yPosition, targetPosition.z);
                gameObject.GetComponent<NavMeshAgent>().enabled = true;
                CheckDropZoneAlternate();
            }
            else // picking up
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
                {
                    StartDragging(hit);
                    AudioManager.Instance.PlaySelect();
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
          
            ShowChampionDescription();
        }
        else
        {
            CharacteristicsManager.Instance.UnitHovered = false;
            // CharacteristicsManager.Instance.HideCharacteristics();
        }
    }

    private void ShowChampionDescription()
    {
        ChampionsDatabaseSO.ChampionData championData = championsDatabase.allChampions.Find(c => c.entityStats.name == championType);
        if (championData != null)
        {
   

            string abilityDescription = ""; // TODO :GET FROM CHAMPION DATA
            string abilityName = "";// TODO :GET FROM CHAMPION DATA
            
            string name = championData.description;

            CharacteristicsManager.Instance.DisplayImage(championData.entityStats.name.ToLower());
            CharacteristicsManager.Instance.DisplayCharacteristics(championData.healthStats.maxHealth, championData.healthStats.armor, championData.attackStats.cooldown, championData.attackStats.range, championData.attackStats.damage, championData.movementStats.speed, abilityDescription, abilityName, championData.entityStats.price, championData.recap);
            CharacteristicsManager.Instance.DisplayName(championData.entityStats.name);
            // CharacteristicsManager.Instance.ChangeCharacterImage(championData.entityStats.image); // Assuming championData.entityStats.image is a Sprite
            //descriptionText.text = description;
       
        }
        CharacteristicsManager.Instance.ShowCharacteristics();
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

    private Vector3 GetWorldPoint(Vector3 screenPoint)
    {
        screenPoint.z = zCoordinate; // Maintain the depth (Z coordinate)
        return Camera.main.ScreenToWorldPoint(screenPoint);
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


    private void ResetPosition()
    {
        isDragging = false; // Ensure dragging is stopped
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; // Reset velocity
            rb.angularVelocity = Vector3.zero; // Reset angular velocity
        }
        ResetColor();
        StartCoroutine(AnimateResetPosition());
        transform.position = previousPos;
        Debug.Log("Current position: " + transform.position);
    }
    private IEnumerator AnimateResetPosition()
    {
        float elapsedTime = 0f;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < resetDuration)
        {
            float t = elapsedTime / resetDuration;
            transform.position = Vector3.Lerp(startingPosition, previousPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Snap to the exact position to eliminate any glitch
        transform.position = previousPos;
        Debug.Log("Resetting the object to the original position: " + previousPos);
    }



    private void PurchaseChampionAndActivate(string championType)
    {
        gameObject.tag = "Player";
        gameObject.AddComponent<AITarget>();
        gameObject.transform.SetParent(GameManager.Instance.team1Parent.transform);

        Debug.Log("Purchased & Activated Champion : " + championType);
        activated = true;
        a_Champion champion = GetComponent<a_Champion>();
        champion.currentUnitMode = GameManager.Instance.currentUnitMode;
        Debug.Log("Champion price: " + champion.Entity.Price);
        GameManager.Instance.SpendMoney(champion.Entity.Price); // Deduct the cost of the champion
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
                        if (GameManager.Instance.CanAfford(championData.entityStats.price))
                        {
                            PurchaseChampionAndActivate(championType);
                            AudioManager.Instance.PlaySelect();
                        }
                        else
                        {
                            Debug.Log("Not enough money.");
                            ResetPosition();
                        }
                        previousPos = transform.position;
                    } // Else the champion has just moved
                    previousPos = transform.position;
                    break;
                case "Store":
                    Debug.Log("Dropped on the store");
                    ResetPosition();
                    // TODO : UNPURCHASE
                    break;
                case "Hole":
                case "Obstacle":
                    Debug.Log("Dropped on an invalid zone, " + hitTag);
                    //ResetPosition();
                    ResetColor();
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
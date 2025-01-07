// using UnityEngine;

// public class DragNDrop : MonoBehaviour
// {
//     private Vector3 offset;
//     private float yPosition; // Fixed Y position for the object
//     private float zCoordinate; // Z coordinate for proper depth calculation

//     private void OnMouseDown()
//     {
//         // Store the object's current Y position and increase it by 10
//         yPosition = transform.position.y + 0.4f;

//         // Get the Z coordinate for depth consistency
//         zCoordinate = Camera.main.WorldToScreenPoint(transform.position).z;

//         // Calculate the offset between the mouse and the object's position
//         offset = transform.position - GetWorldPoint(Input.mousePosition);
//     }

//     private void OnMouseDrag()
//     {
//         // Get the target position based on mouse movement
//         Vector3 targetPosition = GetWorldPoint(Input.mousePosition) + offset;

//         // Restrict movement to the X-Z plane while keeping the Y position fixed
//         transform.position = new Vector3(targetPosition.x, yPosition, targetPosition.z);
//     }

//     private Vector3 GetWorldPoint(Vector3 screenPoint)
//     {
//         screenPoint.z = zCoordinate; // Maintain the depth (Z coordinate)
//         return Camera.main.ScreenToWorldPoint(screenPoint);
//     }
// }

using UnityEngine;

public class DragNDrop : MonoBehaviour
{
    private Vector3 offset;
    private float yPosition; // Fixed Y position for the object
    private float zCoordinate; // Z coordinate for proper depth calculation
    private bool isDragging = false; // Track whether the object is being dragged

    private void Update()
    {
        // Handle mouse click
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            if (isDragging)
            {
                // Drop the object
                isDragging = false;
            }
            else
            {
                // Raycast to check if the object is clicked
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit) && hit.transform == transform)
                {
                    // Start dragging
                    isDragging = true;

                    // Store the object's current Y position and increase it by 0.4
                    yPosition = transform.position.y + 0.4f;

                    // Get the Z coordinate for depth consistency
                    zCoordinate = Camera.main.WorldToScreenPoint(transform.position).z;

                    // Calculate the offset between the mouse and the object's position
                    offset = transform.position - GetWorldPoint(Input.mousePosition);
                }
            }
        }

        // Move the object if it's being dragged
        if (isDragging)
        {
            Vector3 targetPosition = GetWorldPoint(Input.mousePosition) + offset;
            transform.position = new Vector3(targetPosition.x, yPosition, targetPosition.z);
        }
    }

    private Vector3 GetWorldPoint(Vector3 screenPoint)
    {
        screenPoint.z = zCoordinate; // Maintain the depth (Z coordinate)
        return Camera.main.ScreenToWorldPoint(screenPoint);
    }
}

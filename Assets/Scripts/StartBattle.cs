using UnityEngine;

public class StartBattle : MonoBehaviour
{
    public Camera gameViewCamera; // Reference to the GameViewCamera
    // public Camera unitPlacementCamera;

    public void Start()
    {
    }


    public void OnStartButtonPressed()
    {
        Debug.Log("Button was pressed! Starting the battle...");


        GameManager.Instance.StartBattle();

        // SetActiveCamera(gameViewCamera);

    }

    public void OnResetButtonPressed()
    {
        Debug.Log("Button was pressed! Resetting the battle...");

        GameManager.Instance.ResetBattle();
    }

    // private void SetActiveCamera(Camera activeCamera)
    // {
    //     // Disable all other cameras
    //     Camera[] allCameras = Camera.allCameras;
    //     foreach (Camera cam in allCameras)
    //     {
    //         cam.gameObject.SetActive(false);
    //     }

    //     // Enable the active camera
    //     if (activeCamera != null)
    //     {
    //         activeCamera.gameObject.SetActive(true);
    //         Debug.Log("Active camera set to: " + activeCamera.name);

    //     }
    //     else
    //     {
    //         Debug.LogError("Active camera is null.");
    //     }
    // }
}

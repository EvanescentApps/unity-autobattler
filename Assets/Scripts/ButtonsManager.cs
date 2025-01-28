using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonsManager : MonoBehaviour
{
    public Camera gameViewCamera; // Reference to the GameViewCamera
    // public Camera unitPlacementCamera;
    public TMP_Dropdown modeDropdown;

    public void Start()
    {
        modeDropdown.onValueChanged.AddListener(OnAttackModeChanged);

    }

    private void OnAttackModeChanged(int index)
    {
        Debug.Log("Attack mode changed to: " + index);
        if (index == 0) GameManager.Instance.SetCurrentModeAttack();
        else if (index == 1) GameManager.Instance.SetCurrentModeDefense();
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


    public void OnActionButtonPressed()
    {

        GameManager.Instance.ActionButtonPressed();
        Debug.Log("Action button pressed!");

        // TODO : Implement action button logic
    }

    public void OnHomeButtonPressed()
    {
        Debug.Log("Home button pressed!");

        SceneManager.LoadScene("MainMenu");

        // TODO : Implement home button logic
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

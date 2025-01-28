using System.Collections;
using UnityEngine;
using TMPro; 

public class ToastManager : MonoBehaviour
{
    public GameObject toastPanel; 
    public TextMeshProUGUI toastText; 

    private void Start()
    {
       
        toastPanel.SetActive(false);
    }

    public void ShowToast(string message, float duration)
    {
        StartCoroutine(ShowToastCoroutine(message, duration));
    }

    private IEnumerator ShowToastCoroutine(string message, float duration)
    {
        toastPanel.SetActive(true);
        toastText.text = message;

        yield return new WaitForSeconds(duration);

        toastPanel.SetActive(false);
    }
}

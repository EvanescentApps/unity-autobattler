using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CharacteristicsManager : MonoBehaviour{
    public static CharacteristicsManager Instance { get; private set; }

    public GameObject characteristicsPanel;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI characteristicsText;
    public Image characterImage; 

    public bool isCharacteristicsPanelActive => characteristicsPanel.activeSelf;

    private void Awake(){
        if (Instance == null){
            Instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    public void DisplayName(string name){
        nameText.text = name;
    }

    public void ChangeCharacterImage(Sprite newImage)
    {
        if (characterImage != null)
        {
            characterImage.sprite = newImage;
        }
        else
        {
            Debug.LogError("characterImage is not assigned.");
        }
    }

    public void DisplayCharacteristics(float maxHealth, float armor, float cooldown, float range, float damage, float speed, string capacityDescription, string capacityName, int price){
        
        characteristicsText.text = $"<b>Vitesse:</b> {speed} uu/s\n" +
                                 $"<b>Vie:</b> {maxHealth} pdv\n" +
                                 $"<b>Armure:</b> {armor}\n" +
                                 $"<b>Dégâts:</b> {damage}\n" +
                                 $"<b>Cooldown:</b> {cooldown} s\n" +
                                 $"<b>Capacité “{name}”:</b>\n" +
                                 $"{capacityDescription}\n" +
                                 $"<b>Prix:</b> {price} pièces";
    }

    public void ShowCharacteristics(){
        if (!characteristicsPanel.activeSelf){
            Debug.Log("Characteristics panel is ACTIVATED.");
            characteristicsPanel.SetActive(true);
        }
    }
    // public void HideCharacteristics(){
    //     if (characteristicsPanel.activeSelf){
    //         Debug.Log("Characteristics panel is DEACTIVATED.");
    //         characteristicsPanel.SetActive(false);
    //     }
    // }
}
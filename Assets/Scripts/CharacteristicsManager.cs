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

    public bool UnitHovered = false;

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

    private void Update(){
        if (Input.GetMouseButtonDown(0) && !UnitHovered){ // If the player clicks on the screen and the mouse is not hovering a unit
            if (characteristicsPanel.activeSelf){
                Debug.Log("Characteristics panel is DEACTIVATED.");
                characteristicsPanel.SetActive(false);
            }
        }
    }

    public void DisplayImage(string imageName){
        Sprite image = Resources.Load<Sprite>($"Images/{imageName}");
        characterImage.sprite = image;
    }

    // Compétences:
    // Barbare: Charge + attaque amplifiée
    // Chevalier: Bouclier pendant X sec
    // Archer: Attaque à distance ?
    // Magicien: Attaque de zone avec explosion de particules
    // Robinhood: Attaque à distance amplifiée mais moins souvent

    // SAID Dupliquera la barre de vie : en violet, qui se recharge en fonction des dégâts infligés

    // BRILLE EN BLEU ET UN PEU PLUS GRAND PENDANT LA SKILL

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
            UnitHovered = true;
        }
    }
    // public void HideCharacteristics(){
    //     if (characteristicsPanel.activeSelf){
    //         Debug.Log("Characteristics panel is DEACTIVATED.");
    //         characteristicsPanel.SetActive(false);
    //     }
    // }
}
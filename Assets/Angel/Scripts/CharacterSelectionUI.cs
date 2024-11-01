using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionUI1 : MonoBehaviour
{
    public CharacterManager characterManager; // Asigna en el Inspector
    public Button buttonKnight;
    public Button buttonRogue;
    public Button buttonWizard;

    void Start()
    {
        buttonKnight.onClick.AddListener(() => SelectCharacter("Knight"));
        buttonRogue.onClick.AddListener(() => SelectCharacter("Rogue"));
        buttonWizard.onClick.AddListener(() => SelectCharacter("Wizard"));
    }

    void SelectCharacter(string characterName)
    {
        characterManager.SwitchCharacter(characterName, characterManager.transform.position);
        // Opcional: Desactivar la UI de selección después de elegir
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
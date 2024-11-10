using UnityEngine;

public class MinimapIconController : MonoBehaviour
{
    // Sprites de los iconos que se asignar�n desde el inspector
    public Sprite knightIcon;
    public Sprite rogueIcon;
    public Sprite wizardIcon;

    // Referencia al SpriteRenderer donde se cambiar� el icono
    public SpriteRenderer minimapIconSpriteRenderer;

    private int characterIndex;

    void Start()
    {
        // Obtener el �ndice del personaje de CharacterManager
        characterIndex = CharacterManager.Instance.GetCharacterIndex();

        // Configurar el icono en base al �ndice
        switch (characterIndex)
        {
            case 1:
                SetKnightIcon();
                break;
            case 2:
                SetRogueIcon();
                break;
            case 3:
                SetWizardIcon();
                break;
            default:
                Debug.LogWarning("�ndice de personaje no v�lido.");
                break;
        }
    }

    private void SetKnightIcon()
    {
        Debug.Log("Se ha configurado el icono del Knight en el minimapa.");
        minimapIconSpriteRenderer.sprite = knightIcon;
    }

    private void SetRogueIcon()
    {
        Debug.Log("Se ha configurado el icono del Rogue en el minimapa.");
        minimapIconSpriteRenderer.sprite = rogueIcon;
    }

    private void SetWizardIcon()
    {
        Debug.Log("Se ha configurado el icono del Wizard en el minimapa.");
        minimapIconSpriteRenderer.sprite = wizardIcon;
    }
}

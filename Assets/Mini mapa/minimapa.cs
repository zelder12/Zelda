using UnityEngine;

public class MinimapIconController : MonoBehaviour
{
    // Sprites de los iconos que se asignarán desde el inspector
    public Sprite knightIcon;
    public Sprite rogueIcon;
    public Sprite wizardIcon;

    // Referencia al SpriteRenderer donde se cambiará el icono
    public SpriteRenderer minimapIconSpriteRenderer;

    private int characterIndex;

    void Start()
    {
        // Obtener el índice del personaje de CharacterManager
        characterIndex = CharacterManager.Instance.GetCharacterIndex();

        // Configurar el icono en base al índice
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
                Debug.LogWarning("Índice de personaje no válido.");
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

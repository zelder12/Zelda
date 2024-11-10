using UnityEngine;
using Cinemachine;
using UnityEditor; // Solo si usas Cinemachine

public class CharacterManager : MonoBehaviour
{
    // Singleton Instance
    public static CharacterManager Instance { get; private set; }

    // Prefabs de los personajes
    [Header("Character Prefabs")]
    public GameObject knightPrefab;
    public GameObject roguePrefab;
    public GameObject wizardPrefab;

    public GameObject Padre;

    // Instancia actual del personaje
    private GameObject currentPlayerInstance;

    // Nombre del personaje seleccionado
    private string selectedCharacter;

    // Referencia a la cámara (Cinemachine o tu propia cámara de seguimiento)
    public CinemachineVirtualCamera virtualCamera; // Si usas Cinemachine

    // Índice del personaje seleccionado
    public int characterIndex;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        GameManager persistentData = FindObjectOfType<GameManager>();

        if (gameObject.transform.position != null)
        {
            Debug.Log("El personaje es " + persistentData.PlayerRol);
            SwitchCharacter(persistentData.PlayerRol, transform.position);
        }
    }

    void Start()
    {
        //if (PlayerPrefs.HasKey("SelectedCharacter"))
        //{
        //    selectedCharacter = PlayerPrefs.GetString("SelectedCharacter");
        //    Debug.Log("Personaje seleccionado: " + selectedCharacter);
        //}
        //else
        //{
        //    selectedCharacter = "null";
        //}

        //Debug.Log("Personaje cargado: " + selectedCharacter);

        //SwitchCharacter(selectedCharacter, this.transform.position);

    }

    private void Update()
    {
    }

    /// <summary>
    /// Cambia el personaje actual por el seleccionado y lo coloca en la posición indicada.
    /// </summary>
    /// <param name="characterName">Nombre del personaje a seleccionar ("Knight", "Rogue", "Wizard").</param>
    /// <param name="spawnPosition">Posición en la que el personaje aparecerá.</param>
    public void SwitchCharacter(string characterName, Vector3 spawnPosition)
    {

        // Guardar la selección del personaje
        PlayerPrefs.SetString("SelectedCharacter", characterName);
        PlayerPrefs.Save();

        Debug.Log("ANTES DE RECONOCER EL PERSONAJE ");
        // Instanciar el nuevo personaje basado en el nombre
        switch (characterName)
        {
            case "Knight":
                currentPlayerInstance = Instantiate(knightPrefab, spawnPosition, Quaternion.identity);
                currentPlayerInstance.transform.SetParent(transform, false);
                currentPlayerInstance.transform.localPosition = Vector3.zero;
                characterIndex = 1; // Asigna índice del Knight
                Debug.Log("Personaje reconocido: " + characterName);
                break;
            case "Rogue":
                currentPlayerInstance = Instantiate(roguePrefab, spawnPosition, Quaternion.identity);
                currentPlayerInstance.transform.SetParent(transform, false);
                currentPlayerInstance.transform.localPosition = Vector3.zero;
                characterIndex = 2; // Asigna índice del Rogue
                Debug.Log("Personaje reconocido: " + characterName);
                break;
            case "Wizzard":
                currentPlayerInstance = Instantiate(wizardPrefab, spawnPosition, Quaternion.identity);
                currentPlayerInstance.transform.SetParent(transform, false);
                currentPlayerInstance.transform.localPosition = Vector3.zero;
                characterIndex = 3; // Asigna índice del Wizard
                Debug.Log("Personaje reconocido: " + characterName);
                break;
            default:
                Debug.LogError("Personaje no reconocido: " + characterName);
                break;
        }

        // Actualizar la referencia de la cámara al nuevo personaje
        if (virtualCamera != null)
        {
            virtualCamera.Follow = currentPlayerInstance.transform;
        }

        // Opcional: Ajustar la posición inicial del personaje, ya lo hicimos con el parámetro spawnPosition
    }

    /// <summary>
    /// Obtiene el índice del personaje actualmente seleccionado.
    /// </summary>
    /// <returns>Índice del personaje seleccionado.</returns>
    public int GetCharacterIndex()
    {
        return characterIndex;
    }

    /// <summary>
    /// Obtiene el nombre del personaje actualmente seleccionado.
    /// </summary>
    /// <returns>Nombre del personaje seleccionado.</returns>
    public string GetSelectedCharacter()
    {
        return selectedCharacter;
    }
}

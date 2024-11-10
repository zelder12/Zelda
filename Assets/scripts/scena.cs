using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class Scena : MonoBehaviour
{
    public GameObject image; // Referencia a la imagen que deseas activar

    private bool isPlayerInside = false;
    private string inputSequence = ""; // Variable para almacenar la secuencia de teclas presionadas

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            if (image != null)
            {
                image.SetActive(true); // Activa la imagen cuando el jugador entra en el Collider
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            if (image != null)
            {
                image.SetActive(false); // Desactiva la imagen cuando el jugador sale del Collider
            }
        }
    }

    void Update()
    {
        // Verifica si el jugador presiona 'F' para cargar la escena "Zelda 3"
        if (isPlayerInside && Input.GetKeyDown(KeyCode.F))
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("Zelda 3", LoadSceneMode.Single);
        }

        // Captura las teclas presionadas y actualiza la secuencia
        foreach (char c in Input.inputString)
        {
            inputSequence += c;

            // Si la secuencia supera 4 caracteres, recorta los primeros caracteres
            if (inputSequence.Length > 4)
            {
                inputSequence = inputSequence.Substring(inputSequence.Length - 4);
            }

            // Si la secuencia es "waos", carga la escena "WAOS"
            if (inputSequence == "waos")
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.LoadScene("WAOS", LoadSceneMode.Single);
                inputSequence = ""; // Limpia la secuencia después de cargar la escena
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            CinemachineVirtualCamera vcam = FindObjectOfType<CinemachineVirtualCamera>();
            if (vcam != null)
            {
                vcam.Follow = player.transform;
            }

            ResetPlayerMovement(player);

            // Establece las coordenadas según la escena cargada
            if (scene.name == "Zelda 3")
            {
                player.transform.position = new Vector3(23.162f, -22.69f, 0f);
            }
            else if (scene.name == "WAOS")
            {
                player.transform.position = new Vector3(30.7f, 48.1f, -0.007218612f);
            }
        }
        else
        {
            Debug.LogError("No se encontró el jugador con la etiqueta 'Player'.");
        }

        GameObject characterSelectionCanvas = GameObject.Find("CharacterSelectionCanvas");
        if (characterSelectionCanvas != null)
        {
            characterSelectionCanvas.SetActive(false);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            DontDestroyOnLoad(gameObject);
        }

        GameObject ui = GameObject.Find("UI");
        if (ui != null)
        {
            DontDestroyOnLoad(ui);
        }
    }

    private void ResetPlayerMovement(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }
}

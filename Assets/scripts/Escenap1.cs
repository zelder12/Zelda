using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class P1 : MonoBehaviour
{
    [SerializeField] private GameObject image; // Referencia a la imagen que deseas activar o desactivar
    private bool isPlayerInside = false;

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
        if (isPlayerInside && Input.GetKeyDown(KeyCode.F))
        {
            SceneManager.LoadScene("Zelda 4", LoadSceneMode.Single);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Zelda 4")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(48.8f, 56.8f, -0.007f);

                // Configura la Cinemachine Virtual Camera para que siga al jugador
                CinemachineVirtualCamera vcam = FindObjectOfType<CinemachineVirtualCamera>();
                if (vcam != null)
                {
                    vcam.Follow = player.transform;
                }
            }

            GameObject characterSelectionCanvas = GameObject.Find("CharacterSelectionCanvas");
            if (characterSelectionCanvas != null)
            {
                characterSelectionCanvas.SetActive(false);
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            DontDestroyOnLoad(player);
        }

        GameObject ui = GameObject.Find("UI"); // Cambia "UI" por el nombre real de tu GameObject de UI si es diferente
        if (ui != null)
        {
            DontDestroyOnLoad(ui);
        }
    }
}

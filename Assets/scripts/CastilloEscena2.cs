using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CastilloEscena2 : MonoBehaviour
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
            SceneManager.LoadScene("Zelda 3", LoadSceneMode.Single);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Zelda 3")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(1.53f, 57.96f, -0.007f);

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
}

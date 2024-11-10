using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class scenaCatilloExt : MonoBehaviour
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
            // Cargar la escena y suscribirse al evento sceneLoaded
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("Zelda 1", LoadSceneMode.Single);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Zelda 1")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(21.81f, 2.44f, -0.031f);

                // Configura la Cinemachine Virtual Camera para que siga al jugador
                CinemachineVirtualCamera vcam = FindObjectOfType<CinemachineVirtualCamera>();
                if (vcam != null)
                {
                    vcam.Follow = player.transform;
                }

                // Restablecer el movimiento del jugador (sin modificar PlayerMovement)
                ResetPlayerMovement(player);
            }

            GameObject characterSelectionCanvas = GameObject.Find("CharacterSelectionCanvas");
            if (characterSelectionCanvas != null)
            {
                characterSelectionCanvas.SetActive(false);
            }

            // Desuscribirse del evento después de ajustar la posición
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void ResetPlayerMovement(GameObject player)
    {
        // Detener cualquier movimiento o velocidad de forma manual, sin tocar PlayerMovement
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // Detener cualquier movimiento del jugador
        }
    }
}

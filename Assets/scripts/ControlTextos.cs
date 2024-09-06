using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlTextos : MonoBehaviour
{
    [SerializeField, TextArea(3, 10)] private string[] arrayTextos;
    [SerializeField] private UIManager uIManager;
    [SerializeField] private PlayerMovement playerMovement;

    private int indice;
    private bool cercaDeZelda;

    private void Awake()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement no encontrado.");
        }
        if (uIManager == null)
        {
            Debug.LogError("UIManager no asignado.");
        }
    }

    private void OnMouseDown()
    {
        float distancia = Vector2.Distance(this.gameObject.transform.position, playerMovement.transform.position);
        if (distancia <= 7)
        {
            uIManager.ActivaDesactivarCajaTextos(true);
            ActivaCartel();
        }
    }

    private void ActivaCartel()
    {
        if (indice < arrayTextos.Length)
        {
            uIManager.MostrarTextos(arrayTextos[indice]);
            indice++;
        }
        else
        {
            uIManager.ActivaDesactivarCajaTextos(false); 
        }
    }

    private void MostrarTextos()
    {
        if (cercaDeZelda)
        {
            if (indice < arrayTextos.Length)
            {
                uIManager.ActivaDesactivarCajaTextos(true); 
                uIManager.MostrarTextos(arrayTextos[indice]);
                indice++;
            }
            else
            {
                uIManager.ActivaDesactivarCajaTextos(false); 
                indice = 0;
            }
        }
    }
}

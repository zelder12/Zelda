using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tendero : MonoBehaviour
{
    [SerializeField] private GameObject tienda;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            tienda.SetActive(true);
        }
    }

    // Desactiva la tienda cuando el jugador se aleja
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            tienda.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueOrco : MonoBehaviour
{
    public float daño = 10f; // Variable para ajustar el daño desde el Inspector

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement personaje = collision.gameObject.GetComponent<PlayerMovement>();
            if (personaje != null)
            {
                personaje.CausarHerida(daño); // Pasamos el daño como parámetro
            }
        }
    }
}

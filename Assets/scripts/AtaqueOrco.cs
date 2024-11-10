using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueOrco : MonoBehaviour
{
    public float da�o = 10f; // Variable para ajustar el da�o desde el Inspector

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement personaje = collision.gameObject.GetComponent<PlayerMovement>();
            if (personaje != null)
            {
                personaje.CausarHerida(da�o); // Pasamos el da�o como par�metro
            }
        }
    }
}

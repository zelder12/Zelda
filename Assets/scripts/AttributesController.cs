using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributesController : MonoBehaviour
{
    public int cantidad;
    public string tipo;
    public string sub_tipo;

    private PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setCantidad(int cantidad)
    {
        this.cantidad = cantidad;
    }

    public int getCantidad()
    {
        return this.cantidad;
    }

    public void accion()
    {
        if (tipo == "arma")
        {
            if (sub_tipo == "espada")
            {
                Debug.Log("Quitar 1 de vida");
            }
        }else if (tipo == "objeto_utilizable")
        {
            if (sub_tipo == "pocion_vida")
            {
                Debug.Log("Recuperar 1 de vida");
                if (playerMovement != null)
                {
                    playerMovement.Curar(1, false); // Recupera 1 punto de vida
                }
            }

            if (sub_tipo == "pocion_vida_grande")
            {
                Debug.Log("Recuperar 2 de vida");
                if (playerMovement != null)
                {
                    playerMovement.Curar(2, false); // Recupera 2 punto de vida
                }
            }

            if (sub_tipo == "pocion_velocidad")
            {
                Debug.Log("Corre Forrest");
                if (playerMovement != null)
                {
                    playerMovement.AumentarVelocidad(1); // Aumenta la velocidad
                }
            }
        }
    }
}

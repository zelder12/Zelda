using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moneda : MonoBehaviour
{
    public delegate void SumaMoneda(int moneda);
    public static SumaMoneda sumaMoneda;

    [SerializeField] private int cantidadMonedas;
    [SerializeField] private AudioClip sonidoRecoleccion;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (sumaMoneda != null)
            {
                SumarMoneda();
                Destroy(this.gameObject);
            }
        }
    }

    private void SumarMoneda()
    {
        sumaMoneda?.Invoke(cantidadMonedas);
        if (ControladorSonido.instance != null && sonidoRecoleccion != null)
        {
            ControladorSonido.instance.EjecutarSonido(sonidoRecoleccion);
        }
        else
        {
            Debug.LogWarning("ControladorSonido o AudioClip son null.");
        }
    }
}

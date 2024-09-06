using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [SerializeField] private UIManager uIManager; 
    [SerializeField] private string[] textosNPC; 

    private bool cercaDelJugador;
    private int indiceTextoActual = 0; 

    private void Awake()
    {
        if (uIManager == null)
        {
            Debug.LogError("UIManager no asignado.");
        }
    }

    private void Update()
    {
        if (cercaDelJugador && Input.GetKeyDown(KeyCode.E)) 
        {
            MostrarSiguienteTexto();
        }
    }

    private void MostrarSiguienteTexto()
    {
        if (indiceTextoActual < textosNPC.Length)
        {
            uIManager.ActivaDesactivarCajaTextos(true); 
            uIManager.MostrarTextos(textosNPC[indiceTextoActual]); 
            indiceTextoActual++;
        }
        else
        {
            uIManager.ActivaDesactivarCajaTextos(false); 
            indiceTextoActual = 0; 
        }
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            cercaDelJugador = true;
        }
    }

    private void OnTriggerExit2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            cercaDelJugador = false;
            uIManager.ActivaDesactivarCajaTextos(false);
            indiceTextoActual = 0; 
        }
    }
}

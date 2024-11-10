using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [SerializeField] private UIManager uIManager;
    [SerializeField] private string[] textosNPC;
    [SerializeField] private GameObject image; // Referencia a la imagen que deseas activar o desactivar

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
        if (cercaDelJugador && Input.GetKeyDown(KeyCode.F))
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
            if (image != null)
            {
                image.SetActive(true); // Activa la imagen cuando el jugador entra en el Collider
            }
        }
    }

    private void OnTriggerExit2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            cercaDelJugador = false;
            if (image != null)
            {
                image.SetActive(false); // Desactiva la imagen cuando el jugador sale del Collider
            }
            uIManager.ActivaDesactivarCajaTextos(false);
            indiceTextoActual = 0;
        }
    }
}

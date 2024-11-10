using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

public class UIManager : MonoBehaviour
{
    private int totalMonedas;
    private int precioObjeto;
    private int golpesRecibidos = 0;
    public int maxGolpes;
    private PlayerMovement player;

    [SerializeField] private TMP_Text textoMonedas;
    [SerializeField] private GameObject cajaTexto;
    [SerializeField] private TMP_Text textoDialogo;
    [SerializeField] private GameObject panelGameOver;
    [SerializeField] private Button botonReiniciar;
    [SerializeField] private Button botonMenuInicial;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip musicaCorazonBajo;
    [SerializeField] private AudioClip musicaGameOver;

    [SerializeField] private Image sangreScreen;
    [SerializeField] private float incrementoSangre = 0.5f;

    [SerializeField] private Inventario inventarioScript;

    private List<Objetos> inventario = new List<Objetos>();
    private const int LimiteTotalObjetos = 12;

    private void Start()
    {
        
        Moneda.sumaMoneda += SumarMoneda;

        botonReiniciar.onClick.AddListener(ReiniciarJuego);
        botonMenuInicial.onClick.AddListener(IrAlMenuInicial);

        panelGameOver.SetActive(false);
        sangreScreen.color = new Color(sangreScreen.color.r, sangreScreen.color.g, sangreScreen.color.b, 0);
        sangreScreen.gameObject.SetActive(false);
    }

    /*private void Update()
    {
       
    }*/

    public void SumarMoneda(int moneda)
    {
        totalMonedas += moneda;
        textoMonedas.text = totalMonedas.ToString();
    }

    private bool musicaReproducida = false; // Bandera para controlar si la música ya se ha reproducido

    public void RecibirDaño(int vidaPersonaje, int maxVida)
    {
        if (vidaPersonaje > 0)
        {
            sangreScreen.gameObject.SetActive(true);

            // Ajusta la opacidad según el porcentaje de vida restante (mayor opacidad a menor vida)
            float porcentajeVidaRestante = (float)vidaPersonaje / maxVida;
            float alpha = Mathf.Clamp01(1 - porcentajeVidaRestante); // Opacidad máxima cuando vida está cerca de 0
            sangreScreen.color = new Color(sangreScreen.color.r, sangreScreen.color.g, sangreScreen.color.b, alpha);

            // Definir el umbral de vida crítica
            int umbralVidaCritica = 20;

            // Activa la música de estado crítico solo cuando la vida esté en o por debajo de 20
            if (vidaPersonaje <= umbralVidaCritica && !musicaReproducida)
            {
                ReproducirMusicaCorazonBajo();
                musicaReproducida = true;
            }
            else if (vidaPersonaje > umbralVidaCritica && musicaReproducida)
            {
                DetenerMusicaCorazonBajo();
                musicaReproducida = false;
            }
        }
        else
        {
            // Si la vida es 0, desactiva el panel de sangre y detiene la música
            sangreScreen.gameObject.SetActive(false);
            DetenerMusicaCorazonBajo();
            musicaReproducida = false;
        }
    }




    public void QuitarSangre(int vidaPersonaje, int maxVida)
    {
        if (vidaPersonaje > 0)
        {
            // Calcula la opacidad en función de la vida restante
            float porcentajeVidaRestante = (float)vidaPersonaje / maxVida;
            float alpha = Mathf.Clamp01(1 - porcentajeVidaRestante); // Aumenta opacidad a menor vida
            sangreScreen.color = new Color(sangreScreen.color.r, sangreScreen.color.g, sangreScreen.color.b, alpha);

            // Verificar si la vida sigue en el umbral crítico
            int umbralVidaCritica = 20;
            if (vidaPersonaje <= umbralVidaCritica && !musicaReproducida)
            {
                ReproducirMusicaCorazonBajo();
                musicaReproducida = true;
            }
            else if (vidaPersonaje > umbralVidaCritica && musicaReproducida)
            {
                DetenerMusicaCorazonBajo();
                musicaReproducida = false;
            }
        }
        else
        {
            // Si la vida es 0, desactiva el panel de sangre y detiene la música
            sangreScreen.color = new Color(sangreScreen.color.r, sangreScreen.color.g, sangreScreen.color.b, 0);
            sangreScreen.gameObject.SetActive(false);
            DetenerMusicaCorazonBajo();
            musicaReproducida = false;
        }
    }


    public void IniciarGameOver()
    {
        StartCoroutine(ActivarGameOver());
    }

    private IEnumerator ActivarGameOver()
    {
        yield return new WaitForSeconds(2f);
        if (musicaGameOver != null && audioSource != null)
        {
            audioSource.clip = musicaGameOver;
            audioSource.loop = true;
            audioSource.Play();
        }
        panelGameOver.SetActive(true);
    }

    public void ReiniciarJuego()
    {
        // Detener la música de Game Over antes de reiniciar
        if (audioSource.isPlaying)
        {
            audioSource.Stop(); // Detiene cualquier música que esté sonando
        }

        // Desactivar el panel de Game Over y el panel de sangre
        panelGameOver.SetActive(false);
        sangreScreen.gameObject.SetActive(false); // Desactiva sangreScreen

        // Cargar la escena de Zelda1 en vez de la actual
        SceneManager.LoadScene("Zelda 1");

        StartCoroutine(ActualizarReferenciasTrasReinicio()); // Asegura las referencias tras el cambio de escena
    }

    private IEnumerator ActualizarReferenciasTrasReinicio()
    {
        yield return null; // Espera un frame para asegurar que la escena se haya cargado completamente

        // Buscar el PlayerMovement automáticamente
        player = FindObjectOfType<PlayerMovement>();
        if (player == null)
        {
            Debug.LogError("No se encontró PlayerMovement después del cambio de escena.");
        }

        // Configurar la cámara para seguir al PlayerMovement
        var camara = FindObjectOfType<CinemachineVirtualCamera>();
        if (camara != null && player != null)
        {
            camara.Follow = player.transform;
        }
        else
        {
            Debug.LogError("Cámara o PlayerMovement no encontrados para seguir después del cambio de escena.");
        }
    }



    public void IrAlMenuInicial()
    {
        SceneManager.LoadScene(0);
    }

    public void ActivaDesactivarCajaTextos(bool activado)
    {
        cajaTexto.SetActive(activado);
    }

    public void MostrarTextos(string texto)
    {
        textoDialogo.text = texto;
    }

    public void ReproducirMusicaCorazonBajo()
    {
        if (audioSource != null && musicaCorazonBajo != null)
        {
            audioSource.clip = musicaCorazonBajo;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void DetenerMusicaCorazonBajo()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    #region TIENDA

    public void PrecioObjeto(string objeto)
    {
        switch (objeto)
        {
            case "gato":
                precioObjeto = 1;
                break;
            case "fiera":
                precioObjeto = 5;
                break;
            case "conejo":
                precioObjeto = 10;
                break;
            case "escudoBurbuja":
                precioObjeto = 5;
                break;
            default:
                precioObjeto = 0;
                break;
        }
        Debug.Log("Precio del objeto " + objeto + ": " + precioObjeto);
    }

    public void AdquirirObjeto(string objeto)
    {
        PrecioObjeto(objeto);
        Debug.Log("Monedas disponibles: " + totalMonedas);
        Debug.Log("Precio del objeto: " + precioObjeto);

        if (precioObjeto <= totalMonedas)
        {
            Inventario inventarioScript = FindObjectOfType<Inventario>();

            if (inventarioScript != null && inventarioScript.CalcularTotalObjetos() < LimiteTotalObjetos)
            {
                totalMonedas -= precioObjeto;
                textoMonedas.text = totalMonedas.ToString();

                GameObject equipo = (GameObject)Resources.Load(objeto);
                if (equipo != null)
                {
                    GameObject objetoInstanciado = Instantiate(equipo, Vector3.zero, Quaternion.identity);
                    Objetos objetoScript = objetoInstanciado.GetComponent<Objetos>();

                    if (objetoScript != null)
                    {
                        inventarioScript.AddObjeto(objetoInstanciado);
                        Debug.Log("Objeto adquirido y agregado al inventario: " + objeto);
                    }
                    else
                    {
                        Debug.LogWarning("El objeto instanciado no tiene el componente Objetos.");
                    }
                }
                else
                {
                    Debug.LogWarning("El objeto no se pudo cargar desde Resources.");
                }
            }
            else
            {
                Debug.Log("No se puede adquirir el objeto. Límite total de objetos alcanzado.");
            }
        }
        else
        {
            Debug.Log("No se puede adquirir el objeto. Monedas insuficientes.");
        }
    }
    #endregion
}
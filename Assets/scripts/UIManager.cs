using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private int totalMonedas;
    private int precioObjeto;
    private int golpesRecibidos = 0;

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

    public void RecibirDaño(int golpes)
    {
        golpesRecibidos = golpes;
        if(!(golpesRecibidos == 0))
        {
            sangreScreen.gameObject.SetActive(true);

            if (golpesRecibidos == 1)
            {
                sangreScreen.color = new Color(sangreScreen.color.r, sangreScreen.color.g, sangreScreen.color.b, 0.25f);
                DetenerMusicaCorazonBajo();
            }
            else if (golpesRecibidos == 2)
            {
                sangreScreen.color = new Color(sangreScreen.color.r, sangreScreen.color.g, sangreScreen.color.b, 1f);
                ReproducirMusicaCorazonBajo();
            }
        }
        else
        {
            sangreScreen.gameObject.SetActive(false);
        }
        
    }

    public void QuitarSangre()
    {
        if (golpesRecibidos > 0)
        {
            golpesRecibidos--;

            if (golpesRecibidos > 0)
            {
                float alpha = golpesRecibidos == 1 ? 0.25f : 1f;
                sangreScreen.color = new Color(sangreScreen.color.r, sangreScreen.color.g, sangreScreen.color.b, alpha);
            }
            else
            {
                sangreScreen.color = new Color(sangreScreen.color.r, sangreScreen.color.g, sangreScreen.color.b, 0);
                sangreScreen.gameObject.SetActive(false);
                DetenerMusicaCorazonBajo();
            }
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
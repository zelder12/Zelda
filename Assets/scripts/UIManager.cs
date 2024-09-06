using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private int totalMonedas;
    private int precioObjeto;

    [SerializeField] private TMP_Text textoMonedas;
    [SerializeField] private List<GameObject> listaCorazones;
    [SerializeField] private Sprite corazonActivado;
    [SerializeField] private Sprite corazonDesactivado;
    [SerializeField] private GameObject cajaTexto;
    [SerializeField] private TMP_Text textoDialogo;
    [SerializeField] private GameObject panelEquipo;
    [SerializeField] private GameObject panelGameOver; 
    [SerializeField] private Button botonReiniciar; 
    [SerializeField] private Button botonMenuInicial; 
    [SerializeField] private ObjectManager objectManager;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip musicaCorazonBajo;
    [SerializeField] private AudioClip musicaGameOver;

    private List<string> inventario = new List<string>();
    private const int LimiteTotalObjetos = 3;

    private void Start()
    {
        Moneda.sumaMoneda += SumarMoneda;

        botonReiniciar.onClick.AddListener(ReiniciarJuego);
        botonMenuInicial.onClick.AddListener(IrAlMenuInicial);

        panelGameOver.SetActive(false); 
    }

    private void SumarMoneda(int moneda)
    {
        totalMonedas += moneda;
        textoMonedas.text = totalMonedas.ToString();
    }

    public void RestaCorazones(int indice)
    {
        if (indice >= 0 && indice < listaCorazones.Count)
        {
            Image imagenCorazon = listaCorazones[indice].GetComponent<Image>();
            imagenCorazon.sprite = corazonDesactivado;

            if (indice == 0 && listaCorazones.Count == 1)
            {
                ReproducirMusicaCorazonBajo();
                StartCoroutine(ActivarGameOver()); 
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

    public void ActualizaCorazones(int cantidad)
    {
        bool soloUnCorazon = cantidad == 1;

        for (int i = 0; i < listaCorazones.Count; i++)
        {
            Image imagenCorazon = listaCorazones[i].GetComponent<Image>();
            imagenCorazon.sprite = i < cantidad ? corazonActivado : corazonDesactivado;

            if (i == 0 && soloUnCorazon)
            {
                ReproducirMusicaCorazonBajo();
            }
        }
        if (!soloUnCorazon)
        {
            DetenerMusicaCorazonBajo();
        }
    }

    private void ReproducirMusicaCorazonBajo()
    {
        if (audioSource != null && musicaCorazonBajo != null)
        {
            audioSource.clip = musicaCorazonBajo;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void DetenerMusicaCorazonBajo()
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
            if (objectManager.CalcularTotalObjetos() < LimiteTotalObjetos)
            {
                totalMonedas -= precioObjeto;
                textoMonedas.text = totalMonedas.ToString();

                GameObject equipo = (GameObject)Resources.Load(objeto);
                if (equipo != null)
                {
                    GameObject objetoInstanciado = Instantiate(equipo, Vector3.zero, Quaternion.identity, panelEquipo.transform);
                    Objetos objetoScript = objetoInstanciado.GetComponent<Objetos>();
                    if (objetoScript != null)
                    {
                        inventario.Add(objeto);
                        objectManager.AddObjeto(objetoScript);
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

                Debug.Log("Objeto adquirido: " + objeto);
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

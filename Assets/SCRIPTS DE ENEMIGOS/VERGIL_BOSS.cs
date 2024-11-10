using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class Vergil_Boss : MonoBehaviour
{
    // Propiedades del Jefe
    public float vida;
    public float vidaMaxima;
    public float armadura;
    public float da�o = 10f;

    // Referencias del Slider de la vida
    public Slider slider;
    public Canvas canvas;

    private Transform PlayerMovement;
    private NavMeshAgent agente;
    private bool objetivoDetectado = false;
    private Animator anim;
    private bool atacando = false;
    public float rangoAtaque = 10f;
    public float tiempoEntreAtaques = 2f;
    private Collider2D colEspada;

    // Audios para los di�logos y para el da�o
    public AudioClip[] audiosDialogo;  // Audios para di�logos
    public AudioClip[] audiosDa�o;     // Audios espec�ficos de da�o
    private AudioSource audioSource;
    private float tiempoUltimoDialogo = 0f;
    private float tiempoEntreDialogos = 5f;

    // Movimiento aleatorio
    public float rangoMovimientoAleatorio = 15f;
    public float tiempoCambioDestino = 5f;
    private float tiempoSiguienteCambio;

    private void Awake()
    {
        PlayerMovement = FindObjectOfType<PlayerMovement>().transform;
        agente = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        colEspada = GetComponentInChildren<Collider2D>();
        vida = vidaMaxima;

        // Inicializar el Slider
        if (slider != null)
        {
            slider.maxValue = vidaMaxima;
            slider.value = vidaMaxima;
        }

        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
        }
    }

    private void Start()
    {
        agente.updateRotation = false;
        agente.updateUpAxis = false;
        agente.speed = 6f;

        tiempoSiguienteCambio = Time.time + tiempoCambioDestino;
    }

    private void Update()
    {
        if (PlayerMovement == null)
            return;

        if (vida <= 0)
        {
            Destroy(gameObject);
            return;
        }

        // Actualizamos la vida en el slider
        if (slider != null)
        {
            slider.value = vida;
        }

        // Movimiento del enemigo
        float distancia = Vector3.Distance(PlayerMovement.position, this.transform.position);
        objetivoDetectado = distancia < rangoAtaque;

        ComportamientoVergil();
        RotarEnemigo();

        if (Input.GetKeyDown(KeyCode.J))
            RecibirDa�o(10f, "normal");

        else if (Input.GetKeyDown(KeyCode.I))
            RecibirDa�o(10f, "penetrante");

        else if (Input.GetKeyDown(KeyCode.K))
            RecibirDa�o(10f, "magico");
    }

    private void ComportamientoVergil()
    {
        if (objetivoDetectado)
        {
            if (Time.time >= tiempoUltimoDialogo + tiempoEntreDialogos)
            {
                ReproducirDialogoAleatorio();
                tiempoUltimoDialogo = Time.time;
            }

            agente.SetDestination(PlayerMovement.position);
            anim.SetBool("Caminando", true);

            if (!atacando && Vector3.Distance(PlayerMovement.position, transform.position) < rangoAtaque / 2)
                StartCoroutine(Atacar());
        }
        else
        {
            anim.SetBool("Caminando", false);
        }

        if (!objetivoDetectado && Time.time >= tiempoSiguienteCambio)
        {
            MoverAleatoriamente();
            tiempoSiguienteCambio = Time.time + tiempoCambioDestino;
            anim.SetBool("Caminando", true);
        }
    }

    private void MoverAleatoriamente()
    {
        Vector3 posicionAleatoria = transform.position + Random.insideUnitSphere * rangoMovimientoAleatorio;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(posicionAleatoria, out hit, rangoMovimientoAleatorio, NavMesh.AllAreas))
        {
            agente.SetDestination(hit.position);
        }
    }

    void RotarEnemigo()
    {
        if (PlayerMovement != null)
        {
            if (this.transform.position.x > PlayerMovement.position.x)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);
        }
    }

    IEnumerator Atacar()
    {
        atacando = true;
        anim.SetTrigger("Ataca");

        yield return new WaitForSeconds(0.5f);

        if (objetivoDetectado && colEspada != null && colEspada.bounds.Intersects(PlayerMovement.GetComponent<Collider2D>().bounds))
        {
            PlayerMovement.GetComponent<PlayerMovement>().CausarHerida(da�o);
        }

        yield return new WaitForSeconds(tiempoEntreAtaques);
        atacando = false;
    }

    private void ReproducirDialogoAleatorio()
    {
        if (audiosDialogo.Length > 0 && !audioSource.isPlaying)
        {
            int indiceAleatorio = Random.Range(0, audiosDialogo.Length);
            audioSource.PlayOneShot(audiosDialogo[indiceAleatorio]);
        }
    }

    private void ReproducirAudioDa�o()
    {
        if (audiosDa�o.Length > 0 && !audioSource.isPlaying)
        {
            int indiceAleatorio = Random.Range(0, audiosDa�o.Length);
            audioSource.PlayOneShot(audiosDa�o[indiceAleatorio]);
        }
    }

    public void RecibirDa�o(float cantidad, string tipoDa�o)
    {
        switch (tipoDa�o)
        {
            case "normal":
                if (armadura > 0)
                {
                    armadura -= cantidad;
                    if (armadura < 0)
                        vida += armadura;
                }
                else
                {
                    vida -= cantidad;
                }
                break;

            case "penetrante":
                vida -= cantidad;
                break;

            case "magico":
                if (armadura > 0)
                {
                    armadura -= cantidad * 0.5f;
                    if (armadura < 0)
                        vida += armadura;
                }
                else
                {
                    vida -= cantidad;
                }
                StartCoroutine(AplicarVeneno(3f, 1f));
                break;
        }

        // Reproducir audio de da�o
        ReproducirAudioDa�o();
    }

    private IEnumerator AplicarVeneno(float duracion, float da�oPorSegundo)
    {
        float tiempoTotal = 0f;
        while (tiempoTotal < duracion)
        {
            vida -= da�oPorSegundo;
            tiempoTotal += 1f;
            yield return new WaitForSeconds(1f);
        }
    }
}

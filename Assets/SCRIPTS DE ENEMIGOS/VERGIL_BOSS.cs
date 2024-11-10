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
    public float daño = 10f;

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

    // Audios para los diálogos y para el daño
    public AudioClip[] audiosDialogo;  // Audios para diálogos
    public AudioClip[] audiosDaño;     // Audios específicos de daño
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
            RecibirDaño(10f, "normal");

        else if (Input.GetKeyDown(KeyCode.I))
            RecibirDaño(10f, "penetrante");

        else if (Input.GetKeyDown(KeyCode.K))
            RecibirDaño(10f, "magico");
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
            PlayerMovement.GetComponent<PlayerMovement>().CausarHerida(daño);
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

    private void ReproducirAudioDaño()
    {
        if (audiosDaño.Length > 0 && !audioSource.isPlaying)
        {
            int indiceAleatorio = Random.Range(0, audiosDaño.Length);
            audioSource.PlayOneShot(audiosDaño[indiceAleatorio]);
        }
    }

    public void RecibirDaño(float cantidad, string tipoDaño)
    {
        switch (tipoDaño)
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

        // Reproducir audio de daño
        ReproducirAudioDaño();
    }

    private IEnumerator AplicarVeneno(float duracion, float dañoPorSegundo)
    {
        float tiempoTotal = 0f;
        while (tiempoTotal < duracion)
        {
            vida -= dañoPorSegundo;
            tiempoTotal += 1f;
            yield return new WaitForSeconds(1f);
        }
    }
}

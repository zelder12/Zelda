using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GANON_BOSS : MonoBehaviour
{
    public float vida;
    public float vidaMaxima = 200f;
    public float armadura;
    public float daño = 10f;

    public Slider slider;
    public Canvas canvas;
    private Transform playerTransform;
    private NavMeshAgent agente;
    private bool objetivoDetectado = false;
    private Animator anim;
    private bool atacando = false;
    private bool estaMuerto = false; // Bandera para saber si está muerto
    public float rangoAtaque = 10f;
    public float tiempoEntreAtaques = 2f;
    private Collider2D colEspada;

    public AudioClip[] audiosGolpe;
    public AudioClip audioMuerte; // Sonido específico para la muerte
    public AudioClip audioAparicion; // Sonido específico para la aparición
    private AudioSource audioSource;

    public float rangoMovimientoAleatorio = 15f;
    public float tiempoCambioDestino = 5f;
    private float tiempoSiguienteCambio;

    private void Awake()
    {
        playerTransform = FindObjectOfType<PlayerMovement>().transform;
        agente = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        colEspada = GetComponentInChildren<Collider2D>();

        // Inicializa la barra de vida en cero
        if (slider != null)
        {
            slider.maxValue = vidaMaxima;
            slider.value = 0;
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

        // Ejecuta la animación de aparición y el sonido
        Aparecer();

        // Inicia la corrutina para llenar la vida
        StartCoroutine(LlenarVidaInicial());
    }

    private void Aparecer()
    {
        anim.SetTrigger("Aparicion"); // Activa la animación de aparición

        if (audioSource != null && audioAparicion != null)
        {
            audioSource.PlayOneShot(audioAparicion); // Reproduce el sonido de aparición
        }
    }

    private IEnumerator LlenarVidaInicial()
    {
        vida = 0; // Comienza la vida en cero

        float duracion = 2f; // Duración de la animación de llenado en segundos
        float tiempoActual = 0f;

        while (tiempoActual < duracion)
        {
            tiempoActual += Time.deltaTime;
            vida = Mathf.Lerp(0, vidaMaxima, tiempoActual / duracion);

            // Actualiza el slider
            if (slider != null)
            {
                slider.value = vida;
            }

            yield return null; // Espera hasta el siguiente frame
        }

        // Asegúrate de que la vida esté en el valor máximo al terminar
        vida = vidaMaxima;
        if (slider != null)
        {
            slider.value = vidaMaxima;
        }
    }

    private void Update()
    {
        if (playerTransform == null || estaMuerto) return;

        if (vida <= 0 && !estaMuerto)
        {
            Morir(); // Llama a la función de muerte
            return;
        }

        if (slider != null)
        {
            slider.value = vida;
        }

        float distancia = Vector3.Distance(playerTransform.position, transform.position);
        objetivoDetectado = distancia < rangoAtaque;

        ComportamientoGanondorf();
        RotarEnemigo();

        if (Input.GetKeyDown(KeyCode.J)) RecibirDaño(10f, "normal");
        else if (Input.GetKeyDown(KeyCode.I)) RecibirDaño(10f, "penetrante");
        else if (Input.GetKeyDown(KeyCode.K)) RecibirDaño(10f, "magico");
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Verifica si el jugador está dentro del BoxCollider2D
        if (other.CompareTag("Player") && !atacando && !estaMuerto)
        {
            StartCoroutine(Atacar());
        }
    }

    private void ComportamientoGanondorf()
    {
        if (objetivoDetectado)
        {
            agente.SetDestination(playerTransform.position);
            anim.SetBool("Caminando", true);
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

    private void RotarEnemigo()
    {
        if (playerTransform != null)
        {
            transform.localScale = new Vector3(
                transform.position.x > playerTransform.position.x ? -1 : 1,
                1,
                1
            );
        }
    }

    private IEnumerator Atacar()
    {
        atacando = true;
        anim.SetTrigger("Ataca");

        ReproducirSonidoDeAtaque();

        yield return new WaitForSeconds(0.5f);

        if (objetivoDetectado && colEspada != null && colEspada.bounds.Intersects(playerTransform.GetComponent<Collider2D>().bounds))
        {
            playerTransform.GetComponent<PlayerMovement>().CausarHerida(daño);
        }

        yield return new WaitForSeconds(tiempoEntreAtaques);
        atacando = false;
    }

    private void ReproducirSonidoDeAtaque()
    {
        if (audioSource != null && audiosGolpe.Length > 0)
        {
            int indiceAleatorio = Random.Range(0, audiosGolpe.Length);
            audioSource.PlayOneShot(audiosGolpe[indiceAleatorio]);
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
                    if (armadura < 0) vida += armadura;
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
                    if (armadura < 0) vida += armadura;
                }
                else
                {
                    vida -= cantidad;
                }
                StartCoroutine(AplicarVeneno(3f, 1f));
                break;
        }
    }

    private void Morir()
    {
        if (estaMuerto) return; // Asegúrate de que solo se ejecute una vez

        estaMuerto = true;
        anim.SetTrigger("Muerte"); // Activa la animación de muerte
        anim.ResetTrigger("Ataca"); // Detén cualquier animación de ataque
        anim.SetBool("Caminando", false); // Detén la animación de caminar
        agente.isStopped = true; // Detén el movimiento

        if (canvas != null) Destroy(canvas.gameObject); // Opcional: elimina el canvas de vida

        // Reproduce el sonido de muerte si existe
        if (audioSource != null && audioMuerte != null)
        {
            audioSource.PlayOneShot(audioMuerte);
        }

        // Espera hasta que la animación de muerte se complete antes de destruir el objeto
        StartCoroutine(DestruirDespuesDeMuerte());
    }

    private IEnumerator DestruirDespuesDeMuerte()
    {
        // Espera a que la animación de muerte en el primer layer se complete
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        Destroy(gameObject); // Finalmente destruye el objeto
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

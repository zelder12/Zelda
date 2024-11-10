using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GANON_BOSS : MonoBehaviour
{
    public float vida;
    public float vidaMaxima = 200f;
    public float armadura;
    public float da�o = 10f;

    public Slider slider;
    public Canvas canvas;
    private Transform playerTransform;
    private NavMeshAgent agente;
    private bool objetivoDetectado = false;
    private Animator anim;
    private bool atacando = false;
    private bool estaMuerto = false; // Bandera para saber si est� muerto
    public float rangoAtaque = 10f;
    public float tiempoEntreAtaques = 2f;
    private Collider2D colEspada;

    public AudioClip[] audiosGolpe;
    public AudioClip audioMuerte; // Sonido espec�fico para la muerte
    public AudioClip audioAparicion; // Sonido espec�fico para la aparici�n
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

        // Ejecuta la animaci�n de aparici�n y el sonido
        Aparecer();

        // Inicia la corrutina para llenar la vida
        StartCoroutine(LlenarVidaInicial());
    }

    private void Aparecer()
    {
        anim.SetTrigger("Aparicion"); // Activa la animaci�n de aparici�n

        if (audioSource != null && audioAparicion != null)
        {
            audioSource.PlayOneShot(audioAparicion); // Reproduce el sonido de aparici�n
        }
    }

    private IEnumerator LlenarVidaInicial()
    {
        vida = 0; // Comienza la vida en cero

        float duracion = 2f; // Duraci�n de la animaci�n de llenado en segundos
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

        // Aseg�rate de que la vida est� en el valor m�ximo al terminar
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
            Morir(); // Llama a la funci�n de muerte
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

        if (Input.GetKeyDown(KeyCode.J)) RecibirDa�o(10f, "normal");
        else if (Input.GetKeyDown(KeyCode.I)) RecibirDa�o(10f, "penetrante");
        else if (Input.GetKeyDown(KeyCode.K)) RecibirDa�o(10f, "magico");
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Verifica si el jugador est� dentro del BoxCollider2D
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
            playerTransform.GetComponent<PlayerMovement>().CausarHerida(da�o);
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

    public void RecibirDa�o(float cantidad, string tipoDa�o)
    {
        switch (tipoDa�o)
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
        if (estaMuerto) return; // Aseg�rate de que solo se ejecute una vez

        estaMuerto = true;
        anim.SetTrigger("Muerte"); // Activa la animaci�n de muerte
        anim.ResetTrigger("Ataca"); // Det�n cualquier animaci�n de ataque
        anim.SetBool("Caminando", false); // Det�n la animaci�n de caminar
        agente.isStopped = true; // Det�n el movimiento

        if (canvas != null) Destroy(canvas.gameObject); // Opcional: elimina el canvas de vida

        // Reproduce el sonido de muerte si existe
        if (audioSource != null && audioMuerte != null)
        {
            audioSource.PlayOneShot(audioMuerte);
        }

        // Espera hasta que la animaci�n de muerte se complete antes de destruir el objeto
        StartCoroutine(DestruirDespuesDeMuerte());
    }

    private IEnumerator DestruirDespuesDeMuerte()
    {
        // Espera a que la animaci�n de muerte en el primer layer se complete
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        Destroy(gameObject); // Finalmente destruye el objeto
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

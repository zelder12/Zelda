using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    // Propiedades del Enemigo
    public float vida;
    public float armadura;
    public float vidaMaxima;
    public float daño = 10f;

    private Transform PlayerMovement;
    private NavMeshAgent agente;
    private bool objetivoDetectado = false;
    private Animator anim;
    private bool atacando = false;
    public float rangoAtaque = 10f;
    public float tiempoEntreAtaques = 3f;
    private Collider2D colEspada;

    // Variables para el movimiento aleatorio
    public float rangoMovimientoAleatorio = 15f;
    public float tiempoCambioDestino = 5f;
    private float tiempoSiguienteCambio;

    // Nueva variable para controlar si el enemigo es ReDead
    public bool esReDead = false;

    // Para el sonido de grito de ReDead
    public AudioClip gritoReDead;
    private AudioSource audioSource;

    // Para controlar el estado del jugador (si está congelado)
    private bool jugadorCongelado = false;
    public float duracionCongelado = 2f;
    private float tiempoDesdeUltimoGrito = 0f;
    public float tiempoEntreGritos = 10f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // Intentar encontrar al jugador en la escena si la referencia no existe
        if (FindObjectOfType<PlayerMovement>() != null)
        {
            PlayerMovement = FindObjectOfType<PlayerMovement>().transform;
        }
        
        agente = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        colEspada = GetComponentInChildren<Collider2D>();
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        vida = vidaMaxima;
    }

    private void Start()
    {
        agente.updateRotation = false;
        agente.updateUpAxis = false;

        tiempoSiguienteCambio = Time.time + tiempoCambioDestino;
    }

    private void Update()
    {
        // Verificar si PlayerMovement está asignado
        if (PlayerMovement == null) return;

        if (vida <= 0)
        {
            StopAllCoroutines();
            PlayerMovement.GetComponent<PlayerMovement>().isFrozen = false;
            Destroy(gameObject);
            return;
        }

        float distancia = Vector3.Distance(PlayerMovement.position, this.transform.position);
        objetivoDetectado = distancia < rangoAtaque;

        if (esReDead)
        {
            ComportamientoReDead();
        }
        else
        {
            ComportamientoMoblin();
        }

        RotarEnemigo();

        if (Input.GetKeyDown(KeyCode.J)) AplicarDañoNormal(10f);
        else if (Input.GetKeyDown(KeyCode.I)) AplicarDañoPenetrante(8f);
        else if (Input.GetKeyDown(KeyCode.K)) AplicarDañoMagico(5f);
    }

    private void ComportamientoReDead()
    {
        if (objetivoDetectado && !jugadorCongelado)
        {
            if (tiempoDesdeUltimoGrito >= tiempoEntreGritos)
            {
                if (!audioSource.isPlaying && gritoReDead != null)
                {
                    audioSource.PlayOneShot(gritoReDead);
                    tiempoDesdeUltimoGrito = 0f;
                }

                if (!jugadorCongelado)
                {
                    PlayerMovement.GetComponent<PlayerMovement>().isFrozen = true;
                    StartCoroutine(CongelarJugador());
                }

                agente.SetDestination(PlayerMovement.position);
                anim.SetBool("Caminando", true);
                anim.SetBool("Ataca", false);

                if (Vector3.Distance(PlayerMovement.position, transform.position) < rangoAtaque)
                {
                    StartCoroutine(Atacar());
                }
            }
            else
            {
                tiempoDesdeUltimoGrito += Time.deltaTime;
            }
        }
        else
        {
            anim.SetBool("Caminando", false);
        }

        if (!objetivoDetectado)
        {
            if (Time.time >= tiempoSiguienteCambio)
            {
                MoverAleatoriamente();
                tiempoSiguienteCambio = Time.time + tiempoCambioDestino;
            }
            anim.SetBool("Caminando", true);
        }
    }

    private void ComportamientoMoblin()
    {
        if (objetivoDetectado)
        {
            agente.SetDestination(PlayerMovement.position);
            anim.SetBool("Caminando", false);
            anim.SetBool("Ataca", true);

            if (!atacando) StartCoroutine(Atacar());
        }
        else
        {
            anim.SetBool("Ataca", false);

            if (Time.time >= tiempoSiguienteCambio)
            {
                MoverAleatoriamente();
                tiempoSiguienteCambio = Time.time + tiempoCambioDestino;
            }

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
            transform.localScale = this.transform.position.x > PlayerMovement.position.x ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
        }
    }

    IEnumerator Atacar()
    {
        atacando = true;
        anim.SetTrigger("Ataca");

        if (objetivoDetectado && colEspada != null && colEspada.bounds.Intersects(PlayerMovement.GetComponent<Collider2D>().bounds))
        {
            PlayerMovement.GetComponent<PlayerMovement>().CausarHerida(daño);
        }

        yield return new WaitForSeconds(tiempoEntreAtaques);

        atacando = false;
    }

    IEnumerator CongelarJugador()
    {
        jugadorCongelado = true;
        Rigidbody2D jugadorRb = PlayerMovement.GetComponent<Rigidbody2D>();

        if (jugadorRb != null)
        {
            jugadorRb.velocity = Vector2.zero;
            jugadorRb.isKinematic = true;
        }

        yield return new WaitForSeconds(duracionCongelado);

        if (jugadorRb != null)
        {
            jugadorRb.isKinematic = false;
        }

        jugadorCongelado = false;
        PlayerMovement.GetComponent<PlayerMovement>().isFrozen = false;
    }

    // Métodos de tipos de daño
    public void AplicarDañoNormal(float cantidad)
    {
        if (armadura > 0)
        {
            armadura -= cantidad;
            if (armadura < 0) vida += armadura;
        }
        else
        {
            vida -= cantidad;
        }
        StartCoroutine(MostrarIndicadorDeDaño());
    }

    public void AplicarDañoPenetrante(float cantidad)
    {
        vida -= cantidad;
        StartCoroutine(MostrarIndicadorDeDaño());
    }

    public void AplicarDañoMagico(float cantidad)
    {
        if (armadura > 0)
        {
            armadura -= cantidad;
            if (armadura < 0) vida += armadura;
        }
        else
        {
            vida -= cantidad;
        }
        StartCoroutine(AplicarVeneno(3f, 1f));
        StartCoroutine(MostrarIndicadorDeDaño());
    }

    private IEnumerator AplicarVeneno(float duracion, float tickRate)
    {
        float tiempoTranscurrido = 0;
        while (tiempoTranscurrido < duracion && vida > 0)
        {
            vida -= 1f;
            tiempoTranscurrido += tickRate;
            yield return new WaitForSeconds(tickRate);
        }
    }

    private IEnumerator MostrarIndicadorDeDaño()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
}

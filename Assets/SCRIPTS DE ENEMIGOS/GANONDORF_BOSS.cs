using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GANONDORF_BOSS : MonoBehaviour
{
    public float vida;
    public float vidaMaxima = 200f;
    public float armadura;
    public float daño = 10f;

    public Slider slider;
    public Canvas canvas;
    public List<GameObject> enemyPrefabs;
    public GameObject deathSpawnEnemyPrefab;
    private bool haInvocadoEnemigos = false;

    private Transform playerTransform;
    private NavMeshAgent agente;
    private bool objetivoDetectado = false;
    private Animator anim;
    private bool atacando = false;
    public float rangoAtaque = 10f;
    public float tiempoEntreAtaques = 2f;
    private Collider2D colEspada;

    public AudioClip[] audiosGolpe;
    public AudioClip sonidoMuerte;
    private AudioSource audioSource;
    public AnimationClip animacionMuerte;
    private bool estaMuerto = false;

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
        vida = vidaMaxima;

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
        if (estaMuerto) return; // No ejecuta nada más si está muerto
        if (playerTransform == null) return;

        if (vida <= 0 && !estaMuerto)
        {
            estaMuerto = true;
            StartCoroutine(Morir()); // Inicia la secuencia de muerte
            return;
        }

        if (slider != null)
        {
            slider.value = vida;
        }

        // Invocar enemigos cuando la vida alcanza la mitad
        if (vida <= vidaMaxima / 2 && !haInvocadoEnemigos)
        {
            InvocarEnemigos();
            haInvocadoEnemigos = true;
        }

        float distancia = Vector3.Distance(playerTransform.position, transform.position);
        objetivoDetectado = distancia < rangoAtaque;

        ComportamientoGanondorf();
        RotarEnemigo();

        // Depuración de daño
        if (Input.GetKeyDown(KeyCode.J)) RecibirDaño(10f, "normal");
        else if (Input.GetKeyDown(KeyCode.I)) RecibirDaño(10f, "penetrante");
        else if (Input.GetKeyDown(KeyCode.K)) RecibirDaño(10f, "magico");
    }

    private IEnumerator Morir()
    {
        // Activamos inmediatamente el trigger "Muerto" para iniciar la animación de muerte
        anim.SetTrigger("Muerto");

        // Detenemos el NavMeshAgent para que el enemigo se quede quieto
        agente.enabled = false;

        // Desactiva cualquier otra animación en ejecución
        anim.ResetTrigger("Ataca");
        anim.SetBool("Caminando", false);

        // Reproduce el sonido de muerte si existe
        if (sonidoMuerte != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoMuerte);
            yield return new WaitForSeconds(sonidoMuerte.length);
        }
        else if (animacionMuerte != null)
        {
            // Espera la duración de la animación de muerte
            yield return new WaitForSeconds(animacionMuerte.length);
        }

        SpawnDeathEnemy();  // Genera el enemigo de muerte
        Destroy(gameObject); // Destruye el objeto tras la animación y el sonido
    }

    private void SpawnDeathEnemy()
    {
        if (deathSpawnEnemyPrefab != null)
        {
            Instantiate(deathSpawnEnemyPrefab, transform.position, Quaternion.identity);
            Debug.Log("Enemigo generado al morir!");
        }
        else
        {
            Debug.LogWarning("Prefab de enemigo de muerte no asignado.");
        }
    }

    private void InvocarEnemigos()
    {
        if (enemyPrefabs.Count >= 2)
        {
            Vector3 posicionIzquierda = transform.position + Vector3.left * 2;
            Vector3 posicionDerecha = transform.position + Vector3.right * 2;

            Instantiate(enemyPrefabs[0], posicionIzquierda, Quaternion.identity);
            Instantiate(enemyPrefabs[1], posicionDerecha, Quaternion.identity);

            Debug.Log("Enemigos invocados!");
        }
        else
        {
            Debug.LogWarning("No hay suficientes prefabs de enemigos asignados.");
        }
    }

    private void ComportamientoGanondorf()
    {
        if (estaMuerto) return; // No ejecuta si está muerto

        if (objetivoDetectado)
        {
            agente.SetDestination(playerTransform.position);
            anim.SetBool("Caminando", true);

            if (!atacando && Vector3.Distance(playerTransform.position, transform.position) < rangoAtaque / 2)
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
        if (estaMuerto) return; // No ejecuta si está muerto

        Vector3 posicionAleatoria = transform.position + Random.insideUnitSphere * rangoMovimientoAleatorio;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(posicionAleatoria, out hit, rangoMovimientoAleatorio, NavMesh.AllAreas))
        {
            agente.SetDestination(hit.position);
        }
    }

    private void RotarEnemigo()
    {
        if (estaMuerto) return; // No ejecuta si está muerto

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
        if (estaMuerto || anim.GetCurrentAnimatorStateInfo(0).IsName("Muerte")) yield break; // No ataca si está muerto o en animación de muerte

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
        if (audioSource != null && audiosGolpe.Length > 0 && !estaMuerto) // No reproduce sonido si está muerto
        {
            int indiceAleatorio = Random.Range(0, audiosGolpe.Length);
            audioSource.PlayOneShot(audiosGolpe[indiceAleatorio]);
        }
    }

    public void RecibirDaño(float cantidad, string tipoDaño)
    {
        if (estaMuerto) return; // No recibe daño si está muerto

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

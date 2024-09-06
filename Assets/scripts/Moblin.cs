using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform PlayerMovement;
    public Transform[] puntosRuta;
    private int indiceRuta = 0;
    private NavMeshAgent agente;
    private bool objetivoDetectado = false;
    private Transform objetivo;
    private Animator anim;
    private bool atacando = false;
    public float rangoAtaque = 10f;
    public float tiempoEntreAtaques = 3f;
    public Collider2D colEspada;

    private void Awake()
    {
        agente = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        agente.updateRotation = false;
        agente.updateUpAxis = false;

        if (puntosRuta.Length > 0)
        {
            agente.SetDestination(puntosRuta[indiceRuta].position);
            objetivo = puntosRuta[indiceRuta];
        }
    }

    private void Update()
    {
        if (PlayerMovement == null)
        {
            return;
        }

        float distancia = Vector3.Distance(PlayerMovement.position, this.transform.position);
        objetivoDetectado = distancia < rangoAtaque;

        if (objetivoDetectado)
        {
            agente.SetDestination(PlayerMovement.position);
            objetivo = PlayerMovement;
            anim.SetBool("Caminando", false);
            anim.SetBool("Ataca", true);

            if (!atacando)
            {
                StartCoroutine(Atacar());
            }
        }
        else
        {
            if (!agente.pathPending && agente.remainingDistance <= agente.stoppingDistance)
            {
                indiceRuta = (indiceRuta + 1) % puntosRuta.Length;
                agente.SetDestination(puntosRuta[indiceRuta].position);
                objetivo = puntosRuta[indiceRuta];
                anim.SetBool("Caminando", true);
                anim.SetBool("Ataca", false);
            }
        }
        RotarOrco();
    }

    void RotarOrco()
    {
        if (objetivo != null)
        {
            if (this.transform.position.x > objetivo.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    IEnumerator Atacar()
    {
        atacando = true;
        anim.SetTrigger("Ataca");

        yield return new WaitForSeconds(tiempoEntreAtaques);

        atacando = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == colEspada)
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }


    public void HacerDanioAlJugador()
    {
        if (objetivoDetectado && !atacando)
        {
        }
    }
}

// Assets/Scripts/Inventory/Weapon.cs
using UnityEngine;
using System.Collections;

using static Enemigo;

public class Weapon : MonoBehaviour
{


    public GameObject hand;
    public GameObject weapon;
    public float dcc, dpc, dmc, complejidad, limiteEfectos, limiteEncantamientos, ejecucion, cEstamina, cEsencia, cEnergia, cMana, alcance;

    GameManager gameManager;
    public PlayerMovement playerMovement;
    private GameObject objetoColisionado;
    public bool facingRight;
    public void SetHand(GameObject setHand)
    {
        this.hand = setHand;
    }
    private void Update()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }



        if (hand != null)
        {
            if (estadoAtacando)
            {
                weapon.GetComponent<BoxCollider2D>().enabled = true;
            }
            else
            {
                weapon.GetComponent<BoxCollider2D>().enabled = false;

            }
            if (!estadoAtacando && gameManager.GetStat("Esencia") > 0)
            {

                if (Input.GetKeyDown(gameManager.keyMappings["Attack1"]) && playerMovement.escenciaAct >= cEsencia)
                {
                    StartCoroutine(AnimarAtaqueJ(hand, facingRight));
                    playerMovement.escenciaAct = playerMovement.escenciaAct - cEsencia;
                    Debug.Log("ATAQUE 1 REALIZADO");
                }
                else if (Input.GetKeyDown(gameManager.keyMappings["Attack2"]) && playerMovement.escenciaAct >= cEsencia)
                {
                    StartCoroutine(AnimarAtaqueK(hand, facingRight));
                    playerMovement.escenciaAct = playerMovement.escenciaAct - cEsencia;
                    Debug.Log("ATAQUE 2 REALIZADO");
                }
                else if (Input.GetKeyDown(gameManager.keyMappings["Attack3"]) && playerMovement.escenciaAct >= cEsencia)
                {
                    StartCoroutine(AnimarAtaqueL(hand, hand.transform, facingRight));
                    playerMovement.escenciaAct = playerMovement.escenciaAct - cEsencia;
                    Debug.Log("ATAQUE 3 REALIZADO");
                }
                else if (Input.GetKeyDown(gameManager.keyMappings["Attack4"]) && playerMovement.escenciaAct >= cEsencia)
                {
                    StartCoroutine(AnimarAtaqueI(hand, facingRight));
                    playerMovement.escenciaAct = playerMovement.escenciaAct - cEsencia;
                    Debug.Log("ATAQUE 4 REALIZADO");
                }

            }
        }
    }


    private void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        objetoColisionado = collision.gameObject;
        Debug.Log("Colisionaste con: " + objetoColisionado.name);
        //Eliminamos la parte que hace daño al enemigo
         if (objetoColisionado.GetComponent<EnemyMovement>() != null)
        {
            objetoColisionado.GetComponent<EnemyMovement>().AplicarDañoNormal(gameManager.GetStat("Fuerza"));
            Debug.Log("El enemigo: " + objetoColisionado.name + " recibio daño: " + gameManager.GetStat("Fuerza"));
        }
    }

    public bool estadoAtacando = false;
    public bool estadoUsando = false;
    public int masas;
    public float cadencia;

    public IEnumerator AnimarAtaqueJ(GameObject espada, bool facingRight)
    {
        estadoAtacando = true;
        Vector3 originalPosition = new Vector3(0, 0, 0);
        Quaternion originalRotation = Quaternion.Euler(0, 0, 0);

        // Movimiento hacia adelante
        espada.transform.position += new Vector3(1, 0, 0);

        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < ejecucion)
        {
            // Calcula el tiempo normalizado (0 a 1)
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / ejecucion;
            float r = 90;
            // Gira la espada
            if (facingRight)
            {
                r = -90;
            }
            else
            {
                r = 90;
            }
            espada.transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, r, t)); // Rota 45 grados en Z
            yield return null; // Espera un frame
        }

        // Asegúrate de que la espada termine en la rotación final
        espada.transform.rotation = Quaternion.Euler(0, 0, 45);

        // Regresar la espada a la posición original
        espada.transform.position = originalPosition;

        // Regresar a la rotación original
        espada.transform.rotation = originalRotation;
        estadoAtacando = false;
    }

    public IEnumerator AnimarAtaqueL(GameObject espada, Transform mano, bool facingRight)
    {
        estadoAtacando = true;
        Vector3 originalPosition = mano.transform.localPosition;
        Quaternion originalRotation = mano.transform.localRotation;

        // Asegurarse de que la espada esté en posición horizontal antes de la estocada
        mano.transform.localRotation = Quaternion.Euler(0, 0, -90);

        float tiempoTranscurrido = 0f;
        float avanceDistancia = 0.5f; // Distancia de la estocada
        float ejecucion = 0.2f; // Duración de la animación

        // Movimiento de estocada hacia adelante
        while (tiempoTranscurrido < ejecucion)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / ejecucion;

            mano.transform.localPosition = originalPosition + new Vector3(1 * avanceDistancia * t, 0, 0);
            yield return null; // Espera un frame
        }

        // Regresar la mano a la posición y rotación originales
        mano.transform.localPosition = originalPosition;
        mano.transform.localRotation = originalRotation;
        estadoAtacando = false;
    }

    public IEnumerator AnimarAtaqueI(GameObject mano, bool facingRight)
    {
        estadoAtacando = true;
        Vector3 originalPosition = mano.transform.localPosition;
        Quaternion originalRotation = mano.transform.localRotation;

        // Posición de cobertura (levanta la mano con la espada)
        Vector3 targetPosition = originalPosition - new Vector3(0, 0.5f, 0); // Mueve hacia arriba
        float anguloDefensa = 45; // Rotación para cubrirse

        float tiempoTranscurrido = 0f;
        float duracionMovimiento = ejecucion * 0.5f; // Reduce la duración para un movimiento más rápido
        float pausaCobertura = 0.8f; // Incrementa la duración de la pausa en la posición de cobertura

        while (tiempoTranscurrido < duracionMovimiento)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / duracionMovimiento;

            // Mueve y rota la mano para cubrirse
            mano.transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, t);
            mano.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, anguloDefensa, t));

            yield return null;
        }

        // Mantiene la posición de cobertura
        mano.transform.localPosition = targetPosition;
        mano.transform.localRotation = Quaternion.Euler(0, 0, anguloDefensa);

        // Pausa más larga en la posición de cobertura
        yield return new WaitForSeconds(pausaCobertura);

        // Regresa a la posición y rotación originales
        mano.transform.localPosition = originalPosition;
        mano.transform.localRotation = originalRotation;
        estadoAtacando = false;
    }


    public IEnumerator AnimarAtaqueK(GameObject mano, bool facingRight)
    {
        estadoAtacando = true;
        Vector3 originalPosition = mano.transform.localPosition;
        Quaternion originalRotation = mano.transform.localRotation;

        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < ejecucion)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / ejecucion;

            // Rota la mano en un círculo completo (360 grados)
            float angulo = -360;
            mano.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, angulo, t));

            // Mantén la mano en la posición original
            mano.transform.localPosition = originalPosition;

            yield return null;
        }

        // Asegura que la rotación y posición regresen a sus valores originales
        mano.transform.localPosition = originalPosition;
        mano.transform.localRotation = originalRotation;
        estadoAtacando = false;
    }
}

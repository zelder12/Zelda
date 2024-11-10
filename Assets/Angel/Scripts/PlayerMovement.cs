using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public int maxGolpes = 10;
    public int maxVida = 100; // Define el máximo de vida

    //ANGEL
    public float movimiento = 5f;
    public float carrera = 10f;
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private bool facingRight = true;
    private bool estadoAtacando = false;

    private Vector3 leftHandUpPosition = new Vector3(-0.6140003f, -0.3506131f, 0);
    private Vector3 leftHandDownPosition = new Vector3(-0.6140003f, -0.4006124f, 0);

    private Vector3 rightHandUpPosition = new Vector3(0.7860003f, -0.3506131f, 0);
    private Vector3 rightHandDownPosition = new Vector3(0.7860003f, -0.4006124f, 0);

    public GameObject armaL;
    public GameObject armaR;

    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject Player;

    public SpriteController spriteController;
    //FIN

    [SerializeField] private float velocidadNormal = 7f;
    [SerializeField] private float velocidadRápida = 10f;
    [SerializeField] private BoxCollider2D ColEspada;
    [SerializeField] private AudioClip sonidoHerida1;
    [SerializeField] private AudioClip sonidoHerida2;
    [SerializeField] private UIManager uIManager;

    [SerializeField] private GameObject burbujaVisual;

    private Rigidbody2D rig;
    private Animator animator;
    private SpriteRenderer spritePersonaje;
    private AudioSource audioSource;
    private Collider2D playerCollider;
    public int vidaPersonaje = 3;
    public int golpesRecibidos = 0;
    public Vector3 burbujaOffset;

    private Vector3 direccionMovimiento;

    private float velocidadActual;
    private bool velocidadIncrementada = false;
    private float velocidadRápidaOriginal;
    public bool invulnerable = false;
    private bool burbujaActiva = false;
    private bool parpadeando = false;
    private bool estaMuerto = false;

    private Inventario inventarioScript;
    public bool isFrozen = false; // Agregado para controlar el estado de congelación

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        spritePersonaje = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        playerCollider = GetComponent<Collider2D>();
        burbujaOffset = new Vector3(0.3f, 0, 0);

        inventarioScript = GetComponent<Inventario>();
        if (inventarioScript == null)
        {
            Debug.LogError("No se encontró el componente Inventario en el GameObject.");
        }

        if (audioSource == null)
        {
            Debug.LogError("No se encontró el componente AudioSource en el GameObject.");
        }

        // Automatically find UIManager and BoxCollider2D
        uIManager = FindObjectOfType<UIManager>();
        ColEspada = GetComponentInChildren<BoxCollider2D>();

        if (uIManager == null)
        {
            Debug.LogError("No se encontró el componente UIManager en la escena.");
        }

        if (ColEspada == null)
        {
            Debug.LogError("No se encontró el BoxCollider2D del arma.");
        }

        velocidadActual = velocidadNormal;
        velocidadRápidaOriginal = velocidadRápida;
    }

    private void Update()
    {
        if (Player == null)
        {
            foreach (Transform child in transform)
            {
                if (child.name == "Knight(Clone)" || child.name == "Rogue(Clone)" || child.name == "Wizard(Clone)")
                {
                    Player = child.gameObject;
                    animator = Player.GetComponent<Animator>();
                    spriteController = Player.GetComponent<SpriteController>();
                    leftHand = Player.transform.Find("LeftHand").gameObject;
                    rightHand = Player.transform.Find("RightHand").gameObject;

                    if (leftHand == null || rightHand == null)
                    {
                        Debug.LogError("No se encontraron las manos del personaje");
                    }
                }
                Debug.Log("El componente se llama: " + child.name);
            }
        }
        else
        {
            Estado_Idle();
            Movimiento();
            IntanciarArmas();
        }

        if (estaMuerto) return;
        if (inventarioScript.Activar_inv)
        {
            rig.velocity = Vector2.zero; // Detener cualquier movimiento
            direccionMovimiento = Vector3.zero;
            return;
        }

        CheckTilemapColisiones(); // Check for tilemap collisions
    }

    private void Start()
    {
        vidaPersonaje = maxGolpes;
        if (uIManager == null)
        {
            Debug.LogWarning("UIManager no está asignado en el inspector.");
        }
    }

    void Movimiento()
    {
        // Obtener las entradas de teclado (ejes horizontales y verticales)
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // Crear el vector de movimiento
        Vector3 moveDirection = new Vector3(moveX, moveY, 0);

        // Aplicar el movimiento al transform del objeto
        if (!estadoAtacando && !isFrozen) // Verificar si el jugador está congelado
        {
            transform.position += moveDirection * movimiento * Time.deltaTime;
            Debug.Log("El move Direccion es:" + moveDirection + " y el movimiento es:" + movimiento);
            if (moveX > 0 && !facingRight)
            {
                FlipSprites();  // Voltea a la derecha
            }
            else if (moveX < 0 && facingRight)
            {
                FlipSprites();  // Voltea a la izquierda
            }
        }

        // Aquí verificamos si Shift está presionado para aumentar la velocidad solo temporalmente
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            velocidadIncrementada = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            velocidadIncrementada = false;
        }

        // Si Shift está presionado, aumentar la velocidad de movimiento
        if (velocidadIncrementada)
        {
            movimiento = velocidadNormal + carrera; // Incrementa velocidad base con carrera
        }
        else
        {
            movimiento = velocidadNormal; // Restablece a la velocidad normal cuando Shift se suelta
        }

        Estado_Walk();
        Estado_Run();
        Estado_Idle();
    }


    void FlipSprites()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void IntanciarArmas()
    {
        Transform hijo1 = Player.transform.GetChild(0);
        Transform hijo2 = Player.transform.GetChild(1);

        if (leftHand.transform.childCount < 1 && armaL != null)
        {
            Debug.Log("SE INSTANCIO EL ARMA L");
            GameObject instancia1 = Instantiate(armaL, hijo1.position, hijo1.rotation);
            instancia1.GetComponent<Weapon>().SetHand(rightHand);
            instancia1.transform.SetParent(hijo1);
            armaL = instancia1;
        }

        if (rightHand.transform.childCount < 1 && armaR != null)
        {
            Debug.Log("SE INSTANCIO EL ARMA R");
            GameObject instancia2 = Instantiate(armaR, hijo2.position, hijo2.rotation);
            instancia2.GetComponent<Weapon>().SetHand(rightHand);
            instancia2.transform.SetParent(hijo2);
            armaR = instancia2;
        }
    }

    public void Estado_Idle()
    {
        Inventario inventario = GetComponent<Inventario>();
        List<GameObject> Equipo = inventario.EquipoObjetos;

        float speed = 3.5f; // Velocidad de la animación
        // Animación para la mano izquierda
        leftHand.transform.localPosition = Vector3.Lerp(leftHandDownPosition, leftHandUpPosition, Mathf.PingPong(Time.time * speed, 1));
        // Animación para la mano derecha
        rightHand.transform.localPosition = Vector3.Lerp(rightHandDownPosition, rightHandUpPosition, Mathf.PingPong(Time.time * speed, 1));

        if (rightHand.transform.childCount > 0)
        {
            var weapon = rightHand.transform.GetChild(0).GetComponent<Weapon>();
            if (weapon != null && weapon.weaponItem != null)  // Verificar que Weapon y weaponItem no sean null
            {
                weapon.facingRight = facingRight;
                estadoAtacando = weapon.weaponItem.estadoAtacando;
                Debug.Log("EL ARMA R SE CAMBIO DE DIRECCION " + facingRight);
            }
            else
            {
                Debug.LogError("No se encontró un Weapon o weaponItem en la mano derecha");
            }
        }

        if (leftHand.transform.childCount > 0)
        {
            var weapon = leftHand.transform.GetChild(0).GetComponent<Weapon>();
            if (weapon != null && weapon.weaponItem != null)  // Verificar que Weapon y weaponItem no sean null
            {
                weapon.facingRight = facingRight;
                estadoAtacando = weapon.weaponItem.estadoAtacando;
                Debug.Log("EL ARMA L SE CAMBIO DE DIRECCION " + facingRight);
            }
            else
            {
                Debug.LogError("No se encontró un Weapon o weaponItem en la mano izquierda");
            }
        }

        if (leftHand.transform.childCount > 1)
        {
            leftHand.GetComponent<SpriteRenderer>().sprite = spriteController.leftHandUsing;
        }
        else
        {
            leftHand.GetComponent<SpriteRenderer>().sprite = spriteController.leftHand;
        }

        if (rightHand.transform.childCount > 1)
        {
            rightHand.GetComponent<SpriteRenderer>().sprite = spriteController.rightHandUsing;
        }
        else
        {
            rightHand.GetComponent<SpriteRenderer>().sprite = spriteController.rightHand;
        }
    }

    public void Estado_Walk()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 direccionMovimiento = new Vector2(horizontal, vertical).normalized;

        bool isMoving = direccionMovimiento.magnitude > 0;
        if (isMoving)
        {
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    public void Estado_Run()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 direccionMovimiento = new Vector2(horizontal, vertical).normalized;

        bool isMoving = direccionMovimiento.magnitude > 0;
        if (isMoving && Input.GetKeyDown(KeyCode.LeftShift))
        {
            animator.SetBool("IsRun", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            animator.SetBool("IsRun", false);
        }
    }

    // Nuevo código agregado aquí
    private void OnEnable()
    {
        // Se suscribe al evento cuando la escena es cargada
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Se desuscribe del evento cuando el objeto se desactiva
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Método que se ejecuta cuando una escena es cargada
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Restablece las velocidades al valor original cuando cambias de escena
        velocidadNormal = 7f;
        velocidadRápida = velocidadRápidaOriginal;
        velocidadIncrementada = false;
    }
    public void Estado_Deaht()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 direccionMovimiento = new Vector2(horizontal, vertical).normalized;

        bool isMoving = direccionMovimiento.magnitude > 0;
        if (isMoving)
        {
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }
    public void CausarHerida(float daño)
    {
        if (vidaPersonaje > 0 && !invulnerable && !burbujaActiva)
        {
            vidaPersonaje -= (int)daño; // Convertir a int si vidaPersonaje es un entero
            golpesRecibidos++;

            // Llamada a RecibirDaño utilizando vida actual y vida máxima
            uIManager.RecibirDaño(vidaPersonaje, maxVida);

            if (audioSource != null && (sonidoHerida1 != null || sonidoHerida2 != null))
            {
                AudioClip sonidoSeleccionado = Random.value > 0.5f ? sonidoHerida1 : sonidoHerida2;
                audioSource.PlayOneShot(sonidoSeleccionado);
            }
            else
            {
                Debug.LogWarning("AudioSource o clips de sonido no están configurados correctamente.");
            }

            if (vidaPersonaje <= 0)
            {
                animator.SetBool("Death", true);
                estaMuerto = true;
                ConvertirASolido();
                Invoke(nameof(Morir), 1f);
            }
            else
            {
                StartCoroutine(ActivarInvulnerabilidad());

                // Llamada corregida a QuitarSangre con los parámetros requeridos
                uIManager.QuitarSangre(vidaPersonaje, maxVida);
            }
        }
    }




    private void ActualizarBurbujaVisual()
    {
        if (burbujaVisual != null)
        {
            bool isFlipped = spritePersonaje.flipX;

            if (isFlipped)
            {
                burbujaVisual.transform.position = GameObject.FindWithTag("Player").transform.position - burbujaOffset;
            }
            else
            {
                burbujaVisual.transform.position = GameObject.FindWithTag("Player").transform.position + burbujaOffset;
            }

            Vector3 escalaBurbuja = new Vector3(4f, 4f, 0); // Ajusta según tus necesidades
            burbujaVisual.transform.localScale = escalaBurbuja;
        }
    }

    public void ActivarEscudoBurbuja(float duracion)
    {
        if (estaMuerto) return;

        // Activar invulnerabilidad
        burbujaActiva = true;
        invulnerable = true;
        Debug.Log("Escudo Burbuja activado!");

        // Mostrar la burbuja visual
        if (burbujaVisual != null)
        {
            burbujaVisual.SetActive(true);
        }

        // Detener cualquier Coroutine anterior que pudiera cambiar la invulnerabilidad
        StopCoroutine(DesactivarEscudoBurbuja(duracion));
        StartCoroutine(DesactivarEscudoBurbuja(duracion));
    }

    private IEnumerator DesactivarEscudoBurbuja(float duracion)
    {
        // Esperar la duración del escudo
        yield return new WaitForSeconds(duracion);

        // Desactivar invulnerabilidad
        burbujaActiva = false;
        invulnerable = false;

        // Ocultar la burbuja visual
        if (burbujaVisual != null)
        {
            burbujaVisual.SetActive(false);
        }

        Debug.Log("Escudo Burbuja desactivado!");
    }

    private IEnumerator ActivarInvulnerabilidad()
    {
        invulnerable = true;
        StartCoroutine(Parpadear());

        yield return new WaitForSeconds(1f);
        spritePersonaje.color = Color.white;
        parpadeando = false;
        if (!burbujaActiva)
        {
            invulnerable = false;
        }
    }

    private IEnumerator Parpadear()
    {
        parpadeando = true;
        while (parpadeando)
        {
            spritePersonaje.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spritePersonaje.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void ConvertirASolido()
    {
        rig.bodyType = RigidbodyType2D.Static;
    }

    private void Morir()
    {
        if (uIManager != null)
        {
            uIManager.IniciarGameOver();
        }

        Destroy(this.gameObject);
    }

    public void Curar(int cantidad, bool curacionCompleta)
    {
        if (estaMuerto) return;

        if (curacionCompleta)
        {
            vidaPersonaje = maxVida; // Restaura la vida completa
            uIManager.QuitarSangre(vidaPersonaje, maxVida); // Actualiza la sangre basada en la vida
        }
        else
        {
            vidaPersonaje += cantidad;
            vidaPersonaje = Mathf.Min(vidaPersonaje, maxVida); // Asegura que la vida no exceda maxVida

            // Llama a QuitarSangre para actualizar la pantalla de sangre en función de la vida
            uIManager.QuitarSangre(vidaPersonaje, maxVida);
        }

        // Si la vida es suficiente, detener la música de estado crítico
        if (vidaPersonaje > 20)
        {
            uIManager.DetenerMusicaCorazonBajo();
        }
    }

    public void AumentarVida(int cantidad)
    {
        if (estaMuerto) return;

        vidaPersonaje += cantidad;
        vidaPersonaje = Mathf.Min(vidaPersonaje, maxVida); // Asegura que la vida no exceda maxVida

        // Llama a QuitarSangre para actualizar la pantalla de sangre en función de la vida
        uIManager.QuitarSangre(vidaPersonaje, maxVida);

        // Detener la música si la vida está por encima del umbral crítico
        if (vidaPersonaje > 20)
        {
            uIManager.DetenerMusicaCorazonBajo();
        }
    }



    public void AumentarVelocidad(float porcentaje)
    {
        if (estaMuerto) return;

        if (!velocidadIncrementada)
        {
            velocidadNormal += velocidadNormal * porcentaje;
            velocidadRápida += velocidadRápidaOriginal * porcentaje;
            velocidadIncrementada = true;
            StartCoroutine(ReiniciarVelocidad());
        }
    }

    private IEnumerator ReiniciarVelocidad()
    {
        yield return new WaitForSeconds(10f);
        velocidadNormal = 7f;
        velocidadRápida = 10f;
        velocidadIncrementada = false;
    }

    private void CheckTilemapColisiones()
    {
        // Assuming you have a LayerMask for the tilemap
        LayerMask tilemapLayer = LayerMask.GetMask("TilemapLayer"); // Replace with your actual layer name
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero, 0.1f, tilemapLayer);

        if (hit.collider != null)
        {
            Vector2 pushDirection = (transform.position - hit.collider.transform.position).normalized;
            transform.position += (Vector3)pushDirection * 0.04f; // Push the player away by 0.04 units
        }
    }
}
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;


public class PlayerMovement : MonoBehaviour
{

    //ANGEL
    public float movimiento = 5f;
    public float carrera = 10f;
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private bool facingRight = true;
    private bool estadoAtacando= false;

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

        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    animator.SetTrigger("Ataca");
        //}
        //ActualizarBurbujaVisual();
    }
    private void Start()
    {

    }
    //private void FixedUpdate()
    //{
    //    if (estaMuerto) return;
    //    if (inventarioScript.Activar_inv)
    //    {
    //        rig.velocity = Vector2.zero; // Detener la velocidad
    //        return;
    //    }
    //    Movimiento();
    //}
    void Movimiento()
    {
        // Obtener las entradas de teclado (ejes horizontales y verticales)
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // Crear el vector de movimiento
        Vector3 moveDirection = new Vector3(moveX, moveY, 0);

        // Aplicar el movimiento al transform del objeto
        if (!estadoAtacando)
        {
            transform.position += moveDirection * movimiento * Time.deltaTime;
            if (moveX > 0 && !facingRight)
            {
                FlipSprites();  // Voltea a la derecha
            }
            else if (moveX < 0 && facingRight)
            {
                FlipSprites();  // Voltea a la izquierda
            }
        }



        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            movimiento = movimiento + carrera;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            movimiento = movimiento - carrera;
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
        // Si tienes múltiples SpriteRenderers para las partes del cuerpo, invierte también sus sprites
        /*
        leftHandRenderer.flipX = flip;
        rightHandRenderer.flipX = !flip;
        leftWeaponRenderer.flipX = flip;
        rightWeaponRenderer.flipX = !flip;
        */
    }
    //private void Movimiento()
    //{
    //    float horizontal = Input.GetAxis("Horizontal");
    //    float vertical = Input.GetAxis("Vertical");
    //    Vector2 direccionMovimiento = new Vector2(horizontal, vertical).normalized;

    //    bool isMoving = direccionMovimiento.magnitude > 0;
    //    bool isRunning = Input.GetKey(KeyCode.LeftShift);

    //    // Actualiza la velocidad dependiendo de si está corriendo o no
    //    velocidadActual = isRunning ? velocidadRápida : velocidadNormal;

    //    // Establece la velocidad del Rigidbody solo si el personaje se está moviendo
    //    rig.velocity = direccionMovimiento * velocidadActual;

    //    if (isMoving)
    //    {
    //        animator.SetFloat("Speed", rig.velocity.magnitude);

    //        // Activa o desactiva la animación de correr inmediatamente
    //        animator.SetBool("Corriendo", isRunning);
    //    }
    //    else
    //    {
    //        // Detén cualquier movimiento residual
    //        rig.velocity = Vector2.zero;
    //        animator.SetFloat("Speed", 0);
    //        animator.SetBool("Corriendo", false);
    //    }

    //    if (horizontal > 0)
    //    {
    //        ColEspada.offset = new Vector2(1, 0);
    //        spritePersonaje.flipX = false;
    //    }
    //    else if (horizontal < 0)
    //    {
    //        ColEspada.offset = new Vector2(-1, 0);
    //        spritePersonaje.flipX = true;
    //    }
    //    else if (vertical > 0)
    //    {
    //        ColEspada.offset = new Vector2(1, 0);
    //    }
    //    else if (vertical < 0)
    //    {
    //        ColEspada.offset = new Vector2(-1, 0);
    //    }
    //}
    void IntanciarArmas()
    {
        Transform hijo1 = Player.transform.GetChild(0);
        Transform hijo2 = Player.transform.GetChild(1);

        if (leftHand.transform.childCount < 1 && armaL != null) {
            Debug.Log("SE INSTANCIO EL ARMA L");
            GameObject instancia1 = Instantiate(armaL, hijo1.position, hijo1.rotation);
            instancia1.GetComponent<Weapon>().SetHand(rightHand);
            instancia1.transform.SetParent(hijo1);
            armaL = instancia1;
        }

        if (rightHand.transform.childCount < 1 && armaR != null )
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

        Transform rightWeanpon;
        Transform leftWeanpon;

        float speed = 3.5f; // Velocidad de la animación
        // Animación para la mano izquierda
        leftHand.transform.localPosition = Vector3.Lerp(leftHandDownPosition, leftHandUpPosition, Mathf.PingPong(Time.time * speed, 1));
        // Animación para la mano derecha
        rightHand.transform.localPosition = Vector3.Lerp(rightHandDownPosition, rightHandUpPosition, Mathf.PingPong(Time.time * speed, 1));

        if (rightHand.transform.childCount > 0)
        {
            rightHand.transform.GetChild(0).GetComponent<Weapon>().facingRight = facingRight;
            estadoAtacando = rightHand.transform.GetChild(0).GetComponent<Weapon>().weaponItem.estadoAtacando;
            Debug.Log("EL ARMA R SE CAMBIO DE DIRECCION" + facingRight);
        }
        if (leftHand.transform.childCount > 0)
        {
            leftHand.transform.GetChild(0).GetComponent<Weapon>().facingRight = facingRight;
            estadoAtacando = leftHand.transform.GetChild(0).GetComponent<Weapon>().weaponItem.estadoAtacando;
            Debug.Log("EL ARMA L SE CAMBIO DE DIRECCION" + facingRight);
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
        else {
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
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            animator.SetBool("IsRun", false);
        }
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
    public void CausarHerida()
    {
        if (vidaPersonaje > 0 && !invulnerable && !burbujaActiva)
        {
            vidaPersonaje--;
            golpesRecibidos++;
            uIManager.RecibirDaño(golpesRecibidos);

            if (audioSource != null && (sonidoHerida1 != null || sonidoHerida2 != null))
            {
                AudioClip sonidoSeleccionado = Random.value > 0.5f ? sonidoHerida1 : sonidoHerida2;
                audioSource.PlayOneShot(sonidoSeleccionado);
            }
            else
            {
                Debug.LogWarning("AudioSource o clips de sonido no están configurados correctamente.");
            }

            if (vidaPersonaje == 0)
            {
                animator.SetBool("Death", true);
                estaMuerto = true;
                ConvertirASolido();
                Invoke(nameof(Morir), 1f);
            }
            else
            {
                StartCoroutine(ActivarInvulnerabilidad());
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
            vidaPersonaje = 3;
            uIManager.QuitarSangre();
            golpesRecibidos = 0;
        }
        else
        {
            vidaPersonaje += cantidad;
            vidaPersonaje = Mathf.Min(vidaPersonaje, 3);

            if (vidaPersonaje == 3)
            {
                uIManager.QuitarSangre();
                golpesRecibidos = 0;
            }
        }

        if (vidaPersonaje > 1)
        {
            uIManager.DetenerMusicaCorazonBajo();
        }
    }

    public void AumentarVida(int cantidad)
    {
        if (estaMuerto) return;

        vidaPersonaje += cantidad;
        vidaPersonaje = Mathf.Min(vidaPersonaje, 3);
        uIManager.QuitarSangre();
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




}

using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float velocidadNormal = 7f;
    [SerializeField] private float velocidadR�pida = 10f;
    [SerializeField] private BoxCollider2D ColEspada;
    [SerializeField] private AudioClip sonidoHerida1;
    [SerializeField] private AudioClip sonidoHerida2;
    [SerializeField] private UIManager uIManager;

    private Rigidbody2D rig;
    private Animator anim;
    private SpriteRenderer spritePersonaje;
    private AudioSource audioSource;
    private Collider2D playerCollider;
    private int vidaPersonaje = 3;
    private int golpesRecibidos = 0;

    private float velocidadActual;
    private bool velocidadIncrementada = false;
    private float velocidadR�pidaOriginal;
    private bool invulnerable = false;
    private bool parpadeando = false;
    private bool estaMuerto = false;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        spritePersonaje = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        playerCollider = GetComponent<Collider2D>();

        if (audioSource == null)
        {
            Debug.LogError("No se encontr� el componente AudioSource en el GameObject.");
        }
        velocidadActual = velocidadNormal;
        velocidadR�pidaOriginal = velocidadR�pida;
    }

    private void Update()
    {
        if (estaMuerto) return;

        if (Input.GetKeyDown(KeyCode.J))
        {
            anim.SetTrigger("Ataca");
        }
    }

    private void FixedUpdate()
    {
        if (estaMuerto) return;

        Movimiento();
    }

    private void Movimiento()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 direccionMovimiento = new Vector2(horizontal, vertical).normalized;

        bool isMoving = direccionMovimiento.magnitude > 0;
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // Actualiza la velocidad dependiendo de si est� corriendo o no
        velocidadActual = isRunning ? velocidadR�pida : velocidadNormal;

        // Establece la velocidad del Rigidbody solo si el personaje se est� moviendo
        rig.velocity = direccionMovimiento * velocidadActual;

        if (isMoving)
        {
            anim.SetFloat("Camina", rig.velocity.magnitude);

            // Activa o desactiva la animaci�n de correr inmediatamente
            anim.SetBool("Corriendo", isRunning);
        }
        else
        {
            // Det�n cualquier movimiento residual
            rig.velocity = Vector2.zero;
            anim.SetFloat("Camina", 0);
            anim.SetBool("Corriendo", false);
        }

        if (horizontal > 0)
        {
            ColEspada.offset = new Vector2(1, 0);
            spritePersonaje.flipX = false;
        }
        else if (horizontal < 0)
        {
            ColEspada.offset = new Vector2(-1, 0);
            spritePersonaje.flipX = true;
        }
        else if (vertical > 0)
        {
            ColEspada.offset = new Vector2(1, 0);
        }
        else if (vertical < 0)
        {
            ColEspada.offset = new Vector2(-1, 0);
        }
    }



    public void CausarHerida()
    {
        if (vidaPersonaje > 0 && !invulnerable)
        {
            vidaPersonaje--;
            golpesRecibidos++;
            uIManager.RecibirDa�o(golpesRecibidos);

            if (audioSource != null && (sonidoHerida1 != null || sonidoHerida2 != null))
            {
                AudioClip sonidoSeleccionado = Random.value > 0.5f ? sonidoHerida1 : sonidoHerida2;
                audioSource.PlayOneShot(sonidoSeleccionado);
            }
            else
            {
                Debug.LogWarning("AudioSource o clips de sonido no est�n configurados correctamente.");
            }

            if (vidaPersonaje == 0)
            {
                anim.SetTrigger("Muere");
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

    private IEnumerator ActivarInvulnerabilidad()
    {
        invulnerable = true;
        StartCoroutine(Parpadear());

        yield return new WaitForSeconds(1f);

        invulnerable = false;
        spritePersonaje.color = Color.white;
        parpadeando = false;
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
            velocidadR�pida += velocidadR�pidaOriginal * porcentaje;
            velocidadIncrementada = true;
            StartCoroutine(ReiniciarVelocidad());
        }
    }

    private IEnumerator ReiniciarVelocidad()
    {
        yield return new WaitForSeconds(10f);
        velocidadNormal = 7f;
        velocidadR�pida = 10f;
        velocidadIncrementada = false;
    }

}

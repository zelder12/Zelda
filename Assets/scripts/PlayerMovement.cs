using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float velocidadNormal = 7f;
    [SerializeField] private float velocidadRápida = 10f;
    [SerializeField] private BoxCollider2D ColEspada;
    [SerializeField] private AudioClip sonidoHerida1;
    [SerializeField] private AudioClip sonidoHerida2;
    [SerializeField] private UIManager uIManager;

    private Rigidbody2D rig;
    private Animator anim;
    private SpriteRenderer spritePersonaje;
    private AudioSource audioSource;
    private int vidaPersonaje = 3;

    private float velocidadActual;
    private bool velocidadIncrementada = false;
    private float velocidadRápidaOriginal;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        spritePersonaje = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("No se encontró el componente AudioSource en el GameObject.");
        }
        velocidadActual = velocidadNormal;
        velocidadRápidaOriginal = velocidadRápida;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            anim.SetTrigger("Ataca");
        }
    }

    private void FixedUpdate()
    {
        Movimiento();
    }

    private void Movimiento()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 direccionMovimiento = new Vector2(horizontal, vertical).normalized;

        bool isMoving = direccionMovimiento.magnitude > 0;

        velocidadActual = Input.GetKey(KeyCode.LeftShift) ? velocidadRápida : velocidadNormal;

        rig.velocity = direccionMovimiento * velocidadActual;

        if (isMoving)
        {
            anim.SetFloat("Camina", rig.velocity.magnitude);
            anim.SetBool("Corriendo", Input.GetKey(KeyCode.LeftShift));
        }
        else
        {
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
        if (vidaPersonaje > 0)
        {
            vidaPersonaje--;
            uIManager.ActualizaCorazones(vidaPersonaje);

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
                anim.SetTrigger("Muere");
                Invoke(nameof(Morir), 1f);
            }
        }
    }

    private void Morir()
    {
        if (uIManager != null)
        {
            uIManager.IniciarGameOver();
        }

        Destroy(this.gameObject);
    }

    public void RegenerarCorazon()
    {
        if (vidaPersonaje < 3)
        {
            vidaPersonaje++;
            uIManager.ActualizaCorazones(vidaPersonaje);
        }
    }

    public void AumentarVida(int cantidad)
    {
        vidaPersonaje += cantidad;
        vidaPersonaje = Mathf.Min(vidaPersonaje, 3);
        uIManager.ActualizaCorazones(vidaPersonaje);
    }

    public void AumentarVelocidad(float porcentaje)
    {
        Debug.Log("AumentarVelocidad llamado con porcentaje: " + porcentaje);
        if (!velocidadIncrementada)
        {
            velocidadNormal += velocidadNormal * porcentaje;
            velocidadRápida += velocidadRápidaOriginal * porcentaje;
            velocidadIncrementada = true;
            Debug.Log("Velocidad incrementada: Velocidad Normal = " + velocidadNormal + ", Velocidad Rápida = " + velocidadRápida);
            StartCoroutine(ReiniciarVelocidad());
        }
    }

    private IEnumerator ReiniciarVelocidad()
    {
        yield return new WaitForSeconds(10f);
        velocidadNormal = 7f;
        velocidadRápida = 13f;
        velocidadIncrementada = false;
        Debug.Log("Velocidad reiniciada: Velocidad Normal = " + velocidadNormal + ", Velocidad Rápida = " + velocidadRápida);
    }
}

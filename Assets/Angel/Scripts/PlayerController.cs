using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float Carrera = 10f;
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private Animator animator;
    public SpriteRenderer spriteRenderer;
    // Referencias a los SpriteRenderers de las partes del cuerpo (si aplica)
    // public SpriteRenderer leftHandRenderer;
    // public SpriteRenderer rightHandRenderer;
    // public SpriteRenderer leftWeaponRenderer;
    // public SpriteRenderer rightWeaponRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Inicializar referencias adicionales si las tienes
    }
    // Método llamado cuando la animación de muerte termine


    void Update()
    {
        // Capturar la entrada del jugador
        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.y = Input.GetAxisRaw("Vertical");

        // Calcular la velocidad para las animaciones
        float speed = moveDirection.sqrMagnitude;
        animator.SetFloat("Speed", speed);

        // Cambiar la dirección del sprite
        if (moveDirection.x > 0)
        {
            FlipSprites(false);
        }
        else if (moveDirection.x < 0)
        {
            FlipSprites(true);
        }

        // Ejemplo de condición para morir (puedes reemplazar esto con tu propia lógica)
        if (Input.GetKeyDown(KeyCode.K)) // Presiona K para simular la muerte
        {

                animator.enabled = true; // Habilita el Animator
                animator.SetBool("Death",true); // Inicia la animación de muerte
                animator.SetBool("Death", true); // Inicia la animación de muerte
            StartCoroutine(PauseDeathAnimation(animator));

                // Corutina que pausa la animación al llegar al último frame
                IEnumerator PauseDeathAnimation(Animator animator)
                {
                    // Espera hasta que la animación haya terminado
                    while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                    {
                        yield return null; // Espera hasta el siguiente frame
                    }
                animator.SetBool("Death", false); // Inicia la animación de muerte
                // Una vez que la animación terminó, pausamos
                animator.speed = 0; // Detiene la animación en el último frame
                }
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            animator.SetBool("IsRun", true);
            moveSpeed = moveSpeed + Carrera;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            animator.SetBool("IsRun", false);
            moveSpeed = moveSpeed - Carrera;
        }
    }

    void FixedUpdate()
    {
        // Mover al jugador
        rb.MovePosition(rb.position + moveDirection.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    void FlipSprites(bool flip)
    {
        spriteRenderer.flipX = flip;
        // Si tienes múltiples SpriteRenderers para las partes del cuerpo, invierte también sus sprites
        /*
        leftHandRenderer.flipX = flip;
        rightHandRenderer.flipX = !flip;
        leftWeaponRenderer.flipX = flip;
        rightWeaponRenderer.flipX = !flip;
        */
    }
}

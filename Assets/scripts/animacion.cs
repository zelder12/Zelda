using System.Collections;
using UnityEngine;

public class Rupia : MonoBehaviour
{
    public delegate void SumaRupia(int rupia);
    public static SumaRupia sumaRupia;

    [SerializeField] private int cantidadRupias;
    [SerializeField] private AudioClip sonidoRecoleccion;
    private AudioSource audioSource;
    private Animator animator;
    private bool isAnimating = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        if (audioSource == null)
        {
            Debug.LogError("No se encontró el componente AudioSource en el GameObject.");
        }
        if (animator == null)
        {
            Debug.LogError("No se encontró el componente Animator en el GameObject.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (sumaRupia != null)
            {
                SumarRupia();
                if (!isAnimating)
                {
                    StartCoroutine(AnimarRupia());
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }

    private void SumarRupia()
    {
        sumaRupia?.Invoke(cantidadRupias);
        if (audioSource != null && sonidoRecoleccion != null)
        {
            audioSource.PlayOneShot(sonidoRecoleccion, 1.0f);
        }
        else
        {
            Debug.LogWarning("AudioSource o AudioClip son null.");
        }
    }

    private IEnumerator AnimarRupia()
    {
        isAnimating = true;
        animator.SetTrigger("Recolectar"); 
        yield return new WaitForSeconds(1f); 
        Destroy(this.gameObject);
    }
}

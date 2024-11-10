using UnityEngine;
using static Enemigo;

public class Weapon : MonoBehaviour
{
    public GameObject hand;
    public Belicas weaponItem;
    private GameObject objetoColisionado;
    public bool facingRight;

    public void SetHand(GameObject setHand)
    {
        this.hand = setHand;
    }

    private void Update()
    {
        GameManager gameManager = new GameManager();
        if (hand != null && weaponItem != null)  // Verificar que weaponItem no sea null
        {
            if (!weaponItem.estadoAtacando)
            {
                if (Input.GetKeyDown(gameManager.keyMappings["Attack"]))
                {
                    StartCoroutine(weaponItem.AnimarAtaque(hand, facingRight));
                    Debug.Log("ATAQUE REALIZADO");
                }
                else if (Input.GetKeyDown(gameManager.keyMappings["Jump"]))
                {
                    Debug.Log("SALTO REALIZADO");
                }
            }
        }
    }


    private void Start()
    {
        if (weaponItem != null)
        {
            GetComponent<SpriteRenderer>().sprite = weaponItem.sprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        objetoColisionado = collision.gameObject;
        Debug.Log("Colisionaste con: " + objetoColisionado.name);
        // Eliminamos la parte que hace daño al enemigo
        // if (objetoColisionado.GetComponent<EnemyMovement>() != null)
        // {
        //     objetoColisionado.GetComponent<EnemyMovement>().datos.dañoJugador(weaponItem.dcc, weaponItem.dpc, weaponItem.dmc);
        // }
    }

    public void Usar()
    {
    }

    public void Equipar()
    {
    }
}

// Assets/Scripts/Inventory/Weapon.cs
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
        if (hand != null)
        {
            if (!weaponItem.estadoAtacando) {

                if (Input.GetKeyDown(KeyCode.J))
                {
                    StartCoroutine(weaponItem.AnimarAtaque(hand, facingRight));
                    Debug.Log("ATAQUE REALIZADO");
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
        if (objetoColisionado.GetComponent<EnemyMovement>() != null)
        {
            objetoColisionado.GetComponent<EnemyMovement>().datos.dañoJugador(weaponItem.dcc, weaponItem.dpc, weaponItem.dmc);
        }
    }
    public void Usar()
    {
    }
    public void Equipar()
    {
    }


}

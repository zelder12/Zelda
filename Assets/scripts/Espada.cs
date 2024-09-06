using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private BoxCollider2D ColEspada;

    private void Awake()
    {
        ColEspada = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Orco"))
        {
            Destroy(otro.gameObject);
        }
    }
}

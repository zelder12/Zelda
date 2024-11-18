using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIAdmin : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject player;
    public GameObject AngelPanel;
        public GameObject JugadorPanel;
            public GameObject BarraVidaSlider;
        public GameObject AdminPanel;
            public GameObject IzquierdaText;
            public GameObject DerechaText;
    // Start is called before the first frame update
    void Start()
    {
        if (gameManager != null)
        {
            SetMaxHealth(gameManager.GetStat("Salud"));
        }

    }
    public void SetMaxHealth(float maxHealth)
    {
        BarraVidaSlider.gameObject.GetComponent<Slider>().maxValue = maxHealth;
        BarraVidaSlider.gameObject.GetComponent<Slider>().value = maxHealth;
    }

    public void SetHealth(float currentHealth)
    {
        BarraVidaSlider.gameObject.GetComponent<Slider>().value = currentHealth;
    }
    // Update is called once per frame
    void Update()
    {
        if (gameManager == null || player == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            player = GameObject.FindWithTag("Player");
            //Debug.Log("ENTRO AL QUE ES AMBOS NULOS Y TERMINO CON MANAGER: " + gameManager.name + " Y PLAYER: " + player.name);
        }
        else if (gameManager != null && player != null)
        {
            IzquierdaText.GetComponent<TextMeshProUGUI>().text = AtualizarTextoIzquierdo();
            DerechaText.GetComponent<TextMeshProUGUI>().text = AtualizarTextoDerecho();
            Debug.Log("ENTRO AL QUE ES AMBOS ASGINADOS Y TERMINO CON TEXTO IZQUIERDO: " + AtualizarTextoIzquierdo() + " Y DERECHO: " + AtualizarTextoDerecho());
            SetHealth(player.GetComponent<PlayerMovement>().vidaActual);

        }
        else
        {
            Debug.Log("NO ENTRO A NINGUN IF");
        }

    }

    public string AtualizarTextoIzquierdo()
    {
        string textoIzquierda;

        textoIzquierda = "ESTDISTICAS MAXIMAS DE EL JUGADOR - NIVEL: " + gameManager.GetStat("Maestria") +
            $"\nSALUD: {gameManager.GetStat("Salud")}" +
            $"\nESTAMINA: {gameManager.GetStat("Estamina")}" +
            $"\nSACIEDAD: {gameManager.GetStat("Saciedad")}" +
            $"\nCARRERA: {gameManager.GetStat("Carrera")}" +
            $"\nFUERZA: {gameManager.GetStat("Fuerza")}" +
            $"\nESENCIA: {gameManager.GetStat("Esencia")}" +
            $"\nARMADURA: {gameManager.GetStat("Armadura")}" +
            $"\nIMPETUD: {gameManager.GetStat("Impetud")}" +

            "\n\nESTADISTICAS ACTUALES"+
            $"\nSALUD: {player.GetComponent<PlayerMovement>().vidaActual}" +
            $"\nESTAMINA: {player.GetComponent<PlayerMovement>().estaminaAct}" +
            $"\nESCENCIA: {player.GetComponent<PlayerMovement>().escenciaAct}" +
            $"\nENERGIA: {player.GetComponent<PlayerMovement>().energiaAct}"+
            $"\nMANA: {player.GetComponent<PlayerMovement>().manaActual}" +

            "\nRESTABLECIMIENTO DE ESTAMINAS" +
            $"\nESTAMINA: {player.GetComponent<PlayerMovement>().restaurarEstamina}" +
            $"\nESCENCIA: {player.GetComponent<PlayerMovement>().restaurarEscencia}" +
            $"\nENERGIA: {player.GetComponent<PlayerMovement>().restaurarEnergia}"


;

        return textoIzquierda;
    }
    public string AtualizarTextoDerecho()
    {
        string text1,text2;
        if (player.GetComponent<PlayerMovement>().leftHand.transform.childCount > 1)
        {
            text1 = "USANDO";
        }
        else
        {
            text1 = "NO USANDO";
        }

        if (player.GetComponent<PlayerMovement>().rightHand.transform.childCount > 1)
        {
            text2 = "USANDO";
        }
        else
        {
            text2 = "NO USANDO";
        }
        string textoIzquierda;
        textoIzquierda = "VALORES JUGADOR" +
            $"VELOCIDAD: {player.GetComponent<PlayerMovement>().movimiento}" +
            $"\nVISTA: {player.GetComponent<PlayerMovement>().facingRight}" +
            $"\nESTADO: {player.GetComponent<PlayerMovement>().estadoAtacando}" +
            $"\nESTADO MANO IZQUIERDA: {text1}" +
            $"\nESTADO MANO DERECHA: {text2}";

        return textoIzquierda;
    }
}

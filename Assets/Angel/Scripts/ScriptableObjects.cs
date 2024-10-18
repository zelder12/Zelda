using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScriptableObjects : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }


}
[CreateAssetMenu(fileName = "Enemigo", menuName = "Characters/Enemigo")]
public class Enemigo : ScriptableObject
{
    public struct atributos
    {
    }
    public float vida;
    public float armadura;

    public void dañoJugador(float dcc, float dcp, float dmc)
    {
        float daño = (dcc + dcp + dmc);
        vida = vida - daño;
        Debug.Log("RECIBIO DAÑO Y FUE: " + daño + "AHORA TIENE DE VIDA:" + vida);
    }
}

[CreateAssetMenu(fileName = "Humano", menuName = "Characters/Humano")]
public class Character : ScriptableObject
{

    public string characterName;


    public Sprite leftHand;
    public Sprite rightHand;
    public Sprite leftHandUsing;
    public Sprite rightHandUsing;

    [Header("ATRIBUTOS")]
    [Header("RESILIENCIA")]
    //RESILENCIA
    public float Salud;
    public float Estamina;
    public float Saciedad;
    public float Carrera;

    [Header("BELICISMO")]
    //BELICISMO
    public float Fuerza;
    public float Esencia;
    public float Armadura;
    public float Ímpetu;

    [Header("BALISTICA")]
    //BALISTICA
    public float Perforación;
    public float Energía;
    public float Coraza;
    public float Ritmo;

    [Header("ARCANA")]
    //ARCANA
    public float Poder;
    public float Mana;
    public float Cordura;
    public float Claridad;

    [Header("OTROS")]
    public float Carisma;
    public float Inteligencia;
    public float Suerte;
    public float Valor;


}

//ITEM
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    string nombre;
    public Sprite sprite;
    public enum Tipo
    {
        Armas,
        Armaduras,
        Consumibles,
        Manuscrito,
        Herramientas,
        Miscelaneos
    };
    [SerializeField] public Tipo objetosEquipo;
    public Tipo GetObjetosEquipo()
    {
        return objetosEquipo;
    }
    public string Descripcion;
    public float Peso, Valor, Rareza, Durabilidad;
}

//TIPOS
[CreateAssetMenu(fileName = "NewArmas", menuName = "Inventory/Item/Armas")]
public class Armas : Item
{
    public float dcc, dpc, dmc, complejidad, limiteEfectos, limiteEncantamientos, ejecucion, cEstamina, cEsencia, cEnergia, cMana, alcance; 
    public enum Tipo
    {
        Belicas,
        Balistica,
        Arcanas
    };
}

//ARMAS
[CreateAssetMenu(fileName = "NewBelicas", menuName = "Inventory/Item/Arma/Belicas")]
public class Belicas : Armas
{

    public bool estadoAtacando = false;
    public bool estadoUsando = false;
    public int masas;
    public float cadencia;

    public IEnumerator AnimarAtaque(GameObject espada, bool facingRight)
    {
        estadoAtacando = true;
        Vector3 originalPosition = new Vector3(0, 0, 0);
        Quaternion originalRotation = Quaternion.Euler(0, 0, 0);

        // Movimiento hacia adelante
        espada.transform.position += new Vector3(1, 0, 0);

        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < ejecucion)
        {
            // Calcula el tiempo normalizado (0 a 1)
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / ejecucion;
            float r = 90;
            // Gira la espada
            if (facingRight)
            {
                r = -90;
            }
            else
            {
                r = 90;
            }
            espada.transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, r, t)); // Rota 45 grados en Z
            yield return null; // Espera un frame
        }

        while (tiempoTranscurrido < ejecucion)
        {
            // Calcula el tiempo normalizado (0 a 1)
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / ejecucion;

            // Gira la espada
            espada.transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 90, t)); // Rota 45 grados en Z

            yield return null; // Espera un frame
        }

        // Asegúrate de que la espada termine en la rotación final
        espada.transform.rotation = Quaternion.Euler(0, 0, 45);

        // Regresar la espada a la posición original
        espada.transform.position = originalPosition;

        // Regresar a la rotación original
        espada.transform.rotation = originalRotation;
        estadoAtacando = false;
    }
}

[CreateAssetMenu(fileName = "NewBalisticas", menuName = "Inventory/Item/Arma/Balisticas")]
public class Balisticas : Armas
{
    float precision, velocidad, alcance, cadencia;
}
[CreateAssetMenu(fileName = "NewArcanas", menuName = "Inventory/Item/Arma/Arcanas")]
public class Arcanas : Armas
{
    public float duracion, cadencia;
    public int masas;
}
//________________________________________________________________________________

[CreateAssetMenu(fileName = "NewArmaduras", menuName = "Inventory/Armaduras")]
public class Armaduras : Item
{
    public float rcc, rcp, rmc;
}
[CreateAssetMenu(fileName = "NewConsumibles", menuName = "Inventory/Consumibles")]
public class Consumibles : Item
{

}
[CreateAssetMenu(fileName = "NewArmas", menuName = "Inventory/Manuscritos")]
public class Manuscritos : Item
{

}
[CreateAssetMenu(fileName = "NewHerramientas", menuName = "Inventory/Herramientas")]
public class Herramientas : Item
{

}

[CreateAssetMenu(fileName = "NewMisceláneos", menuName = "Inventory/Misceláneos")]
public class Misceláneos : Item
{

}









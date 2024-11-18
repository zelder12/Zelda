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

    public void da�oJugador(float dcc, float dcp, float dmc)
    {
        float da�o = (dcc + dcp + dmc);
        vida = vida - da�o;
        Debug.Log("RECIBIO DA�O Y FUE: " + da�o + "AHORA TIENE DE VIDA:" + vida);
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
    public float �mpetu;

    [Header("BALISTICA")]
    //BALISTICA
    public float Perforaci�n;
    public float Energ�a;
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

    public IEnumerator AnimarAtaqueJ(GameObject espada, bool facingRight)
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

        // Aseg�rate de que la espada termine en la rotaci�n final
        espada.transform.rotation = Quaternion.Euler(0, 0, 45);

        // Regresar la espada a la posici�n original
        espada.transform.position = originalPosition;

        // Regresar a la rotaci�n original
        espada.transform.rotation = originalRotation;
        estadoAtacando = false;
    }

    public IEnumerator AnimarAtaqueL(GameObject espada, Transform mano, bool facingRight)
    {
        estadoAtacando = true;
        Vector3 originalPosition = mano.transform.localPosition;
        Quaternion originalRotation = mano.transform.localRotation;

        // Asegurarse de que la espada est� en posici�n horizontal antes de la estocada
        mano.transform.localRotation = Quaternion.Euler(0, 0, -90);

        float tiempoTranscurrido = 0f;
        float avanceDistancia = 0.5f; // Distancia de la estocada
        float ejecucion = 0.2f; // Duraci�n de la animaci�n

        // Movimiento de estocada hacia adelante
        while (tiempoTranscurrido < ejecucion)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / ejecucion;

            mano.transform.localPosition = originalPosition + new Vector3(1 * avanceDistancia * t, 0, 0);
            yield return null; // Espera un frame
        }

        // Regresar la mano a la posici�n y rotaci�n originales
        mano.transform.localPosition = originalPosition;
        mano.transform.localRotation = originalRotation;
        estadoAtacando = false;
    }

    public IEnumerator AnimarAtaqueI(GameObject mano, bool facingRight)
    {
        estadoAtacando = true;
        Vector3 originalPosition = mano.transform.localPosition;
        Quaternion originalRotation = mano.transform.localRotation;

        // Posici�n de cobertura (levanta la mano con la espada)
        Vector3 targetPosition = originalPosition - new Vector3(0, 0.5f, 0); // Mueve hacia arriba
        float anguloDefensa = 45; // Rotaci�n para cubrirse

        float tiempoTranscurrido = 0f;
        float duracionMovimiento = ejecucion * 0.5f; // Reduce la duraci�n para un movimiento m�s r�pido
        float pausaCobertura = 0.8f; // Incrementa la duraci�n de la pausa en la posici�n de cobertura

        while (tiempoTranscurrido < duracionMovimiento)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / duracionMovimiento;

            // Mueve y rota la mano para cubrirse
            mano.transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, t);
            mano.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, anguloDefensa, t));

            yield return null;
        }

        // Mantiene la posici�n de cobertura
        mano.transform.localPosition = targetPosition;
        mano.transform.localRotation = Quaternion.Euler(0, 0, anguloDefensa);

        // Pausa m�s larga en la posici�n de cobertura
        yield return new WaitForSeconds(pausaCobertura);

        // Regresa a la posici�n y rotaci�n originales
        mano.transform.localPosition = originalPosition;
        mano.transform.localRotation = originalRotation;
        estadoAtacando = false;
    }


    public IEnumerator AnimarAtaqueK(GameObject mano, bool facingRight)
    {
        estadoAtacando = true;
        Vector3 originalPosition = mano.transform.localPosition;
        Quaternion originalRotation = mano.transform.localRotation;

        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < ejecucion)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / ejecucion;

            // Rota la mano en un c�rculo completo (360 grados)
            float angulo = -360;
            mano.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, angulo, t));

            // Mant�n la mano en la posici�n original
            mano.transform.localPosition = originalPosition;

            yield return null;
        }

        // Asegura que la rotaci�n y posici�n regresen a sus valores originales
        mano.transform.localPosition = originalPosition;
        mano.transform.localRotation = originalRotation;
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

[CreateAssetMenu(fileName = "NewMiscel�neos", menuName = "Inventory/Miscel�neos")]
public class Miscel�neos : Item
{

}









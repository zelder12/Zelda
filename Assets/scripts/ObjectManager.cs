/*using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    private Dictionary<Objetos.ObjetosEquipo, List<Objetos>> objetosEquipoMap = new Dictionary<Objetos.ObjetosEquipo, List<Objetos>>();
    private const int LimiteTotalObjetos = 3;

    public Inventario inventario; // Aseg�rate de tener una referencia al Inventario
    public int slotIndex; // Aqu� puedes almacenar el �ndice del slot que quieres vaciar

    public void AddObjeto(Objetos objeto)
    {
        if (objeto != null)
        {
            int totalObjetos = CalcularTotalObjetos();
            Debug.Log($"Total de objetos antes de a�adir: {totalObjetos}");

            if (totalObjetos < LimiteTotalObjetos)
            {
                Objetos.ObjetosEquipo equipo = objeto.GetObjetosEquipo();

                if (!objetosEquipoMap.ContainsKey(equipo))
                {
                    objetosEquipoMap[equipo] = new List<Objetos>();
                }

                objetosEquipoMap[equipo].Add(objeto);

                Debug.Log($"Objeto del tipo {equipo} a�adido. Total en inventario: {CalcularTotalObjetos()}");
            }
            else
            {
                Debug.LogWarning("No se puede a�adir m�s objetos. L�mite total alcanzado.");
            }
        }
    }

    public void UsarObjeto(Objetos.ObjetosEquipo equipo)
    {
        if (objetosEquipoMap.TryGetValue(equipo, out List<Objetos> objetos))
        {
            if (objetos.Count > 0)
            {
                Objetos objeto = objetos[0];

                // Llamada corregida: ahora pasamos el inventario y el �ndice del slot
                objeto.UsarObjeto(inventario, slotIndex);

                objetos.RemoveAt(0);

                if (objetos.Count == 0)
                {
                    objetosEquipoMap.Remove(equipo);
                }

                Debug.Log($"Objeto del tipo {equipo} usado. Quedan: {objetos.Count}. Total en inventario: {CalcularTotalObjetos()}");
            }
            else
            {
                Debug.LogWarning($"No hay objetos disponibles del tipo {equipo}");
            }
        }
        else
        {
            Debug.LogWarning($"No se encontr� el tipo de objeto {equipo} en el inventario");
        }
    }

    public int CalcularTotalObjetos()
    {
        int count = 0;
        foreach (var kvp in objetosEquipoMap)
        {
            count += kvp.Value.Count;
        }
        return count;
    }
}*/

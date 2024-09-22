using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public GameObject[] slots;
    Text text;
    private int num_slots_max = 9;

    // Start is called before the first frame update
    void Start()
    {
        slots = new GameObject[num_slots_max];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setSlot(GameObject slot, int pos, int cant)
    {
        bool exist = false;

        // Recorre los slots buscando si ya existe el item
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].tag == slot.tag)
            {
                var attributesController = slots[i].GetComponent<AttributesController>();
                if (attributesController != null)
                {
                    int already_cant = attributesController.getCantidad();
                    attributesController.setCantidad(already_cant + cant);
                    exist = true;
                }
                break; // Sale del bucle cuando encuentra el item
            }
        }

        // Si el item no existe en el inventario, se añade en la posición especificada
        if (!exist)
        {
            slot.GetComponent<AttributesController>().setCantidad(cant);
            this.slots[pos] = slot;
        }
    }

    public void setSlotDrag(GameObject slot, int pos, int cant)
    {
        // Asigna el slot a la posición indicada
        this.slots[pos] = slot;
    }

    public GameObject[] getSlots()
    {
        return this.slots;
    }

    public void showInventory()
    {
        // Obtiene todos los objetos hijos con tag "inventario"
        Component[] inventario = GameObject.FindGameObjectWithTag("inventario").GetComponentsInChildren<Transform>();
        int contador = 0;

        // Si se eliminan los items del inventario actual, muestra los nuevos
        if (removeItems(inventario))
        {
            for (int i = 0; i < inventario.Length; i++)
            {
                if (inventario[i].tag == "slot")
                {
                    if (slots[contador] != null)
                    {
                        GameObject child = inventario[i].gameObject;
                        GameObject item = Instantiate(slots[contador], child.transform.position, Quaternion.identity);
                        item.transform.SetParent(child.transform, false);
                        item.transform.localPosition = new Vector3(0, 0, 0);
                        item.name = item.name.Replace("Clone", "").Replace("()", "");
                    }
                    contador++;
                }
            }
        }
    }

    public bool removeItems(Component[] inventario)
    {
        // Recorre todos los slots del inventario y elimina los hijos
        for (int e = 0; e < inventario.Length; e++)
        {
            GameObject child = inventario[e].gameObject;
            if (child.tag == "slot" && child.transform.childCount > 0)
            {
                // Elimina todos los hijos del slot
                for (int a = 0; a < child.transform.childCount; a++)
                {
                    Destroy(child.transform.GetChild(a).gameObject);
                }
            }
        }
        return true;
    }
}

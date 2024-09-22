using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour , IDropHandler
{
    public GameObject item;
    public void OnDrop(PointerEventData eventData)
    {
        item = DragHandler.itemDragging;
        item.transform.SetParent(transform);
        item.transform.position = transform.position;
        refreshInventory();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setItem(GameObject item)
    {
        this.item = item;
    }

    public void refreshInventory()
    {
        Component[] inventario = GameObject.FindGameObjectWithTag("inventario").GetComponentsInChildren<Transform>();
        int contador = 0;
        for (int e = 0; e < inventario.Length; e++)
        {
            if (inventario[e].tag == "slot")
            {
                if (inventario[e].transform.childCount == 0)
                {
                    GameObject.FindGameObjectWithTag("general-events").GetComponent<InventoryController>().setSlotDrag(null, contador, 0);
                }
                else if (inventario[e].transform.childCount == 1)
                {
                    GameObject child = inventario[e].GetComponentInChildren<Transform>().GetChild(0).gameObject;
                    int cant = child.GetComponent<AttributesController>().getCantidad();
                    GameObject.FindGameObjectWithTag("general-events").GetComponent<InventoryController>().setSlot(child, contador, cant);
                }
                contador++;
            }
        }
    }
}

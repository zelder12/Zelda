using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventario : MonoBehaviour
{
    public List<GameObject> Bag = new List<GameObject>();
    public GameObject[] inv;
    public bool Activar_inv;

    public GameObject Selector;
    public int ID;

    public List<Image> Equipo = new List<Image>();
    public int ID_equipo;
    public int Fases_inv;

    public GameObject Opciones;
    public Image[] Seleccion;
    public Sprite[] Seleccion_Sprite;
    public int ID_Select;

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Item"))
        {
            AddObjeto(coll.gameObject); // Reutiliza el método para añadir objetos
        }
    }

    public void AddObjeto(GameObject item)
    {
        Objetos objetoRecogido = item.GetComponent<Objetos>();

        for (int i = 0; i < Bag.Count; i++)
        {
            if (!Bag[i].GetComponent<Image>().enabled)
            {
                Bag[i].GetComponent<Image>().enabled = true;
                Bag[i].GetComponent<Image>().sprite = item.GetComponent<SpriteRenderer>().sprite;

                if (objetoRecogido != null)
                {
                    Objetos objetoInventario = Bag[i].AddComponent<Objetos>();
                    objetoInventario.objetosEquipo = objetoRecogido.GetObjetosEquipo();
                }

                Destroy(item); // Elimina el objeto del escenario
                break;
            }
        }
    }

    public int CalcularTotalObjetos()
    {
        int totalObjetos = 0;

        foreach (GameObject slot in Bag)
        {
            if (slot.GetComponent<Image>().enabled)
            {
                totalObjetos++;
            }
        }

        return totalObjetos;
    }

    public void UsarObjeto(int index)
    {
        if (index >= 0 && index < Bag.Count)
        {
            Objetos objeto = Bag[index].GetComponent<Objetos>();

            if (objeto != null)
            {
                objeto.UsarObjeto(this, index); // Pasamos el inventario y el índice del slot
                VaciarSlot(index);
            }
        }
    }

    public void VaciarSlot(int index)
    {
        if (index >= 0 && index < Bag.Count)
        {
            Bag[index].GetComponent<Image>().sprite = null;
            Bag[index].GetComponent<Image>().enabled = false;

            Objetos objeto = Bag[index].GetComponent<Objetos>();
            if (objeto != null)
            {
                Destroy(objeto); // Eliminar el componente Objetos del slot
            }
        }
    }

    private void VaciarSlotEquipo(int idEquipo)
    {
        if (idEquipo >= 0 && idEquipo < Equipo.Count)
        {
            Equipo[idEquipo].sprite = null; // Eliminar la imagen del objeto
            Equipo[idEquipo].enabled = false; // Desactivar el slot para reutilizarlo más tarde

            Objetos objeto = Equipo[idEquipo].GetComponent<Objetos>();
            if (objeto != null)
            {
                Destroy(objeto); // Eliminar el componente Objetos del slot
            }

            Debug.Log($"Slot de equipo {idEquipo} vaciado.");
        }
    }



    private void DetectarTeclaEquipo()
    {
        // Recorremos los slots del equipo y verificamos si se presiona la tecla correspondiente
        for (int i = 0; i < Equipo.Count; i++)
        {
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i))) // Alpha1 es el número 1 en el teclado
            {
                UsarObjetoDelEquipo(i);
            }
        }
    }

    private void UsarObjetoDelEquipo(int idEquipo)
    {
        if (idEquipo >= 0 && idEquipo < Equipo.Count && Equipo[idEquipo].enabled)
        {
            Objetos objetoEnEquipo = Equipo[idEquipo].GetComponent<Objetos>();
            if (objetoEnEquipo != null)
            {
                // Usar el objeto
                objetoEnEquipo.UsarObjeto(this, idEquipo);
                Debug.Log($"Objeto del equipo en ID_equipo {idEquipo} utilizado.");

                // Vaciar el slot después de usar el objeto
                VaciarSlotEquipo(idEquipo);
            }
            else
            {
                Debug.LogWarning($"No hay objeto en el equipo en ID_equipo {idEquipo}.");
            }
        }
        else
        {
            Debug.LogWarning($"El slot de equipo {idEquipo} está vacío o fuera de rango.");
        }
    }



    // Ya tienes lógica para navegar y manejar el inventario
    public void Navegar()
    {
        switch (Fases_inv)
        {
            case 0:
                Selector.SetActive(true);
                Opciones.SetActive(false);

                inv[1].SetActive(false);

                if (Input.GetKeyDown(KeyCode.W) && ID_equipo > 0)
                {
                    ID_equipo--;
                }
                if (Input.GetKeyDown(KeyCode.S) && ID_equipo < Equipo.Count - 1)
                {
                    ID_equipo++;
                }

                Selector.transform.position = Equipo[ID_equipo].transform.position;

                if (Input.GetKeyDown(KeyCode.F) && Activar_inv)
                {
                    Fases_inv = 1;
                }
                break;

            case 1:
                Selector.SetActive(true);
                Opciones.SetActive(false);

                if (Input.GetKeyDown(KeyCode.F) && Bag[ID].GetComponent<Image>().enabled == true)
                {
                    Fases_inv = 2;
                }

                inv[1].SetActive(true);

                // *** Aquí verificamos que el objeto no haya sido destruido ***
                if (ID >= 0 && ID < Bag.Count && Bag[ID] != null)
                {
                    if (Input.GetKeyDown(KeyCode.D) && ID < Bag.Count - 1)
                    {
                        ID++;
                    }
                    if (Input.GetKeyDown(KeyCode.A) && ID > 0)
                    {
                        ID--;
                    }
                    if (Input.GetKeyDown(KeyCode.W) && ID > 3)
                    {
                        ID -= 4;
                    }
                    if (Input.GetKeyDown(KeyCode.S) && ID < 8)
                    {
                        ID += 4;
                    }

                    // Actualizamos la posición del selector
                    if (Bag.Count > 0 && ID >= 0 && ID < Bag.Count && Bag[ID] != null)
                    {
                        Selector.transform.position = Bag[ID].transform.position;
                    }
                }

                if (Input.GetKeyDown(KeyCode.G) && Activar_inv)
                {
                    Fases_inv = 0;
                }
                break;

            case 2:
                // Detecta si se presiona la tecla Esc para cerrar las opciones de selección
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Fases_inv = 1; // Regresar a la fase donde no hay opciones
                    Opciones.SetActive(false); // Desactiva las opciones
                    Selector.SetActive(true); // Reactiva el selector
                }

                if (Input.GetKeyDown(KeyCode.G))
                {
                    Fases_inv = 1;
                }

                Opciones.SetActive(true);
                Opciones.transform.position = Bag[ID].transform.position;
                Selector.SetActive(false);

                if (Input.GetKeyDown(KeyCode.W) && ID_Select > 0)
                {
                    ID_Select--;
                }
                if (Input.GetKeyDown(KeyCode.S) && ID_Select < Seleccion.Length - 1)
                {
                    ID_Select++;
                }

                switch (ID_Select)
                {
                    case 0: // Equipar objeto
                        Seleccion[0].sprite = Seleccion_Sprite[1];
                        Seleccion[1].sprite = Seleccion_Sprite[0];
                        Seleccion[2].sprite = Seleccion_Sprite[0];

                        if (Input.GetKeyDown(KeyCode.F))
                        {
                            if (Equipo[ID_equipo].GetComponent<Image>().enabled == false)
                            {
                                Equipo[ID_equipo].GetComponent<Image>().sprite = Bag[ID].GetComponent<Image>().sprite;
                                Equipo[ID_equipo].GetComponent<Image>().enabled = true;

                                // Copiar el componente Objetos al slot del equipo
                                Objetos objetoEnInventario = Bag[ID].GetComponent<Objetos>();
                                if (objetoEnInventario != null)
                                {
                                    Objetos objetoEnEquipo = Equipo[ID_equipo].gameObject.AddComponent<Objetos>();
                                    objetoEnEquipo.objetosEquipo = objetoEnInventario.GetObjetosEquipo();
                                }

                                VaciarSlot(ID); // Vaciar el slot del inventario
                            }

                            else
                            {
                                Sprite obj = Bag[ID].GetComponent<Image>().sprite;
                                Bag[ID].GetComponent<Image>().sprite = Equipo[ID_equipo].GetComponent<Image>().sprite;
                                Equipo[ID_equipo].GetComponent<Image>().sprite = obj;
                            }

                            Fases_inv = 0;
                        }
                        break;

                    case 1: // Usar objeto
                        Seleccion[0].sprite = Seleccion_Sprite[0];
                        Seleccion[1].sprite = Seleccion_Sprite[1];
                        Seleccion[2].sprite = Seleccion_Sprite[0];

                        if (Input.GetKeyDown(KeyCode.F))
                        {
                            // Usar el objeto
                            Objetos objetoEnUso = Bag[ID].GetComponent<Objetos>();
                            if (objetoEnUso != null)
                            {
                                objetoEnUso.UsarObjeto(this, ID); // Llamar a la función que usa el objeto
                            }

                            Fases_inv = 1;
                        }
                        break;

                    case 2: // Descartar objeto
                        Seleccion[0].sprite = Seleccion_Sprite[0];
                        Seleccion[1].sprite = Seleccion_Sprite[0];
                        Seleccion[2].sprite = Seleccion_Sprite[1]; // Resaltar la opción 3

                        if (Input.GetKeyDown(KeyCode.F))
                        {
                            // Descartar el objeto
                            Debug.Log("Objeto descartado: " + Bag[ID].GetComponent<Objetos>().objetosEquipo);

                            VaciarSlot(ID); // Vaciar slot en lugar de destruir el objeto

                            Fases_inv = 1; // Regresar a la fase anterior
                        }
                        break;
                }
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Navegar();

        if (Activar_inv)
        {
            inv[0].SetActive(true);
           
        }
        else
        {
            Fases_inv = 0;
            inv[0].SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Activar_inv = !Activar_inv;
        }

        DetectarTeclaEquipo();
    }
}

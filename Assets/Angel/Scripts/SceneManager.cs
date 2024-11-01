using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CargarEscenaSiguiente()
    {
        // Obtener el índice de la escena actual
        int indiceActual = SceneManager.GetActiveScene().buildIndex;

        // Cargar la siguiente escena
        SceneManager.LoadScene(indiceActual + 1);
    }

    public void CargarEscenaPorNombre(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }
}

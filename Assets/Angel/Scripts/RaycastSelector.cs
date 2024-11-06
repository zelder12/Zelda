using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEditor;

public class RaycastSelector : MonoBehaviour
{
    public GraphicRaycaster graphicRaycaster; // Asigna el GraphicRaycaster del Canvas
    public EventSystem eventSystem; // Asigna el EventSystem de la escena
    public string personajeSave;
    public GameObject personajeDelete;
    public GameObject SavesPanel;
    public GameManager gameManager;

    void Update()
    {
        // Detecta si el usuario hace clic izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            DetectSelectedSavePanel();
        }
        if (Input.GetMouseButtonDown(1))
        {
            BorrarPartida();
        }
    }

    private void DetectSelectedSavePanel()
    {
        GameManager persistentData = FindObjectOfType<GameManager>();
        // Configura los datos del puntero para el raycasting
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        // Realiza el raycast y almacena los resultados en una lista
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerData, results);

        // Procesa cada resultado del raycast
        foreach (RaycastResult result in results)
        {
            // Verifica si el objeto tiene la etiqueta "SavePanel"
            if (result.gameObject.CompareTag("GridCell"))
            {
                // Accede al `SavePanel` y sus componentes
                GameObject selectedSavePanel = result.gameObject;

                // Acceder a la imagen y al texto dentro del `SavePanel`
                UnityEngine.UI.Image saveImage = selectedSavePanel.transform.Find("Image").gameObject.GetComponent<UnityEngine.UI.Image>();
                TextMeshProUGUI saveText = selectedSavePanel.GetComponentInChildren<TextMeshProUGUI>();

                // Muestra información de la celda seleccionada en la consola
                Debug.Log("Panel de partida guardada seleccionada: " + selectedSavePanel.name);
                Debug.Log("Texto de la partida: " + saveText.text);
                Debug.Log("Nombre del sprite de la imagen: " + saveImage.sprite.name);
                persistentData.PlayerRol = saveImage.sprite.name;
                LoadSceneAsync("Zelda 1");
                // Aquí puedes agregar lógica adicional para cargar la partida o mostrar detalles

                break; // Detiene el bucle al encontrar el primer `SavePanel` seleccionado
            }
            else
            {
                Debug.Log("Objeto Equivocado");
            }
        }
    }
    GameObject selectedSavePanelD;
    public void BorrarPartida()
    {
        GameManager persistentData = FindObjectOfType<GameManager>();
        // Configura los datos del puntero para el raycasting
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        // Realiza el raycast y almacena los resultados en una lista
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerData, results);

        // Procesa cada resultado del raycast
        foreach (RaycastResult result in results)
        {
            // Verifica si el objeto tiene la etiqueta "SavePanel"
            if (result.gameObject.CompareTag("GridCell"))
            {
                // Accede al `SavePanel` y sus componentes
                selectedSavePanelD = result.gameObject;

                // Acceder a la imagen y al texto dentro del `SavePanel`
                UnityEngine.UI.Image saveImage = selectedSavePanelD.transform.Find("Image").gameObject.GetComponent<UnityEngine.UI.Image>();
                TextMeshProUGUI saveText = selectedSavePanelD.GetComponentInChildren<TextMeshProUGUI>();

                // Muestra información de la celda seleccionada en la consola
                Debug.Log("Panel de partida seleccionada: " + selectedSavePanelD.name);
                Debug.Log("Texto de la partida: " + saveText.text);
                Debug.Log("Nombre del sprite de la imagen: " + saveImage.sprite.name);
                SavesPanel.SetActive(false);
                personajeDelete.SetActive(true);
                break; // Detiene el bucle al encontrar el primer `SavePanel` seleccionado
            }
            else
            {
                Debug.Log("Objeto Equivocado");
            }
        }   
    }
    public void EliminarPartida()
    {
        SaveManager saveManager = new SaveManager();
        Debug.Log("Nombre del usuario: " + gameManager.playerUser + " Nombre de la partida: " + selectedSavePanelD.name);
        saveManager.BorrarPartida(gameManager.playerUser,selectedSavePanelD.name);
        Destroy(selectedSavePanelD);
        SavesPanel.SetActive(true);
        personajeDelete.SetActive(false);
    }

    public GameObject loadingScreen; // Panel de pantalla de carga en la UI
    public UnityEngine.UI.Slider progressBar; // Barra de progreso de la pantalla de carga

    // Método para iniciar la carga asíncrona de una escena por nombre
    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    // Corrutina para cargar la escena de forma asíncrona
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Activa la pantalla de carga
        loadingScreen.SetActive(true);

        // Inicia la carga asíncrona de la escena
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // Evita que la escena se active inmediatamente cuando termine de cargar
        operation.allowSceneActivation = false;

        // Actualiza la barra de progreso mientras se carga
        while (!operation.isDone)
        {
            // Calcula el progreso (valor entre 0 y 1)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;

            // Si la carga está completa, activa la escena cuando el progreso llegue al 90%
            if (operation.progress >= 0.9f)
            {
                // Muestra la barra completa
                progressBar.value = 1f;

                // Espera a que el usuario o la lógica permita activar la escena
                yield return new WaitForSeconds(1); // Espera adicional, opcional
                operation.allowSceneActivation = true; // Activa la escena
            }

            yield return null; // Espera un frame antes de continuar el bucle
        }

        // Desactiva la pantalla de carga después de cargar la escena
        loadingScreen.SetActive(false);
    }
}

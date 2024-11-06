using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class GridRaycastSelector : MonoBehaviour
{
    public GraphicRaycaster raycaster; // Asigna el GraphicRaycaster de la escena
    public EventSystem eventSystem; // Asigna el EventSystem de la escena

    void Update()
    {
        // Detecta cuando el usuario hace clic izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            DetectCellClicked();
        }
    }

    private void DetectCellClicked()
    {
        // Configura los datos del puntero para el raycasting
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        // Realiza el raycast y almacena los resultados en una lista
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        // Procesa cada resultado del raycast
        foreach (RaycastResult result in results)
        {
            // Verifica si el objeto tiene la etiqueta "GridCell"
            if (result.gameObject.CompareTag("GridCell"))
            {
                // Accede al panel de la celda
                GameObject cellPanel = result.gameObject;

                // Obtén la imagen y el texto dentro del panel de la celda
                Image cellImage = cellPanel.GetComponentInChildren<Image>();
                Text cellText = cellPanel.GetComponentInChildren<Text>();

                // Opcional: Muestra información de la celda seleccionada en la consola
                Debug.Log("Celda seleccionada: " + cellPanel.name);
                Debug.Log("Texto de la celda: " + cellText.text);
                Debug.Log("Imagen de la celda: " + cellImage.sprite.name);

                // Aquí puedes agregar la lógica de lo que quieras hacer con la celda seleccionada

                break; // Sal del bucle al encontrar la primera celda
            }
        }
    }
}

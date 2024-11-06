using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AsyncSceneLoader : MonoBehaviour
{
    public GameObject loadingScreen; // Panel de pantalla de carga en la UI
    public UnityEngine.UI.Slider progressBar; // Barra de progreso de la pantalla de carga

    // M�todo para iniciar la carga as�ncrona de una escena por nombre
    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    // Corrutina para cargar la escena de forma as�ncrona
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Activa la pantalla de carga
        loadingScreen.SetActive(true);

        // Inicia la carga as�ncrona de la escena
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // Evita que la escena se active inmediatamente cuando termine de cargar
        operation.allowSceneActivation = false;

        // Actualiza la barra de progreso mientras se carga
        while (!operation.isDone)
        {
            // Actualiza el progreso (valor entre 0 y 1)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;

            // Si la carga est� completa, activa la escena
            if (operation.progress >= 0.9f)
            {
                // Aqu� podr�as a�adir una pausa o mostrar un mensaje de "Toca para continuar"
                operation.allowSceneActivation = true; // Activa la escena
            }

            yield return null; // Espera un frame antes de continuar el bucle
        }
    }
}

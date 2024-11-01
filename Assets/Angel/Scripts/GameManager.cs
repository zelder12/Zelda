using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton Instance
    public static GameManager instance;

    // Variables globales
    public int score = 0;
    public int coins = 0;

    // Referencias a UI (opcional)
    public UnityEngine.UI.Text scoreText;
    public UnityEngine.UI.Text coinsText;

    void Awake()
    {
        // Implementación del patrón Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persistir entre escenas
        }
        else
        {
            Destroy(gameObject); // Asegurar que solo haya una instancia
        }
    }

    void Start()
    {
        UpdateUI();
    }

    // Método para agregar puntuación
    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Puntuación actual: " + score);
        UpdateUI();
    }

    // Método para agregar monedas
    public void AddCoins(int amount)
    {
        coins += amount;
        Debug.Log("Monedas actuales: " + coins);
        UpdateUI();
    }

    // Método para restar monedas
    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            Debug.Log("Monedas gastadas: " + amount + ". Monedas restantes: " + coins);
            UpdateUI();
            return true;
        }
        else
        {
            Debug.Log("Monedas insuficientes para gastar: " + amount);
            return false;
        }
    }

    // Actualizar la UI (si tienes referencias a textos)
    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Puntuación: " + score;

        if (coinsText != null)
            coinsText.text = "Monedas: " + coins;
    }

    // Método para reiniciar el juego
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Método para cargar una escena específica
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Método para salir del juego
    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}

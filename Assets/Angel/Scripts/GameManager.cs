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
        // Implementaci�n del patr�n Singleton
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

    // M�todo para agregar puntuaci�n
    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Puntuaci�n actual: " + score);
        UpdateUI();
    }

    // M�todo para agregar monedas
    public void AddCoins(int amount)
    {
        coins += amount;
        Debug.Log("Monedas actuales: " + coins);
        UpdateUI();
    }

    // M�todo para restar monedas
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
            scoreText.text = "Puntuaci�n: " + score;

        if (coinsText != null)
            coinsText.text = "Monedas: " + coins;
    }

    // M�todo para reiniciar el juego
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // M�todo para cargar una escena espec�fica
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // M�todo para salir del juego
    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}

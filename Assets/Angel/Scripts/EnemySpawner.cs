using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab del enemigo
    public Transform spawnPoint;   // Punto de generación
    private int elapsedMinutes = 0; // Minutos transcurridos

    private void Start()
    {
        StartCoroutine(SpawnEnemiesOverTime());
    }

    private IEnumerator SpawnEnemiesOverTime()
    {
        while (true) // Bucle infinito para generar enemigos continuamente
        {
            elapsedMinutes++;

            if (elapsedMinutes % 5 == 0) // Cada 5 minutos
            {
                SpawnMultipleEnemies(5); // Genera cinco enemigos
            }
            else
            {
                SpawnEnemy(); // Genera un solo enemigo
            }

            yield return new WaitForSeconds(60f); // Espera 1 minuto (60 segundos)
        }
    }

    private void SpawnEnemy()
    {
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        newEnemy.transform.rotation = Quaternion.Euler(0, 0, 0); // Asegura la rotación en (0, 0, 0)
        Debug.Log("Enemigo generado.");
    }

    private void SpawnMultipleEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            newEnemy.transform.rotation = Quaternion.Euler(0, 0, 0); // Asegura la rotación en (0, 0, 0)
        }
        Debug.Log($"{count} enemigos generados.");
    }
}

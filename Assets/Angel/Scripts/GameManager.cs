using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public string playerUser = "";
    public string PlayerRol;
    public string saveRun;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Inicializar listas de bonificaciones para cada estadística con tamaño 8
        foreach (var stat in baseStats.Keys)
        {
            bonuses[stat] = new List<float>(new float[8]);
        }
    }
    private void Update()
    {
        if (!string.IsNullOrEmpty(saveRun))
        {
            try
            {

                // Busca el primer archivo JSON en el directorio
                string[] jsonFiles = Directory.GetFiles(saveRun, "*.json");

                if (jsonFiles.Length > 0)
                {
                    string jsonFilePath = jsonFiles[0];

                    // Lee el contenido del archivo JSON
                    string jsonContent = File.ReadAllText(jsonFilePath);

                    // Deserializa el JSON y lo añade a la lista
                    GameSave saveData = JsonConvert.DeserializeObject<GameSave>(jsonContent);
                    Debug.Log($"Datos del archivo cargados: {saveData.gameName}");
                    playerUser = saveData.userName;
                    PlayerRol = saveData.rol;

                    SetBaseStat("Salud", saveData.salud);
                    SetBaseStat("Estamina", saveData.estamina);
                    SetBaseStat("Saciedad", saveData.saciedad);
                    SetBaseStat("Carrera", saveData.carrera);
                    SetBaseStat("Fuerza", saveData.fuerza);
                    SetBaseStat("Esencia", saveData.esencia);
                    SetBaseStat("Armadura", saveData.armadura);
                    SetBaseStat("Impetu", saveData.impetu);

                }
                else
                {
                    Debug.Log("No se encontraron archivos JSON en la carpeta " + saveRun);
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Error al leer el archivo JSON en " + saveRun + ": " + ex.Message);
            }
        }
        else
        {
            Debug.Log("NO SE A ASIGNADO NIGUN SAVE AUN");
        }
    }
    // CONTROLES
    public Dictionary<string, KeyCode> keyMappings = new Dictionary<string, KeyCode> {
        // ATAQUES
        {"Attack1", KeyCode.J},
        {"Attack2", KeyCode.K},
        {"Attack3", KeyCode.L},
        {"Attack4", KeyCode.I},

        // ACCIONES
        {"Bag", KeyCode.E},
        {"Accion1", KeyCode.F},

        // BAGS
        {"Slot1", KeyCode.Alpha1},

        // ADMIN
        {"ReducirVida", KeyCode.N},
        {"AumentarVida", KeyCode.M}

    };

    // ESTADISTICAS BASE
    public Dictionary<string, float> baseStats = new Dictionary<string, float> {
        // RESILIENCIA
        {"Salud", 100f},
        {"Estamina", 50f},
        {"Saciedad", 50f},
        {"Carrera", 10f},

        // BELICISMO
        {"Fuerza", 20f},
        {"Esencia", 30f},
        {"Armadura", 15f},
        {"Impetu", 5f},

        // BALÍSTICA
        {"Perforacion", 15f},
        {"Energia", 25f},
        {"Coraza", 10f},
        {"Ritmo", 5f},

        // ARCANA
        {"Poder", 25f},
        {"Mana", 40f},
        {"Cordura", 10f},
        {"Claridad", 5f},

        // OTROS
        {"Carisma", 5f},
        {"Inteligencia", 10f},
        {"Suerte", 5f},
        {"Valor", 5f},

        // NIVELES
        {"Maestria", 1f},
        {"Maestria Belica", 1f},
        {"Maestria Balistica", 1f},
        {"Maestria Arcana", 1f}
    };

    public void SetBaseStat(string statName, float newValue)
    {
        if (baseStats.ContainsKey(statName))
        {
            if (statName == "Maestria")
            {
                baseStats["Salud"] += 1f;
                baseStats["Fuerza"] += 0.5f;
                baseStats["Esencia"] += 0.5f;
                baseStats["Fuerza"] += 0.5f;
                baseStats["Impetu"] += 0.5f;
                baseStats["Maestria"] += newValue;

                Debug.Log($"Estadística De Nivel Aumentadas: {statName} = {newValue}");
                Debug.Log($"Nueva vida: = {baseStats["Salud"]}");
            }
            else
            {
                baseStats[statName] = newValue;
                Debug.Log($"Estadística base actualizada: {statName} = {newValue}");
            }
        }
        else
        {
            Debug.LogWarning($"La estadística {statName} no existe en las estadísticas base.");
        }
    }

    // DICCIONARIO BONIFICACIONES
    private Dictionary<string, List<float>> bonuses = new Dictionary<string, List<float>>();

    // Método para obtener el valor total de una estadística (base + bonificaciones)
    public float GetStat(string statName)
    {
        float total = baseStats.ContainsKey(statName) ? baseStats[statName] : 0f;

        if (bonuses.ContainsKey(statName))
        {
            foreach (float bonus in bonuses[statName])
            {
                total += bonus;
            }
        }

        return total;
    }

    // Método para agregar o actualizar una bonificación específica
    public void SetBonus(string statName, int sourceIndex, float bonusValue)
    {
        if (bonuses.ContainsKey(statName) && sourceIndex >= 0 && sourceIndex < 8)
        {
            bonuses[statName][sourceIndex] = bonusValue;
        }
    }

    // Método para remover una bonificación específica
    public void RemoveBonus(string statName, int sourceIndex)
    {
        if (bonuses.ContainsKey(statName) && sourceIndex >= 0 && sourceIndex < 8)
        {
            bonuses[statName][sourceIndex] = 0f;
        }
    }
}

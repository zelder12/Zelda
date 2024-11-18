using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

enum Rol
{

}

[System.Serializable]
public class UserData
{
    public string password;
    public string username;
    public string fechaCreacion;
}
public class GameSave
{
    // Información general
    public string userName;
    public string gameName;
    public string playerName;
    public string rol;
    public string fechaCreado;
    public string fechaUltimo;

    public bool mision1;
    public bool mision2;
    public bool mision3;
    public bool mision4;
    public bool mision5;
    public bool jefeFinal;

    // RESILIENCIA
    public float salud;      // Cantidad de daño que puede recibir
    public float estamina;   // Recurso para moverse
    public float saciedad;   // Resistencia al agotamiento
    public float carrera;    // Velocidad al correr

    // BELICISMO
    public float fuerza;     // Daño con armas
    public float esencia;    // Recurso para lanzar ataques
    public float armadura;   // Resistencia a daño
    public float impetu;     // Velocidad de ataque

    // BALÍSTICA
    public float perforacion; // Daño con arcos
    public float energia;     // Recurso para lanzar flechas
    public float coraza;      // Resistencia a flechas
    public float ritmo;       // Velocidad para lanzar flechas

    // ARCANA
    public float poder;      // Daño con hechizos
    public float mana;       // Recurso para lanzar hechizos
    public float cordura;    // Resistencia a hechizos
    public float claridad;   // Velocidad para lanzar hechizos

    // OTROS
    public float carisma;    // Reduce costos
    public float inteligencia; // Capacidad de aprender
    public float suerte;     // Mejor botín
    public float valor;      // Reduce el miedo

    // NIVELES
    public float maestria;             // Nivel general del personaje
    public float maestriaBelica;       // Nivel con armas cuerpo a cuerpo
    public float maestriaBalistica;    // Nivel con armas a distancia
    public float maestriaArcana;       // Nivel con magia
}

public class SaveManager : MonoBehaviour
{
    private string userFolder;
    private void Start()
    {


    }

    public string NuevoUsuario(string usuario, string password)
    {

        // Definir el camino de la carpeta del usuario
        userFolder = Path.Combine(Application.persistentDataPath, usuario);

        // Verificar si la carpeta ya existe
        if (Directory.Exists(userFolder))
        {
            return $"EL USUARIO YA EXISTE, INTENTE CON OTRO NOMBRE";
        }

        // Crear la carpeta del usuario
        Directory.CreateDirectory(userFolder);
        Debug.Log("Carpeta de usuario creada en: " + userFolder);

        string savesFolder = Path.Combine(userFolder, "Saves");
        Directory.CreateDirectory(savesFolder);
        Debug.Log("Carpeta de save creada en: " + savesFolder);

        // Crear el objeto UserData y asignar la contraseña
        UserData userData = new UserData
        {
            username = usuario,
            password = password,
            fechaCreacion = DateTime.Now.ToString()
        };

        // Convertir el objeto UserData a JSON
        string json = JsonUtility.ToJson(userData);

        // Guardar el JSON en un archivo
        string filePath = Path.Combine(userFolder, "UserData.json");
        File.WriteAllText(filePath, json);

        return $"USUARIO CREADO CON EXITO";

    }

    public void SaveData(GameSave gameSave)
    {
        string json = JsonUtility.ToJson(gameSave);
        string filePath = Path.Combine(userFolder, "playerData.json");
        File.WriteAllText(filePath, json);
        Debug.Log("Datos guardados en: " + filePath);
    }

    public string CargarUsuario(string usuario, string password)
    {
        // Definir el camino de la carpeta del usuario
        userFolder = Path.Combine(Application.persistentDataPath, usuario);

        // Verificar si la carpeta ya existe
        if (Directory.Exists(userFolder))
        {
            string filePath = Path.Combine(userFolder, "UserData.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                UserData userData = JsonUtility.FromJson<UserData>(json);
                Debug.Log("Datos cargados: " + userData.username);

                if (password == userData.password && usuario.Equals(userData.username))
                {
                    return "1";
                }
                else
                {
                    return "DATOS CORRUCTOS";
                }
            }
            else
            {
                return "DATOS CORRUCTOS";
            }
        }
        else
        {
            return "DATOS CORRUCTOS";
        }
    }

    public string CrearPartida(string usuario, string nombre, string apodo, string rol)
    {
        userFolder = Path.Combine(Application.persistentDataPath, usuario);
        string saveFolder = Path.Combine(userFolder, "Saves", nombre);

        if (Directory.Exists(saveFolder))
        {
            return $"LA PARTIDA YA EXISTE, INTENTE CON OTRO NOMBRE";
        }

        Directory.CreateDirectory(saveFolder);
        Debug.Log("Carpeta de partida creada en: " + saveFolder);

        if (rol == "Knight")
        {
            GameSave gameSave = new GameSave
            {
                userName = usuario,
                playerName = nombre,
                gameName = nombre,
                rol = rol,
                fechaCreado = DateTime.Now.ToString(),

                salud = 25,
                estamina = 10,
                saciedad = 10,
                carrera = 5,

                fuerza = 8,
                esencia = 16,
                armadura = 12,
                impetu = 0.5f,

            };

            string json = JsonUtility.ToJson(gameSave);

            string filePath = Path.Combine(saveFolder, "UserData.json");
            File.WriteAllText(filePath, json);

            return $"PARTIDA CREADA CON EXITO";
        }
        else if (rol == "Rogue")
        {
            GameSave gameSave = new GameSave
            {
                userName = usuario,
                playerName = nombre,
                gameName = nombre,
                rol = rol,
                fechaCreado = DateTime.Now.ToString(),

                salud = 5,
                estamina = 20, //carrea por el 200%
                saciedad = 10, //todos los recursos entre 4
                carrera = 10,

                fuerza = 1.6f, // salud entre 3
                esencia = 16,
                armadura = 12, //salud entre 2
                impetu = 0.5f, //fuerza entre esencia

            };

            string json = JsonUtility.ToJson(gameSave);

            string filePath = Path.Combine(saveFolder, "UserData.json");
            File.WriteAllText(filePath, json);

            return $"PARTIDA CREADA CON EXITO";
        }
        else if (rol == "Wizzard")
        {
            GameSave gameSave = new GameSave
            {
                userName = usuario,
                playerName = nombre,
                gameName = nombre,
                rol = rol,
                fechaCreado = DateTime.Now.ToString(),

                salud = 25,
                estamina = 10, //carrea por el 100%
                saciedad = 10, //todos los recursos entre 4
                carrera = 5,

                fuerza = 8, // salud entre 3
                esencia = 16,
                armadura = 12, //salud entre 2
                impetu = 0.5f, //fuerza entre esencia

            };

            string json = JsonUtility.ToJson(gameSave);

            string filePath = Path.Combine(saveFolder, "UserData.json");
            File.WriteAllText(filePath, json);

            return $"PARTIDA CREADA CON EXITO";
        }
        else
        {
            Debug.LogError("Personaje no reconocido: " + rol);
            return $"NO SE PUEDO CREAR";

        }

    }

    public void BorrarPartida(string usuario, string nombre)
    {
        userFolder = Path.Combine(Application.persistentDataPath, usuario);
        string saveFolder = Path.Combine(userFolder, "Saves", nombre);

        if (Directory.Exists(saveFolder))
        {
            Debug.Log($"LA PARTIDA YA EXISTE, SE BORRARA");
            Directory.Delete(saveFolder, true);
            Debug.Log("Carpeta de partida BORRADA en: " + saveFolder);
            Debug.Log($"LA PARTIDA SE BORRO");
        }

    }
    public int CantidadDePartidas(string usuario)
    {
        userFolder = Path.Combine(Application.persistentDataPath, usuario);
        string saveFolder = Path.Combine(userFolder, "Saves");
        DirectoryInfo di = new DirectoryInfo(saveFolder);
        DirectoryInfo[] directories = di.GetDirectories();
        Debug.Log("Número de carpetas cargadas de Saves es: " + directories.Length);
        return directories.Length;
    }

    public List<GameSave> CargarPartidas(string usuario)
    {
        List<GameSave> listaDeSaves = new List<GameSave>();
        userFolder = Path.Combine(Application.persistentDataPath, usuario);
        string saveFolder = Path.Combine(userFolder, "Saves");
        DirectoryInfo di = new DirectoryInfo(saveFolder);

        // Verifica si la carpeta de "Saves" existe
        if (!di.Exists)
        {
            Debug.Log("La carpeta de saves no existe.");
            return listaDeSaves;
        }

        DirectoryInfo[] directories = di.GetDirectories();
        Debug.Log("Número de carpetas cargadas de Saves es: " + directories.Length);

        foreach (DirectoryInfo directory in directories)
        {
            string[] jsonFiles = Directory.GetFiles(directory.FullName, "*.json");

            if (jsonFiles.Length > 0) // Verifica si hay al menos un JSON en la carpeta
            {
                string jsonFile = jsonFiles[0]; // Toma el primer JSON de la carpeta
                try
                {
                    // Lee el contenido del archivo JSON
                    string jsonContent = File.ReadAllText(jsonFile);
                    Debug.Log("Datos del archivo cargados");

                    // Deserializa el JSON y lo añade a la lista
                    GameSave saveData = JsonConvert.DeserializeObject<GameSave>(jsonContent);
                    listaDeSaves.Add(saveData);
                }
                catch (Exception ex)
                {
                    Debug.Log("Error al leer el archivo " + jsonFile + ": " + ex.Message);
                }
            }
        }

        return listaDeSaves;
    }

    public string BuscarSave(string usuario, string nombre)
    {
        userFolder = Path.Combine(Application.persistentDataPath, usuario);
        string saveFolder = Path.Combine(userFolder, "Saves", nombre);

        if (Directory.Exists(saveFolder))
        {
            return saveFolder;
        }
        return null;
    }
}

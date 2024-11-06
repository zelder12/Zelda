using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string playerUser = "";
    public string PlayerRol;
    public Dictionary<string, KeyCode> keyMappings = new Dictionary<string, KeyCode> {
        
        //ATAQUES
        {"Attack1", KeyCode.J},
        {"Attack2", KeyCode.K},
        {"Attack3", KeyCode.L},
        {"Attack4", KeyCode.I},

        //ACCIONES
        {"Bag", KeyCode.E},
        {"Accion1", KeyCode.F},

        //BAGS
        {"Slot1", KeyCode.Alpha1},

    };
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


}

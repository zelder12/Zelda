using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class ControlPaneles : MonoBehaviour
{
    public GameObject homePanel;
        public GameObject loginPanel;
        public GameObject logo;
            public GameObject inicioOpcionesPanel;
                public GameObject CrearSubPanel;
                    public GameObject UsuarioAccionPanel;
                        public TextMeshProUGUI labelAccion;
                    public TMP_InputField inputNombre;
                    public TMP_InputField inputPass;
                    public GameObject cargarBtn;
                    public GameObject crearBtn;

                public GameObject CargarSubPanel;

    public GameObject MainMenuPanel;
    public GameObject OpcionesPanel;
    public GameObject SavesPanel;
        public GameObject CrearPartidaPanel;
            public TMP_InputField NombreInput;
             public TMP_InputField ApodoInput;
            public TMP_Dropdown myDropdown;
            public UnityEngine.UI.Image PersonajeIcon;
        public GameObject BorrarPartidaPanel;
    public GameObject SavesScroll;
            public Transform Viewport;
                public Transform Content;
                
    public GameObject iniciarBtn;

    public string nombreUsuario;
    public GameObject savePanelPrefab;
    public Sprite knight, rogue, wizzard;
    public GameManager gameManager;
    public Font newFont; // Fuente para UI Text
    public TMP_FontAsset newTMPFont; // Fuente para TextMeshPro
    void Awake()
    {
        homePanel = gameObject;

        loginPanel = transform.Find("LoginPanel").gameObject;
        MainMenuPanel = transform.Find("MainMenuPanel").gameObject;
        OpcionesPanel = transform.Find("OpcionesPanel").gameObject;
        SavesPanel = transform.Find("SavesPanel").gameObject;

        inicioOpcionesPanel = loginPanel.transform.Find("InicioOpcionesPanel").gameObject;
        CrearSubPanel = inicioOpcionesPanel.transform.Find("CrearSubPanel").gameObject;
        CargarSubPanel = inicioOpcionesPanel.transform.Find("CargarSubPanel").gameObject;
        labelAccion = UsuarioAccionPanel.transform.Find("AccionTxt").gameObject.GetComponent<TextMeshProUGUI>();
        inputNombre = CrearSubPanel.transform.Find("inputNombre").gameObject.GetComponent<TMP_InputField>();
        inputPass = CrearSubPanel.transform.Find("inputPass").gameObject.GetComponent<TMP_InputField>();

        cargarBtn = CrearSubPanel.transform.Find("Cargar").gameObject;
        crearBtn = CrearSubPanel.transform.Find("Crear").gameObject;

        

        
        CrearPartidaPanel = SavesPanel.transform.Find("CrearPartidaPanel").gameObject;
        myDropdown = CrearPartidaPanel.transform.Find("RolDrop").gameObject.GetComponent<TMP_Dropdown>();

        SavesScroll = SavesPanel.transform.Find("SavesScroll").gameObject;
        NombreInput = CrearPartidaPanel.transform.Find("NombreInput").gameObject.GetComponent<TMP_InputField>();
    }

    void Start()
    {
        //MainMenuPanel.SetActive(false);
        //OpcionesPanel.SetActive(false);
        //SavesPanel.SetActive(false);

        //inicioOpcionesPanel.SetActive(false);
        //CrearSubPanel.SetActive(false);
        //CargarSubPanel.SetActive(false);
        //UsuarioAccionPanel.SetActive(false);

        myDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        Text[] uiTexts = FindObjectsOfType<Text>();
        foreach (Text uiText in uiTexts)
        {
            uiText.font = newFont;
        }

        // Cambiar fuentes para todos los TextMeshPro
        TextMeshProUGUI[] tmpTexts = FindObjectsOfType<TextMeshProUGUI>();
        foreach (TextMeshProUGUI tmpText in tmpTexts)
        {
            tmpText.font = newTMPFont;
        }

        Debug.Log("Todas las fuentes han sido cambiadas.");
    }


    public void Iniciar()
    {
        Debug.Log("Boton Iniciar Presionado");
        inicioOpcionesPanel.SetActive(true);
        logo.SetActive(false);
    }
    public void DatosPanel(bool btn)
    {
        if (btn)
        {
            CrearSubPanel.SetActive(true);
            crearBtn.SetActive(true);
            cargarBtn.SetActive(false);
            Debug.Log("Boton Crear Presionado");
        }
        else
        {
            CrearSubPanel.SetActive(true); 
            crearBtn.SetActive(false);
            cargarBtn.SetActive(true);
            Debug.Log("Boton Cargar Presionado");

        }
    }
    public void Crear()
    {
        string accion;
        SaveManager saveManager = new SaveManager();
        string nombre = inputNombre.text.ToString();
        inputNombre.text = "";
        string pass = inputPass.text.ToString();
        inputPass.text = "";
        UsuarioAccionPanel.SetActive(true);
        if (nombre == "")
        {
            accion = "DEBES TENER POR LO MENOS UN NOMBRE";
        }
        else
        {
            accion = saveManager.NuevoUsuario(nombre, pass);
        }
        Debug.Log(accion);
        labelAccion.text = accion;
        Debug.Log("Boton Crear Presionado");
        CargarPartidas();
    }

    public void Cargar()
    {
        SaveManager saveManager = new SaveManager();
        string nombre = inputNombre.text.ToString();
        inputNombre.text = "";
        string pass = inputPass.text.ToString();
        inputPass.text = "";
        UsuarioAccionPanel.SetActive(true);
        string accion = saveManager.CargarUsuario(nombre, pass);
        Debug.Log(accion);
        if (accion == "1")
        {
            labelAccion.text = "EL LIBRO TE DA LA BIENVENIDA";
            loginPanel.SetActive(false);
            MainMenuPanel.SetActive(true);
            nombreUsuario = nombre;
            gameManager.playerUser = nombre;
            Debug.Log("El usuario es: " + gameManager.playerUser);
            CargarPartidas();
        }
        else
        {
            labelAccion.text = "EL LIBRO NO TE RECONOCE";
        }
        Debug.Log("Boton Cargar Presionado");

    }

    public void Cancelar()
    {
        UsuarioAccionPanel.SetActive(false);
        CrearSubPanel.SetActive(false);
        SavesPanel.SetActive(false);
        BorrarPartidaPanel.SetActive(false);
        OpcionesPanel.SetActive(false);
    }

    public void Partidas()
    {
        SavesPanel.SetActive(true);
        SavesScroll.SetActive(true);
    }
    public void Opciones()
    { 
        OpcionesPanel.SetActive(true);
    }
    public void atras()
    { 
        SavesPanel.SetActive(false);
    }
    public void CrearPartida()
    {
        CrearPartidaPanel.SetActive(true);
        SavesScroll.SetActive(false);
        PersonajeIcon.sprite = rogue;
    }

    public void CrearSave()
    {
        string nombre = NombreInput.text;
        NombreInput.text = null;
        SaveManager saveManager = new SaveManager();
        string apodo = ApodoInput.text;
        ApodoInput.text = null;
        if (nombre == "" && apodo == "")
        {
            UsuarioAccionPanel.SetActive(true);
            labelAccion.text = "NO HAZ LLENADO CORRECTAMENTE EL CONTRATO..";
            return;
        }
        string rol = PersonajeIcon.sprite.name;
        saveManager.CrearPartida(nombreUsuario,nombre,apodo,rol);
        CargarPartidas();
    }

    public void CreaPartidaCerrar()
    {
        CrearPartidaPanel.SetActive(false);
        SavesScroll.SetActive(true);
    }

    public void CargarPartidas()
    {
        SaveManager saveManager = new SaveManager();
        //for (int i = 0; i < saveManager.CantidadDePartidas(nombreUsuario) ; i++)
        //{
        //    GameObject savePanel = Instantiate(savePanelPrefab, Content);
        //    savePanel.name = $"{saveManager.CantidadDePartidas(nombreUsuario)}";
        //}

        // Llama al método y recoge la lista de GameSave
        List<GameSave> partidasGuardadas = saveManager.CargarPartidas(nombreUsuario);
        
        int numeroDeHijos = Content.childCount;
        if (numeroDeHijos > 0)
        {
            for (int i = numeroDeHijos - 1; i >= 0; i--)
            {
                Transform hijo = Content.GetChild(i);
                Destroy(hijo.gameObject);
                
            }
            foreach (GameSave partida in partidasGuardadas)
            {
                Debug.Log("Partida cargada: " + partida.ToString());
                GameObject savePanel = Instantiate(savePanelPrefab, Content);
                string texto = $"{partida.gameName}\nUsuario: {partida.userName} - Nivel\nCreado: {partida.fechaCreado}";
                savePanel.name = $"{partida.gameName}";
                savePanel.GetComponentInChildren<TextMeshProUGUI>().text = texto;
                switch (partida.rol)
                {
                    case "Knight":
                        savePanel.transform.Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = knight;
                        Debug.Log("Personaje: Knight");
                        break;
                    case "Rogue":
                        savePanel.transform.Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = rogue;
                        Debug.Log("Personaje: Rogue");
                        break;
                    case "Wizzard":
                        savePanel.transform.Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = wizzard;
                        Debug.Log("Personaje: Wizzard");
                        break;
                    default:
                        Debug.Log("ERROR AL BUSCAR EL ROL: " + partida.rol);

                        break;
                }
            }
        }
        else 
        {
            foreach (GameSave partida in partidasGuardadas)
            {
                Debug.Log("Partida cargada: " + partida.ToString());
                GameObject savePanel = Instantiate(savePanelPrefab, Content);
                string texto = $"NOMBRE DE LA PARTIDAD: {partida.gameName}\nCREADO: {partida.fechaCreado} - ULTIMO - USUARIO {partida.userName}\nNIVEL";
                savePanel.name = $"{partida.gameName}";
                savePanel.GetComponentInChildren<TextMeshProUGUI>().text = texto;
            }
        }
    }

    public void SalirDelJuego()
    {
        Application.Quit();
    }

    private void OnDropdownValueChanged(int selectedIndex)
    {
        // Obtén el nombre de la opción seleccionada
        string selectedOption = myDropdown.options[selectedIndex].text;

        // Muestra la opción seleccionada en la consola
        Debug.Log("Opción seleccionada: " + selectedOption + " (Índice: " + selectedIndex + ")");
        switch (selectedIndex)
        {
            case 0:
                PersonajeIcon.sprite = knight;
                Debug.Log("Personaje: " + PersonajeIcon.sprite.name);
                break;
            case 1:
                PersonajeIcon.sprite = rogue;
                Debug.Log("Personaje: " + PersonajeIcon.sprite.name);
                break;
            case 2:
                PersonajeIcon.sprite = wizzard;
                Debug.Log("Personaje: " + PersonajeIcon.sprite.name);
                break;
            default:
                break;
        }
    }

    // Limpieza al destruir el objeto (buena práctica)
    private void OnDestroy()
    {
        myDropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
    }

}
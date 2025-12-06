using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    //*SINGLETON*
    //Declaramos que esto sea una variable publica y estática, que pueda acceder cualquiera (get) y que solo se pueda modificar desde la misma clase (private set)
    public static InputManager Instance { get; private set; }

    //Asset donde se almacenan los ActionMaps
    public InputActionAsset inputAsset;

    //ActionMaps que usaremos, almacenarlos en variables nos puede ayudar a cambiar entre ellos
    public InputActionMap playerActionMap;
    public InputActionMap menuActionMap;
    public InputActionMap miniGameActionMap;

    public InputActionMap currentActionMap;
    
    void Awake()
    {
        //*SINGLETON*

        //Si existe ya una instancia y no es esta, destruye el duplicado
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
        //Si no existe una instancia, esta será la única por lo tanto se asigna
            Instance = this;
        }

        //Que no se destruya la instancia al cargar una nueva escena
        DontDestroyOnLoad(gameObject);

//-----------> ASIGNACIÓN INPUT ASSET

        //La idea para acceder al asset desde todas las escenas sería lo siguiente:
        //Creamos un empty en la escena el cuál tenga este script, con la variable de InputActionAsset serializada, asignaremos desde el inspector dicho asset.
        //Este script deberá tener un singleton y tendremos que crear un prefab del empty mencionado antes (tendrá un nombre como InputManager o algo así)

        //Para prevenir errores de acceso (en el caso de que se nos olvide asignar desde el inspector el asset en alguna escena), comprobaremos que la variable este vacía.
        //Si se da el caso, lo asignaremos desde los assets del proyecto, necesitaremos acceder a AssetDatabase y con la función LoadAssetAtPath<Type>(string path); asignamos el asset
        //Necesitamos el path del archivo del asset, r.mouse click y copy path en el asset para obtenerlo

        if(inputAsset == null)
        {
            inputAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/InputSystem_Actions.inputactions");
        }

        playerActionMap = inputAsset.FindActionMap("Player");
        menuActionMap = inputAsset.FindActionMap("UI");
        miniGameActionMap = inputAsset.FindActionMap("MiniGame");
    }

    void Start()
    {
        //provisional para tener algun action map asignado desde el principio

        currentActionMap = playerActionMap;
        Debug.Log("Start --> el ActionMap Actual es:" + currentActionMap.name);
    }

    //Cambio de Action maps
    /*
    void ChangeInputMap(InputActionMap newActionMap)
    {
        if(currentActionMap != newActionMap)
        {
            inputAsset.FindActionMap(currentActionMap.name).Disable();
        }

        inputAsset.FindActionMap(newActionMap.name).Enable();
    }
    */

    public void ChangeInputMap(InputActionMap newActionMap)
    {
        if(currentActionMap == newActionMap)
        {
            return;
        }
            
        currentActionMap.Disable();
        currentActionMap = newActionMap;
        currentActionMap.Enable();
        
        Debug.Log(newActionMap.name);
    }    
}

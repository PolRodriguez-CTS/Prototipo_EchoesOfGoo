using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Components
    private CharacterController _controller;

    //Inputs
    [SerializeField] private InputActionAsset _playerInputs;
    [SerializeField] private InputActionMap _gameplayActionMap;
    [SerializeField] private InputActionMap _menuActionMap;
    [SerializeField] private InputActionMap _miniGameActionMap;

    private InputAction _moveAction;
    private InputAction _jumpAction;

    private Vector3 _moveInput;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();

        /*_moveAction = _gameplayActionMap.FindAction("Move");
        _jumpAction = _gameplayActionMap.FindAction("Jump");*/

        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
    }
    
    void Start() { }

    void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector3>();
        Debug.Log(_moveInput);
        
        if (_jumpAction.WasPressedThisFrame())
        {
            Debug.Log("Jump");
        }
    }

    void FixedUpdate() { }
}

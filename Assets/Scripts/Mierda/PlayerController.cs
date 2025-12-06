using System;
//using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEditor.Experimental.GraphView;

public class PlayerController : MonoBehaviour
{
    //Componentes
    private CharacterController _controller;
    private Animator _animator;

    //Inputs
    private InputAction _moveAction;
    private Vector2 _moveInput;
    private InputAction _jumpAction;
    //private InputAction _lookAction;
    //private Vector2 _lookInput;
    private InputAction _dashAction;

    private InputAction _clickAction;

    [SerializeField] private float _movementSpeed = 5;
    [SerializeField] private float _jumpHeight = 2;
    [SerializeField] private float _smoothTime = 0.2f;
    private float _turnSmoothVelocity;

    //Gravedad
    [SerializeField] private float _gravity = -12f;
    [SerializeField] private Vector3 _playerGravity;

    //Ground Sensor
    [SerializeField] private Transform _sensor;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] float _sensorRadius;

    private Transform _mainCamera;

    //--> Dash
    public float _dashSpeed = 30;
    public float _dashTime = 0.25f;
    private Vector3 _lastMoveDirection;
    private bool isDashing = false;


    public float targetAngle;

    public float jumpTimeOut = 0.5f;
    public float fallTImeOut = 0.15f;
    float _jumpTimeOutDelta;
    float _fallTimeOutDelta;


    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();

        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
        //_lookAction = InputSystem.actions["Look"];
        _dashAction = InputSystem.actions["Sprint"];

        //--
        _clickAction = InputSystem.actions["Attack"];
        //--

        _mainCamera = Camera.main.transform;
    }

    void Start()
    {
        _jumpTimeOutDelta = jumpTimeOut;
        _fallTimeOutDelta = fallTImeOut;
    }

    void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();
        //_lookInput = _lookAction.ReadValue<Vector2>();

        Gravity();

        Movement();

        if (_jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
        }

        if(_dashAction.WasPressedThisFrame() && _moveInput != Vector2.zero)
        {
            StartCoroutine(Dash());
        }

        //Pa cambiar
        /*
        if(_clickAction.WasPressedThisFrame())
        {
            InputManager.Instance.ChangeInputMap(InputManager.Instance.menuActionMap);
        }
        */
    }

    void Movement()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);
        

        if(direction != Vector3.zero)
        {
            targetAngle = Mathf.Atan2(direction.x, direction.z)*Mathf.Rad2Deg + _mainCamera.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _smoothTime);

            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);
        }
        Vector3 characterMovement = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        
        _controller.Move(direction * _movementSpeed * Time.deltaTime);
    }

    void Jump()
    {
        if(_jumpTimeOutDelta <= 0)
        {
            _playerGravity.y = Mathf.Sqrt(_jumpHeight * -2 * _gravity);
        }
    }

    void Gravity()
    {
        if(IsGrounded())
        {
            _fallTimeOutDelta = fallTImeOut;
            if(_playerGravity.y < 0)
            {
                _playerGravity.y = -2;
            }

            if(_jumpTimeOutDelta >= 0)
            {
                _jumpTimeOutDelta -= Time.deltaTime;
            }
        }
        
        else
        {
            _jumpTimeOutDelta = jumpTimeOut;

            if(_fallTimeOutDelta >= 0)
            {
                _fallTimeOutDelta -= Time.deltaTime;
            }
            else
            {
                //_animator.SetBool("Fall", true);
            }

            _playerGravity.y += _gravity * Time.deltaTime;
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;

        float timer = 0;

        while(timer < _dashTime)
        {
            _controller.Move(_lastMoveDirection.normalized * _dashSpeed * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(_sensor.position, _sensorRadius, _groundLayer);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_sensor.position, _sensorRadius);
    }
}
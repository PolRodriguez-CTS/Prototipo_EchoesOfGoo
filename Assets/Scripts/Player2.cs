using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player2 : MonoBehaviour
{
    //Componentes
    private CharacterController _controller;
    private Animator _animator;

    //Inputs
    private InputAction _moveAction;
    private Vector2 _moveInput;
    private InputAction _jumpAction;
    private InputAction _lookAction;
    private Vector2 _lookInput;
    private InputAction _dashAction;

    [SerializeField] private float _movementSpeed = 15;
    [SerializeField] private float _jumpHeight = 2;
    [SerializeField] private float _smoothTime = 0.2f;
    private float _turnSmoothVelocity;

    //Dash
    public float _dashSpeed = 30;
    public float _dashTime = 0.25f;
    private Vector3 _lastMoveDirection;
    private bool isDashing = false;

    //Gravedad
    [SerializeField] private float _gravity = -15f;
    [SerializeField] private Vector3 _playerGravity;

    //Ground Sensor
    [SerializeField] private Transform _sensor;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] float _sensorRadius;

    private Transform _mainCamera;

    //Libertinaje puro y duro
    public float _speedChangeRate = 20;
    public float speed;
    public float _animationSpeed;
    public bool isSprinting = false;
    public float _sprintSpeed = 8;
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
        _lookAction = InputSystem.actions["Look"];
        _dashAction = InputSystem.actions["Sprint"];

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
        _lookInput = _lookAction.ReadValue<Vector2>();

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
    }

    void Movement()
    {
        if(isDashing) return;

        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);

        float targetSpeed = _movementSpeed;

        if(direction == Vector3.zero)
        {
            targetSpeed = 0;
        }

        float currentSpeed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;

        if(currentSpeed < targetSpeed || currentSpeed > targetSpeed)
        {
            speed = Mathf.MoveTowards(speed, targetSpeed * direction.magnitude, _speedChangeRate * Time.deltaTime);
        }
        else
        {
            speed = targetSpeed;
        }

        _animationSpeed = Mathf.Lerp(_animationSpeed, targetSpeed, _speedChangeRate * Time.deltaTime);

        if(_animationSpeed < 0.05f)
        {
            _animationSpeed = 0;
        }

        //_animator.SetFloat("Speed", _animationSpeed);
        
        if (direction != Vector3.zero)
        {
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _mainCamera.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _smoothTime);

            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);
            _lastMoveDirection = (Quaternion.Euler(0, targetAngle, 0) * Vector3.forward).normalized;
        }

        Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

        _controller.Move(moveDirection.normalized * (speed * Time.deltaTime) + _playerGravity * Time.deltaTime);
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

    void Jump()
    {
        if(_jumpTimeOutDelta <= 0)
        {
            //_animator.SetBool("Jump", true);

            _playerGravity.y = Mathf.Sqrt(_jumpHeight * -2 * _gravity);
        }
    }

    void Gravity()
    {
        //_animator.SetBool("Grounded", IsGrounded());
        if(IsGrounded())
        {
            _fallTimeOutDelta = fallTImeOut;

            //_animator.SetBool("Jump", false);
            //_animator.SetBool("Fall", false);

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

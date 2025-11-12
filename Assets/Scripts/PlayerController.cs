using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
   //Componentes
    private CharacterController _characterController;
    //private Animator _animator;

    //Inputs
    private InputAction _moveAction;
    public Vector2 _moveValue;
    private InputAction _jumpAction;

    //Camara
    [SerializeField] private Transform _mainCamera;
    private float _turnSmoothVelocity;
    private float _smoothTime = 0.2f;

    //Parámetros
    private float _playerSpeed = 8;
    private float _playerJump = 2;

    //Gravedad
    private float _gravity = 2 * -9.81f;
    private Vector3 _playerGravity;

    //GroundSensor
    [SerializeField] private Transform _sensor;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _sensorRadius;

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        //_animator = GetComponentInChildren<Animator>();

        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        _moveValue = _moveAction.ReadValue<Vector2>();
        
        Movement();

        if (_jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
        }

        Gravity();
    }

    void Movement()
    {
        Vector3 direction = new Vector3(_moveValue.x, 0, _moveValue.y);
        /*_animator.SetFloat("Horizontal", _moveValue.x);
        _animator.SetFloat("Vertical", direction.magnitude);*/

        if (direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _mainCamera.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _smoothTime);
            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            _characterController.Move(moveDirection * _playerSpeed * Time.deltaTime);
        }

        
        //_animator.SetFloat("Horizontal", _moveValue.x);
        //_animator.SetFloat("Vertical", _moveValue.y);
    }
    

    void Jump()
    {
        //Debug.Log("salto");
        //_animator.SetBool("isJumping", true);

        _playerGravity.y = Mathf.Sqrt(_playerJump * -2 * _gravity);

        _characterController.Move(_playerGravity * Time.deltaTime);
    }

    void Gravity()
    {
        if (!IsGrounded())
        {
            _playerGravity.y += _gravity * Time.deltaTime;
        }

        else if (IsGrounded() && _playerGravity.y < 0)
        {
            _playerGravity.y = _gravity;
            //_animator.SetBool("isJumping", false);
        }

        _characterController.Move(_playerGravity * Time.deltaTime);
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

    /*bool DeathZoneDetection()
    {
        return Physics.CheckSphere(_deathZone.position, _deathZoneRadius, _deathLayer);
    }*/

    /*void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
  
            Death();
        }
    }*/
}

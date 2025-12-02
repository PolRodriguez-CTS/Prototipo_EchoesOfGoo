using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private InputAction _dashAction;

    //Camara
    [SerializeField] private Transform _mainCamera;
    private float _turnSmoothVelocity;
    private float _smoothTime = 0.1f;

    //Parámetros
    //<---------------------------------------------------------------------->
    private float _playerSpeed = 10;
//--> Jump
    private float _timeToMaxHeight;
    private float _playerJump = 5;
    
//--> Dash
    public float _dashSpeed = 20; //fuerza del dash
    public float _dashTime; //almacenar tiempo, lo que durará el dash
    private Vector3 _lastMoveDirection; //dirección en la que miras antes del dash
    private bool isDashing = false; //booleana de control para la acción del dash
    //<---------------------------------------------------------------------->

    //Gravedad
    private float _gravity = -9.81f;
    
    [SerializeField] private Vector3 _playerGravity;

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
        _dashAction = InputSystem.actions["Sprint"];
    }
    
    void Start()
    {
        //esto s¡no sabemos si arregla algo
        /*_gravity = -80;
        float timeToApex = -(_playerJump / _gravity);
        _timeToMaxHeight = timeToApex;*/
    }

    void Update()
    {
        _moveValue = _moveAction.ReadValue<Vector2>();
        
        if(isDashing == false)
        {
            Movement();
        }
        

        if (_jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
        }

        Gravity();

        if(_dashAction.WasPressedThisFrame())
        {
            StartCoroutine(Dash());
        }


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
            _lastMoveDirection = direction;

            _characterController.Move(moveDirection * _playerSpeed * Time.deltaTime);
        }
        //*Hacer que al estar quieto el smooth time sea casi instantaneo, es decir, cuando estás quieto el personaje cambia de orientación casi al instante, al estar en movimiento el factor de smooth que se note más
        
        //_animator.SetFloat("Horizontal", _moveValue.x);
        //_animator.SetFloat("Vertical", _moveValue.y);
    }


    IEnumerator Dash()
    {
        isDashing = true;

        for (int i = 0; i < i + _dashTime; i++)
        {
            _characterController.Move(_lastMoveDirection * _dashSpeed * Time.deltaTime);
            yield return null;
            /*if(i => _dashTime)
            {
                isDashing = false;
                Debug.Log("Ha acabado el dash");
            }*/
        }
        
        /*float startTime = Time.time;

        while (Time.time < startTime + _dashTime)
        {
            //Temporalmente para sacar el movimiento
            //Vector3 direction = new Vector3(_moveValue.x, 0, _moveValue.y);
            _characterController.Move(_lastMoveDirection * _dashSpeed * Time.deltaTime);
            yield return null;
        }*/

        /*while (i < i + _dashTime)
        {
            //Temporalmente para sacar el movimiento
            //Vector3 direction = new Vector3(_moveValue.x, 0, _moveValue.y);
            _characterController.Move(_lastMoveDirection * _dashSpeed * Time.deltaTime);
            yield return null;
        }*/

        isDashing = false;
    }
    

    void Jump()
    {
        //Debug.Log("salto");
        //_animator.SetBool("isJumping", true);
        
        
        float timeToApex = -(_playerJump / _gravity);
        _timeToMaxHeight = timeToApex;
        

        //Debug.Log(timeToApex); 

        _playerGravity.y = Mathf.Sqrt(_playerJump * -2 * _gravity);

        _characterController.Move(_playerGravity * Time.deltaTime);

        StartCoroutine(TimeToApex());

        /*if(_playerGravity.y == 0)
        {
            //_playerGravity.y = 
        }*/
    }

    IEnumerator TimeToApex()
    {
        yield return new WaitForSeconds(_timeToMaxHeight);
        //_gravity = -80;
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
}
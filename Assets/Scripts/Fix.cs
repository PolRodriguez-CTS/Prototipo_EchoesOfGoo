using UnityEngine;
using UnityEngine.InputSystem;

public class Fix : MonoBehaviour
{
    private CharacterController _characterController;

//--> Inputs
    private InputAction _moveAction;
    private Vector2 _moveInput;
    private InputAction _jumpAction;
    private InputAction _lookAction;
    private Vector2 _lookInput;
    private InputAction _dashAction;

//--> Cosas movimiento
    private float _playerSpeed = 5;


    void Awake()
    {
//--> Acceder a los Inputs
        _characterController = GetComponent<CharacterController>();
//--> Acceder a los Inputs
        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
        _lookAction = InputSystem.actions["Look"];
        _dashAction = InputSystem.actions["Dash"];
    }

    void Start(){}

    void Update()
    {
//--> Lectura de Inputs
        _moveInput = _moveAction.ReadValue<Vector2>();
        _lookInput = _lookAction.ReadValue<Vector2>();
    }

    void Movement()
    {

    }

//------------------------------------------------------------
/*
Explicación para retrasados (principalmente para el que escribió este código) del script de movimiento

--> Movimiento Completo
Usamos un vector para leer la dirección a partir de los Inputs --> direction = input de movimiento (en sus respectivos ejes)
Necesitaremos una variable tipo float, para almacenar (targetSpeed) y comprobar la velocidad del character controller (currentSpeed = characterController.velocity)

------ Sprint ------- (*No aplica para nuestro juego*)
Podemos crear una varaible que se use como pequeño offset para evitar errores por imprecisión

Si el jugador está caminando y quiere correr o viceversa, ejecutaremos una parte del código bajo esa condición:

Con un lerp y un ratio de interpolación cambiaremos de la velocidad actual a la velocidad deseada (podemos redondear este valor si queremos con Round)
---------------------

Cuando la dirección es diferente a 0, haremos lo siguiente

Sacamos la tangente del angulo entre dirección en X e Z (eje horizontal), nos devuelve el valor en radianes. Multiplicamos por Mathf.Rad2Deg para pasarlo a grados.

Esto sacará un ángulo respecto a los ejes fijos del mundo:

ej. práctico cámara

(((({

→ Input del jugador = “adelante” → dirección = (0, 0, 1)

→ Atan2(0,1) = 0°

→ La cámara apunta a 90° (mirando a la derecha global)

--- Sin sumar la cámara ---

targetAngle = 0°
→ el personaje avanza hacia el “norte” del mundo, aunque la cámara mire a la derecha.

--- Sumando la cámara ---

targetAngle = 0° + 90° = 90°
→ el personaje avanza hacia donde está viendo la cámara.

}))))


*/
//------------------------------------------------------------


/*
    void MovementCompleto()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);

        float targetSpeed;

        if(isSprinting)
        {
            targetSpeed = _sprintSpeed;
        }

        if(direction == Vector3.zero)
        {
            targetSpeed = 0;
        }

        float currentSpeed = new Vector3(_characterController.velocity.x, 0, _characterController.velocity.z).magnitude;

        float speedOffset = 0.1f;

        if(currentSpeed < targetSpeed - speedOffset || currentSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * _speedChangeRate);

            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }


        _animationSpeed = Mathf.Lerp(_animationSpeed, targetSpeed, Time.deltaTime * _speedChangeRate);

        if(_animationSpeed < 0.1f)
        {
            _animationSpeed = 0;
        }

        _animator.SetFloat("Speed", _animationSpeed);

        if (direction != Vector3.zero)
        {
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _mainCamera.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _smoothTime);

            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);
        }

        Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

        _controller.Move(_speed * Time.deltaTime * moveDirection.normalized + _playerGravity * Time.deltaTime);
    }
    */
}

using UnityEngine;

public class CameraController : MonoBehaviour
{
    private PlayerInput _input;  
    private Transform _transform;
    private Vector3 _vMove, _targetAngles, _followAngles, _followVelocity = Vector3.zero;
    private bool _controllerPauseState = false, _leftShiftPressed = false;
    private float _mouseSensitivity = 0.5f;
    private float _moveSpeed = 5.0f;

    private void Awake()
    {
        _input = new PlayerInput();
        _input.Player.LShift.performed += context => _leftShiftPressed = true;
        _input.Player.LShift.canceled += context => _leftShiftPressed = false;
        _input.Player.Esc.performed += context => SwipeControllerPauseState();
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    private void Start()
    {
        _vMove = new Vector3(0, 0, 0);
        _transform = gameObject.GetComponent<Transform>();
        SwipeControllerPauseState();
    }

    private void Update()
    {
        if (!_controllerPauseState)
        {
            if (_input.Player.MouseX.ReadValue<float>() != 0 || _input.Player.MouseY.ReadValue<float>() != 0) //Вращение камеры
            {
                float mouseYInput = 0;
                float mouseXInput = 0;

                mouseYInput = _input.Player.MouseY.ReadValue<float>();
                mouseXInput = _input.Player.MouseX.ReadValue<float>();

                if (_targetAngles.y > 180)
                {
                    _targetAngles.y -= 360; _followAngles.y -= 360;
                }
                else if (_targetAngles.y < -180)
                {
                    _targetAngles.y += 360; _followAngles.y += 360;
                }

                if (_targetAngles.x > 180)
                {
                    _targetAngles.x -= 360; _followAngles.x -= 360;
                }
                else if (_targetAngles.x < -180)
                {
                    _targetAngles.x += 360; _followAngles.x += 360;
                }

                _targetAngles.y += mouseXInput * _mouseSensitivity;
                _targetAngles.x += mouseYInput * _mouseSensitivity;

                _targetAngles.x = Mathf.Clamp(_targetAngles.x, -0.5f * 180, 0.5f * 180);
                _followAngles = Vector3.SmoothDamp(_followAngles, _targetAngles, ref _followVelocity, 0.5f / 100);

                _transform.localRotation = Quaternion.Euler(-_followAngles.x, _followAngles.y, 0);
            }

            Vector2 moveVector = _input.Player.WASD.ReadValue<Vector2>();
            if (moveVector != Vector2.zero)
            {
                _vMove.x = moveVector.x;
                _vMove.z = moveVector.y;
                _transform.Translate(_vMove * Time.deltaTime * (_leftShiftPressed ? _moveSpeed * 2.0f : _moveSpeed));
            }

            if (_input.Player.MouseWheel.ReadValue<float>() != 0) //Увеличение скорости на колёсико мыши
            {
                _moveSpeed += _input.Player.MouseWheel.ReadValue<float>() > 0 ? _moveSpeed < 25 ? 0.25f : 0 : _moveSpeed > 1 ? -0.25f : 0;
            }
        
        }        
    }

    public void SwipeControllerPauseState()
    {
        _controllerPauseState = !_controllerPauseState;
        Cursor.lockState = _controllerPauseState ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _controllerPauseState;
    }

    public void ChangeMouseSens(int sens)
    {
        _mouseSensitivity = sens / 10.0f;
    }
}

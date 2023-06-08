using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Moveable))]
public class CameraController : MonoBehaviour, IOscControllable
{
    public string OscAddress => "/camera";
    public Transform OrbitTarget { get => _orbitTarget; set => ChangeOrbitCameraTarget(value); }

    [SerializeField] private float _moveSpeed = 0.05f;
    [SerializeField] private float _rotateSpeed = 0.05f;
    [SerializeField] private float _lerpSpeed = 0.5f;
    [SerializeField] private bool _orbitCameraEnabled = false;
    [SerializeField] private Transform _orbitTarget;
    
    private Moveable _moveable;
    private Rigidbody _rigidbody;
    private PlayerInputActions _playerInputActions;
    private Vector2 _translateInput;
    private Vector2 _rotateInput;
    private float _verticalInput;
    private SceneController _sceneController;

    private float _orbitDistance = 10f;


    private TransformSnapshot _originalPosition;

    private void Start()
    {
        _moveable = GetComponent<Moveable>();
        RegisterEndpoints();

        _originalPosition = new TransformSnapshot(gameObject.transform);
    }

    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Camera.Enable();
        _playerInputActions.Camera.Translate.performed += OnTranslate;
        _playerInputActions.Camera.Up.performed += OnMoveUp;
        _playerInputActions.Camera.Down.performed += OnMoveDown;
        _playerInputActions.Camera.Look.performed += OnLook;
        _playerInputActions.Camera.Reset.performed += ResetCamera;

        _sceneController = FindObjectOfType<SceneController>();
        _orbitTarget = _sceneController.transform;
    }

    private void OnDisable()
    {
        _playerInputActions.Camera.Disable();
        _playerInputActions.Camera.Translate.performed -= OnTranslate;
        _playerInputActions.Camera.Up.performed -= OnMoveUp;
        _playerInputActions.Camera.Down.performed -= OnMoveDown;
        _playerInputActions.Camera.Look.performed -= OnLook;
        _playerInputActions.Camera.Reset.performed -= ResetCamera;
    }


    private void OnTranslate(InputAction.CallbackContext ctx) {
        _translateInput = ctx.ReadValue<Vector2>() * _moveSpeed;
        // var currentPos = _moveable.TargetSnapshot.Position;
        // currentPos.x += ctx.ReadValue<Vector2>().x * _moveSpeed;
        // currentPos.z += ctx.ReadValue<Vector2>().y * _moveSpeed;
        // Debug.Log("Move performed : " + ctx.ReadValue<Vector2>());
        // _moveable.MoveTo(currentPos, _moveSpeed);
        // _rigidbody.MovePosition(currentPos);
        // _rigidbody.AddForce(currentPos);
    }

    private void OnMoveUp(InputAction.CallbackContext ctx) {
        var currentPos = _moveable.TargetSnapshot.Position;
        currentPos.y += ctx.ReadValue<float>() * _moveSpeed;
        // _moveable.MoveTo(currentPos, _moveSpeed);
        _rigidbody.MovePosition(currentPos);
    }

    private void OnMoveDown(InputAction.CallbackContext ctx) {
        var currentPos = _moveable.TargetSnapshot.Position;
        currentPos.y -= ctx.ReadValue<float>() * _moveSpeed;
        // _moveable.MoveTo(currentPos, _moveSpeed);
        _rigidbody.MovePosition(currentPos);
    }

    private void OnLook(InputAction.CallbackContext ctx) {
        _rotateInput = ctx.ReadValue<Vector2>();
    }

    private void ChangeOrbitCameraTarget(Transform newTarget)
    {
        _orbitTarget = newTarget;
    }

    private void OrbitCamera()
    {
        var lookRotation = Quaternion.LookRotation(_orbitTarget.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _lerpSpeed);


        // left/right on _rotateInput control rotation
        
        Quaternion targetRotation = Quaternion.Euler(_rotateInput.y, 0, _rotateInput.x) * transform.rotation;

        // Perform a smooth transition between current and target rotation
        Quaternion smoothRotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotateSpeed);

        _moveable.RotateTo(smoothRotation, _lerpSpeed);
        // _rigidbody.AddTorque(transform.up * _rotateInput.x * _rotateSpeed, ForceMode.Impulse);


        // now do the positon - left/right on left joystick = rotation degrees around central axis

        float distance = Vector3.Distance(transform.position, _orbitTarget.position);
        
        // calculate position around circle
        float x = Mathf.Sin(Time.time * _moveSpeed) * distance;
        float z = Mathf.Cos(Time.time * _moveSpeed) * distance;
        Vector3 targetPosition = new Vector3(x, transform.position.y, z) + _orbitTarget.position;

        // move to position
        _moveable.MoveTo(targetPosition, _lerpSpeed);


        // Vector3 direction = (transform.position - _orbitTarget.position).normalized;
        // Vector3 targetPosition = _orbitTarget.position + direction * distance;


    }

    private void MoveCamera()
    {
        // var currentPos = _moveable.TargetSnapshot.Position;
        // currentPos.x += _translateInput.x;
        // currentPos.z += _translateInput.y;
        // _moveable.MoveTo(currentPos, _moveSpeed);

        // add a force towards the front of where the camera is looking
        var force = transform.forward * _translateInput.y * _moveSpeed;
        force += transform.right * _translateInput.x * _moveSpeed;
        _rigidbody.AddForce(force, ForceMode.Impulse);


        Quaternion targetRotation = Quaternion.Euler(_rotateInput.y, 0, _rotateInput.x) * transform.rotation;

        // Perform a smooth transition between current and target rotation
        Quaternion smoothRotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateSpeed);

        Quaternion rotation = Quaternion.Euler(-_rotateInput.y * _rotateSpeed, _rotateInput.x * _rotateSpeed, 0f);

        transform.rotation *= rotation; 
        // transform.rotation = transform.
        // _moveable.RotateTo(rotation, _lerpSpeed);    


        // Apply the rotation to the rigidbody
        // _rigidbody.MoveRotation(smoothRotation);
        // _rigidbody.AddTorque(transform.up * _rotateInput.x * _rotateSpeed, ForceMode.Impulse);
        // _rigidbody.AddTorque(smoothRotation.eulerAngles * _rotateSpeed, ForceMode.VelocityChange);
        // _rigidbody.AddTorque(rotation.eulerAngles, ForceMode.VelocityChange);
    }

    private void FixedUpdate()
    {

        if (_orbitCameraEnabled)
        {
            OrbitCamera();
        }
        else
        {
            MoveCamera();
        }


        // rotate the camera by applying torque
        // _rigidbody.AddTorque(transform.up * _rotateInput.x * _moveSpeed, ForceMode.VelocityChange);
        // 

        // Quaternion targetRotation = Quaternion.Euler(_rotateInput.y, 0, _rotateInput.x) * transform.rotation;
        // Perform a smooth transition between current and target rotation
        // Quaternion smoothRotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotateSpeed);

        // var currentRot = transform.rotation;
        // Vector3 euler = currentRot.eulerAngles;
        // euler.x += _rotateInput.y * _moveSpeed * -1;
        // euler.y += _rotateInput.x * _moveSpeed;
        // _moveable.RotateTo(Quaternion.Euler(euler.x, euler.y, euler.z), _moveSpeed);
    }

    public void RegisterEndpoints()
    {
        // OscManager.Instance.AddEndpoint($"{OscAddress}/moveX", (OscDataHandle dataHandle) => {
        //     var currentPos = _moveable.TargetSnapshot.Position;
        //     currentPos.x = dataHandle.GetElementAsFloat(0);
        //     _moveable.MoveTo(currentPos, _moveSpeed);
        // });

        // OscManager.Instance.AddEndpoint($"{OscAddress}/moveY", (OscDataHandle dataHandle) => {
        //     var currentPos = _moveable.TargetSnapshot.Position;
        //     currentPos.y = dataHandle.GetElementAsFloat(0);
        //     _moveable.MoveTo(currentPos, _moveSpeed);
        // });

        // OscManager.Instance.AddEndpoint($"{OscAddress}/moveZ", (OscDataHandle dataHandle) => {
        //     Debug.Log("Move Z called! Amount: " + dataHandle.GetElementAsFloat(0));
        //     var currentPos = _moveable.TargetSnapshot.Position;
        //     currentPos.z = dataHandle.GetElementAsFloat(0);
        //     _moveable.MoveTo(currentPos, _moveSpeed);
        // });

        // OscManager.Instance.AddEndpoint($"{OscAddress}/rotateX", (OscDataHandle dataHandle) => {
        //     var currentRot = transform.rotation;
        //     Vector3 euler = currentRot.eulerAngles;
        //     euler.x += dataHandle.GetElementAsFloat(0);
        //     _moveable.RotateTo(Quaternion.Euler(euler.x, euler.y, euler.z), _moveSpeed);
        // });

        // OscManager.Instance.AddEndpoint($"{OscAddress}/speed", (OscDataHandle dataHandle) => {
        //     _moveSpeed = dataHandle.GetElementAsFloat(0);
        // });
    }

    private void ResetCamera(InputAction.CallbackContext ctx)
    {
        // StartCoroutine(ResetCameraDelay());
        _rigidbody.velocity = Vector3.zero;
        transform.position = _originalPosition.Position;
        transform.rotation = _originalPosition.Rotation;
    }


    // private IEnumerator ResetCameraDelay()
    // {
    //     Debug.Log("RESETTNG CAMERA!");
    //     _rigidbody.isKinematic = true;
    //     // _moveable.TransformTo(_originalPosition);
    //     yield return new WaitForSeconds(4f);
    //     _rigidbody.isKinematic = false;
    // }
}

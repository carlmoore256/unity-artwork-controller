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
    private ArtworkSceneController _sceneController;

    private float _orbitDistance = 10f;

    private bool _isMoving = false;

    [SerializeField] private float _cameraResetDuration = 3f;


    private TransformSnapshot _originalPosition;

    private void Start()
    {
        _moveable = GetComponent<Moveable>();

        _originalPosition = new TransformSnapshot(gameObject.transform);
    }

    private void OnEnable()
    {
        RegisterEndpoints();

        _rigidbody = GetComponent<Rigidbody>();
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Camera.Enable();
        _playerInputActions.Camera.Translate.performed += OnTranslate;
        _playerInputActions.Camera.Up.performed += OnMoveUp;
        _playerInputActions.Camera.Down.performed += OnMoveDown;
        _playerInputActions.Camera.Look.performed += OnLook;
        _playerInputActions.Camera.Reset.performed += ResetCamera;

        _sceneController = FindObjectOfType<ArtworkSceneController>();
        _orbitTarget = _sceneController.transform;
    }

    private Coroutine _resetCameraCoroutine;
    public void RegisterEndpoints()
    {
        OscManager.Instance.AddEndpoint("/camera/reset", (OscDataHandle oscDataHandle) => {
            Debug.Log("Resetting camera");
            if (_resetCameraCoroutine != null)
            {
                StopCoroutine(_resetCameraCoroutine);
            }
            _resetCameraCoroutine = StartCoroutine(LerpCameraToDefault());
        });
    }

    private void OnDisable()
    {   
        if (OscManager.Instance != null)
        // OscManager.Instance.RemoveEndpoint
            OscManager.Instance.RemoveEndpoint("/camera/reset");

        _playerInputActions.Camera.Disable();
        _playerInputActions.Camera.Translate.performed -= OnTranslate;
        _playerInputActions.Camera.Up.performed -= OnMoveUp;
        _playerInputActions.Camera.Down.performed -= OnMoveDown;
        _playerInputActions.Camera.Look.performed -= OnLook;
        _playerInputActions.Camera.Reset.performed -= ResetCamera;
    }


    private void OnTranslate(InputAction.CallbackContext ctx) {
        _translateInput = ctx.ReadValue<Vector2>() * _moveSpeed;
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

    }

    private void MoveCamera()
    { 
       // add a force towards the front of where the camera is looking
        var force = transform.forward * _translateInput.y * _moveSpeed;
        force += transform.right * _translateInput.x * _moveSpeed;
        _rigidbody.AddForce(force, ForceMode.Impulse);

        Quaternion targetRotation = Quaternion.Euler(_rotateInput.y, 0, _rotateInput.x) * transform.rotation;

        // Perform a smooth transition between current and target rotation
        Quaternion smoothRotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateSpeed);

        Quaternion rotation = Quaternion.Euler(-_rotateInput.y * _rotateSpeed, _rotateInput.x * _rotateSpeed, 0f);

        transform.rotation *= rotation; 
    }

    private void FixedUpdate()
    {
        if (_isMoving)
        {
            return;
        }

        if (_orbitCameraEnabled)
        {
            OrbitCamera();
        }
        else
        {
            MoveCamera();
        }
    }


    public void UnregisterEndpoints()
    {
        
    }

    private void ResetCamera(InputAction.CallbackContext ctx)
    {
        // transform.position = _originalPosition.Position;
        // transform.rotation = _originalPosition.Rotation;
        if (_resetCameraCoroutine != null) {
            StopCoroutine(_resetCameraCoroutine);
        }
        _resetCameraCoroutine = StartCoroutine(LerpCameraToDefault());
    }

    private IEnumerator LerpCameraToDefault()
    {
        _rigidbody.isKinematic = true;
        _isMoving = true;
        // _moveable.TransformTo(_originalPosition);
        float t = 0;
        // lerp camera to _original position
        float duration = _cameraResetDuration;
        float time = 0f;
        while (time < duration) {
            transform.position = Vector3.Lerp(transform.position, _originalPosition.Position, time/duration);
            transform.rotation = Quaternion.Lerp(transform.rotation, _originalPosition.Rotation, time/duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = _originalPosition.Position;
        transform.rotation = _originalPosition.Rotation;

        _rigidbody.isKinematic = false;
        _isMoving = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero; // Add this line
        _rigidbody.Sleep(); // Add this line
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

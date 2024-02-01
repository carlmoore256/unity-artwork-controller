using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(OrbitCamera))]
public class CameraInput : MonoBehaviour
{
    private OrbitCamera _orbitCamera;
    [SerializeField] private InputAction _orbitAction;
    [SerializeField] private InputAction _zoomAction;

    [SerializeField] private float _orbitMultiplier = 1f;
    [SerializeField] private float _zoomMultiplier = 0.1f;

    private void Start()
    {
#if UNITY_EDITOR
        if (!Mouse.current.enabled)
        {
            InputSystem.EnableDevice(Mouse.current);
        }
#endif
    }

    private void OnEnable()
    {
        _orbitCamera = GetComponent<OrbitCamera>();
        _orbitAction.Enable();

        _orbitAction.performed += ctx =>
        {
            var value = ctx.ReadValue<Vector2>();
            value.y *= -1;
            _orbitCamera.Rotate(value * _orbitMultiplier);
        };

        _zoomAction.Enable();
        _zoomAction.performed += ctx =>
        {
            // var value = ctx.ReadValue<float>();
            var value = ctx.ReadValue<Vector2>().y * -1;
            _orbitCamera.ChangeRadius(value * _zoomMultiplier);
        };


    }

    private void OnDisable()
    {
        _orbitAction.Disable();
        _zoomAction.Disable();
    }

    private void Update()
    {
        // var mouseX = Input.GetAxis("Mouse X");
        // var mouseY = Input.GetAxis("Mouse Y");
        // // var mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        // _orbitCamera.Rotate(new Vector2(mouseX * _orbitMultiplier, -mouseY * _orbitMultiplier));
        // _orbitCamera.Zoom(mouseScroll);
        _orbitCamera.ApplyTransform(transform);
    }





}
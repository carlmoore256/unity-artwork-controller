using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.PlayerLoop;

public class OrbitCamera : MonoBehaviour
{
    public Transform target;
    public float azimuthSpeed = 0.01f;
    public float elevationSpeed = 0.01f;
    public float minElevation = 1f;

    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
        }
    }

    [SerializeField]
    private float _radius = 10f;

    [SerializeField]
    private float focusLerpSpeed = 0.1f;

    [SerializeField]
    private float yOffset = 1f;

    [SerializeField]
    private float lerpSpeed = 0.1f; // Lerp speed for position and rotation

    [SerializeField]
    private float azimuth;

    [SerializeField]
    private float elevation = 3f;

    [SerializeField]
    private float initAzimuth = 180f;

    [SerializeField]
    private float minRadius = 1f;

    [SerializeField]
    private float maxRadius = 20f;

    [SerializeField]
    private float zoomSpeed = 5f;

    [SerializeField]
    private float arrowSpeed = 5f;

    [SerializeField]
    private bool clampElevation = true;


    [SerializeField]
    private float _dofOffset = -0.5f;


    private void Start()
    {
        // if (target != null)
        //     _radius = Vector3.Distance(transform.position, target.position);
        // UpdateDepthOfFieldFocusDistance(_radius);
    }


    public void ApplyTransform(Transform t)
    {
        if (target == null)
        {
            Debug.LogWarning("No target set for OrbitCamera");
            return;
        }

        azimuth += azimuthSpeed * Time.deltaTime;
        var rotation = Quaternion.Euler(elevation, azimuth, 0);
        var position = rotation * new Vector3(0.0f, 0.0f, -_radius) + target.position;
        t.SetPositionAndRotation(
            Vector3.Slerp(transform.position, position, lerpSpeed * Time.deltaTime),
            Quaternion.Slerp(transform.rotation, rotation, lerpSpeed * Time.deltaTime)
        );

        t.LookAt(
            new Vector3(target.position.x, target.position.y + yOffset, target.position.z)
        );
    }

    private Coroutine _restorePositionRoutine;
    private Vector2 _restorePosition;
    private float _restoreRadius;

    public void CancelRestorePosition()
    {
        if (_restorePositionRoutine != null)
        {
            StopCoroutine(_restorePositionRoutine);
        }
    }

    // temporarily allows camera to be panned to a position, but restores after given timeout
    public void RotateTemporary(Vector2 axis, float timeout)
    {
        Debug.Log($"[OrbitCamera] RotateTemporary: {axis} | {timeout}");
        Vector2 restorePosition = new(azimuth, elevation);
        azimuth += axis.x;
        elevation += axis.y;
        CancelRestorePosition();
        _restorePositionRoutine = CoroutineHelpers.DelayedAction(
            () =>
            {
                azimuth = restorePosition.x;
                elevation = restorePosition.y;
            },
            timeout,
            this
        );
    }

    public void Rotate(Vector2 axis)
    {
        Rotate(axis.x, axis.y);
    }

    public void Rotate(float horizontal, float vertical)
    {
        // make sure we cancel any existing restore position routine
        CancelRestorePosition();
        azimuth += horizontal;
        elevation += vertical;
        if (clampElevation)
        {
            elevation = Mathf.Clamp(elevation, minElevation, 90f);
        }
    }

    public void ChangeRadius(float delta)
    {
        Radius = Mathf.Clamp(_radius + delta * zoomSpeed, minRadius, maxRadius);
    }

    public void ChangeRadiusTemporary(float delta, float timeout)
    {
        float restoreRadius = _radius;
        Radius = Mathf.Clamp(_radius + delta * zoomSpeed, minRadius, maxRadius);
        CancelRestorePosition();
        _restorePositionRoutine = CoroutineHelpers.DelayedAction(
            () =>
            {
                Radius = restoreRadius;
            },
            timeout,
            this
        );
    }

    public void FocusOn(Transform t)
    {
        CancelRestorePosition();
        target = t;
    }

    public void LookAtSecondaryTarget(Transform secondaryTarget, float newElevation = 20f)
    {
        CancelRestorePosition();

        // Direction from primary target to secondary target
        Vector3 toSecondary = (secondaryTarget.position - target.position).normalized;

        // Desired direction from camera to primary target for the secondary target to be in-between
        Vector3 desiredDirectionToPrimary = -toSecondary;

        // Compute the azimuth
        azimuth =
            -90f
            - (
                Mathf.Atan2(desiredDirectionToPrimary.z, desiredDirectionToPrimary.x)
                * Mathf.Rad2Deg
            );

        elevation = Mathf.Clamp(newElevation, minElevation, 90f);

        // Update the focus distance
        float distanceToSecondary = Vector3.Distance(
            secondaryTarget.position,
            transform.position
        );
        // UpdateDepthOfFieldFocusDistance(distanceToSecondary);
    }

    /// <summary>
    /// Solves for the azimuth and elevation of the camera based on a new position
    /// </summary>
    /// <param name="pos"></param>
    public void SolvePosition(Vector3 pos)
    {
        CancelRestorePosition();
        Debug.Log(
            $"[OrbitCamera] Solving position: {pos} | Current => radius: {_radius}, azimuth: {azimuth}, elevation: {elevation}"
        );
        // Calculate relative position to the target
        Vector3 relativePos = pos - target.position;
        // Calculate the radius
        _radius = Mathf.Sqrt(
            relativePos.x * relativePos.x
                + relativePos.y * relativePos.y
                + relativePos.z * relativePos.z
        );
        // Calculate the azimuth (in degrees)
        azimuth = Mathf.Atan2(relativePos.z, relativePos.x) * Mathf.Rad2Deg;
        // Calculate the elevation (in degrees)
        elevation =
            Mathf.Atan2(
                relativePos.y,
                Mathf.Sqrt(relativePos.x * relativePos.x + relativePos.z * relativePos.z)
            ) * Mathf.Rad2Deg;

        elevation = Mathf.Clamp(elevation, minElevation, 90f);

        Debug.Log(
            $"[OrbitCamera] Solved position: {pos} | New => radius: {_radius}, azimuth: {azimuth}, elevation: {elevation}"
        );
    }

    public float DistanceFromTarget()
    {
        return Vector3.Distance(transform.position, target.position);
    }
}
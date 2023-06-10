using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
using System.Linq;

public class MotifMotionController : MonoBehaviour, IOscControllable
{
    public int _artworkId = 0;
    public int ArtworkId => _artworkId;
    public Artwork Artwork => GetComponent<Artwork>();
    public string OscAddress => $"/artwork/{Artwork.Index}/motion";

    // private List<Moveable> _moveables = new List<Moveable>();
    private Dictionary<Moveable, TransformSnapshot> _targetSnapshots = new Dictionary<Moveable, TransformSnapshot>();

    [SerializeField] public float _moveXAmount = 0.1f;
    [SerializeField] public float _moveYAmount = 0.1f;
    [SerializeField] public float _moveZAmount = 0.1f;

    [SerializeField] private float _speed = 1f;
    [SerializeField] private bool _enableMotion = true;


    Transform LookAtTarget;



    void Start()
    {
        LookAtTarget = GameObject.Find("Main Camera").transform;

        Artwork.ForeachMoveable((moveable) => {
            _targetSnapshots.Add(moveable, moveable.CurrentSnapshot);
        });

        // _moveables.AddRange(FindObjectsOfType<Moveable>());
        // _moveables.AddRange(Artwork.AllMoveables);
        // foreach(var moveable in Artwork.AllMoveables)
        // {
        //     _targetSnapshots.Add(moveable, moveable.CurrentSnapshot);           
        // }

    }

    void OnEnable()
    {
        RegisterEndpoints();
    }

    void OnDisable()
    {
        if (OscManager.Instance == null)
            return;
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/sinX");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/cosY");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/sinZ");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/speed");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/range");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/lookAtCamera");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/reset");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/lerpCenter");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/lerpReset");
        // OscManager.Instance.RemoveEndpoint($"{OscAddress}/lerpTarget");
        // OscManager.Instance.RemoveEndpoint($"{OscAddress}/enableMotion");
        // OscManager.Instance.RemoveEndpoint($"{OscAddress}/enableLookAtTarget");
        // OscManager.Instance.RemoveEndpoint($"{OscAddress}/enableLookAtCamera");
        
    }

    // idea : add a camera controller

    public void RegisterEndpoints()
    {
        OscManager.Instance.AddEndpoint($"{OscAddress}/sinX", (OscDataHandle dataHandle) => {
            _moveXAmount = dataHandle.GetElementAsFloat(0);
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/cosY", (OscDataHandle dataHandle) => {
            _moveYAmount = dataHandle.GetElementAsFloat(0);
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/sinZ", (OscDataHandle dataHandle) => {
            _moveZAmount = dataHandle.GetElementAsFloat(0);
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/speed", (OscDataHandle dataHandle) => {
            _speed = dataHandle.GetElementAsFloat(0);
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/range", (OscDataHandle dataHandle) => {
            _speed = dataHandle.GetElementAsFloat(0);
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/lookAtCamera", (OscDataHandle dataHandle) => {
            Artwork.ForeachMoveable((moveable) => {
                moveable.LookAtTarget = LookAtTarget;
                moveable.EnableLookAtTarget = !moveable.EnableLookAtTarget;
            });
            // foreach(var m in _moveables)
            // {
            //     m.LookAtTarget = Camera.main.transform;
            //     m.EnableLookAtTarget = !m.EnableLookAtTarget;
            // }
        });


        OscManager.Instance.AddEndpoint($"{OscAddress}/reset", (OscDataHandle dataHandle) => {
            Artwork.ForeachMoveable((moveable) => {
                moveable.ResetToDefault();
            });

            // foreach(var moveable in _moveables)
            // {
            //     moveable.ResetToDefault();
            // }
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/lerpCenter", (OscDataHandle dataHandle) => {
            var allMoveables = Artwork.AllMoveables;
            var center = allMoveables.Select(m => m.CurrentSnapshot.Position).Aggregate((a, b) => a + b);
            center /= allMoveables.Count();
            float value = dataHandle.GetElementAsFloat(0);

            foreach(var moveable in allMoveables)
            {
                var newPos = Vector3.Lerp(moveable.CurrentSnapshot.Position, center, value);
                _targetSnapshots[moveable].Position = newPos;
            }
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/lerpReset", (OscDataHandle dataHandle) => {
            Artwork.ForeachMoveable((moveable) => {
                moveable.LerpToDefault(dataHandle.GetElementAsFloat(0));
            });
        });
    }

    float _phase = 0f;


    void OscillatingMovement()
    {
        _phase += Time.deltaTime * _speed;
        if (_phase > 1)
        {
            _phase = 0;
        }

        Artwork.ForeachMoveable((moveable, normIndex) => {
            var currentPos = moveable.transform.position;
            currentPos.x += Mathf.Sin((_phase + normIndex) * 2 * Mathf.PI) * _moveXAmount;
            currentPos.y += Mathf.Cos((_phase + normIndex) * 2 * Mathf.PI) * _moveYAmount;
            currentPos.z += Mathf.Sin((_phase + normIndex) * 2 * Mathf.PI) * _moveZAmount;
            _targetSnapshots[moveable].Position = currentPos;
        });
    }

    void RandomMovement()
    {
        Artwork.ForeachMoveable((moveable, normIndex) => {
            var currentPos = moveable.transform.position;
            currentPos.x += Random.Range(-1f, 1f) * _moveXAmount;
            currentPos.y += Random.Range(-1f, 1f) * _moveYAmount;
            currentPos.z += Random.Range(-1f, 1f) * _moveZAmount;
            _targetSnapshots[moveable].Position = currentPos;
        });
    }

    // we have to do this because there may be multiple operations on all of the moveables
    void UpdateTargetSnapshots()
    {
        Artwork.ForeachMoveable(movable => _targetSnapshots[movable] = movable.CurrentSnapshot);
    }

    void ApplyTargetSnapshots()
    {
        Artwork.ForeachMoveable(movable => movable.TransformTo(_targetSnapshots[movable]));
    }
    
    void Update()
    {
        if (!_enableMotion) return;
        UpdateTargetSnapshots();
        OscillatingMovement();
        ApplyTargetSnapshots();
    }
}


    // Artwork.ForeachMotif((motif, normIndex) => {
    //     // Moveable moveable = motif.Moveable;
    //     // // TransformSnapshot snapshot = moveable.CurrentSnapshot;
    //     // // TransformSnapshot snapshot = _targetSnapshots[moveable];
    //     // Vector3 newPos = _targetSnapshots[moveable].Position;

    //     var currentPos = motif.GetPosition();


    //     Debug.Log("NORM INDEX " + normIndex);

    //     // currentPos.x += Random.RandomRange(-0.1f, 0.1f) * moveXAmount;
    //     currentPos.x += Mathf.Sin((_phase + normIndex) * 2 * Mathf.PI) * _moveXAmount;
    //     currentPos.y += Mathf.Cos((_phase + normIndex) * 2 * Mathf.PI) * _moveYAmount;
    //     currentPos.z += Mathf.Sin((_phase + normIndex) * 2 * Mathf.PI) * _moveZAmount;

    //     motif.ForeachMoveable((moveable) => {
    //         moveable.MoveTo(currentPos, 0.6f);
    //     });
    // });

    // for(int i = 0; i < _moveables.Count; i++)
    // {
    //     float fracIndex = (float)i / (float)_moveables.Count;
    //     Moveable moveable = _moveables[i];
    //     // TransformSnapshot snapshot = moveable.CurrentSnapshot;
    //     // TransformSnapshot snapshot = _targetSnapshots[moveable];
    //     Vector3 newPos = _targetSnapshots[moveable].Position;

    //     // currentPos.x += Random.RandomRange(-0.1f, 0.1f) * moveXAmount;
    //     newPos.x += Mathf.Sin((_phase + fracIndex) * 2 * Mathf.PI) * _moveXAmount;
    //     newPos.y += Mathf.Cos((_phase + fracIndex) * 2 * Mathf.PI) * _moveYAmount;
    //     newPos.z += Mathf.Sin((_phase + fracIndex) * 2 * Mathf.PI) * _moveZAmount;

    //     // moveable.MoveTo(newPos, 0.6f);
    //     _targetSnapshots[moveable].Position = newPos;
    //     // _targetSnapshots[moveable] = snapshot;
    // }
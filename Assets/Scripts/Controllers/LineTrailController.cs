using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;

public class LineTrailController : MonoBehaviour, IOscControllable, IArtworkController
{
    [SerializeField] private int _trailLength = 10;
    [SerializeField] private bool _isEnabled = false;
    [SerializeField] private float _startWidth = 0.0f;
    [SerializeField] private float _endWidth = 0.1f;

    private List<LineTrail> _lineTrails = new List<LineTrail>();
    private Material _lineMaterial;

    public Artwork Artwork => GetComponent<Artwork>();
    public string OscAddress => $"/artwork/{Artwork.Id}/line";


    void Start()
    {
        _lineMaterial = Resources.Load<Material>("Materials/LineMaterial");
    }

    void OnEnable()
    {
        RegisterEndpoints();

        _lineTrails.Clear();
        Artwork.ForeachMotif((motif) => {
            var lineTrail = motif.gameObject.GetComponent<LineTrail>();
            if (lineTrail == null) {
                motif.gameObject.AddComponent<LineTrail>();
            }
            lineTrail.Initialize(_trailLength, _endWidth, _startWidth, _lineMaterial);
            _lineTrails.Add(lineTrail);
        });
    }

    void OnDisable()
    {
        UnregisterEndpoints();
    }

    public void RegisterEndpoints() {
        OscManager.Instance.AddEndpoint($"{OscAddress}/toggle", (OscDataHandle dataHandle) => {
            _isEnabled = !_isEnabled;
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/length", (OscDataHandle dataHandle) => {
            var length = dataHandle.GetElementAsInt(0);
            if (length <= 0) {
                if (_isEnabled) DisableLineTrails();
                return;
            } 

            if (!_isEnabled) EnableLineTrails();
            _lineTrails.ForEach(trail => trail.SetLineCount(length));
        });

        // add a spider-web effect
    }

    public void UnregisterEndpoints()
    {
        if (OscManager.Instance == null) return;
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/toggle");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/length");
    }

    private void DisableLineTrails() {
        _lineTrails.ForEach(trail => trail.enabled = false);
        _isEnabled = false;
    }

    private void EnableLineTrails() {
        _lineTrails.ForEach(trail => trail.enabled = true);
        _isEnabled = true;
    }
}

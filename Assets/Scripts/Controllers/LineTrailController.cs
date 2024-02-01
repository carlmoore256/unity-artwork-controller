using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;

public class LineTrailController : MonoBehaviour, INetworkEndpoint, IArtworkController
{
    [SerializeField] private int _trailLength = 10;
    [SerializeField] private bool _isEnabled = false;
    [SerializeField] private float _startWidth = 0.0f;
    [SerializeField] private float _endWidth = 0.1f;

    private List<LineTrail> _lineTrails = new List<LineTrail>();
    private Material _lineMaterial;

    public Artwork Artwork => GetComponent<Artwork>();
    public string Address => $"/line";


    void OnEnable()
    {
        // Register();
        _lineMaterial = Resources.Load<Material>("Materials/LineMaterial");
        _lineTrails.Clear();
        // Artwork.ForeachMotif((motif) => {
        //     var lineTrail = motif.gameObject.GetComponent<LineTrail>();
        //     if (lineTrail == null) {
        //         lineTrail = motif.gameObject.AddComponent<LineTrail>();
        //     }
        //     lineTrail.Initialize(_trailLength, _endWidth, _startWidth, _lineMaterial);
        //     _lineTrails.Add(lineTrail);
        // });
    }

    void OnDisable()
    {
        Unregister();
    }

    public void Register(string baseAddress) {
        OscManager.Instance.AddEndpoint($"{baseAddress}/line/toggle", (OscDataHandle dataHandle) => {
            _isEnabled = !_isEnabled;
        }, this);

        OscManager.Instance.AddEndpoint($"{baseAddress}/line/length", (OscDataHandle dataHandle) => {
            var length = dataHandle.GetElementAsInt(0);
            if (length <= 0) {
                if (_isEnabled) DisableLineTrails();
                return;
            } 

            if (!_isEnabled) EnableLineTrails();
            _lineTrails.ForEach(trail => trail.SetLineCount(length));
        }, this);

        // add a spider-web effect
    }

    public void Unregister()
    {
        if (OscManager.Instance == null) return;
        OscManager.Instance.RemoveAllEndpointsForOwner(this);
        // OscManager.Instance.RemoveEndpoint($"{Address}/toggle");
        // OscManager.Instance.RemoveEndpoint($"{Address}/length");
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

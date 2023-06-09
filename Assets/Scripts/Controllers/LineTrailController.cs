using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;

public class LineTrailController : MonoBehaviour, IOscControllable, IArtworkController
{
    [SerializeField] private int _trailLength = 10;
    public int TrailLength { get => _trailLength; set => _trailLength = value; }

    public float startWidth = 0.0f;
    public float endWidth = 0.1f;

    public Material lineMaterial;

    private List<LineTrail> _lineTrails = new List<LineTrail>();

    public Artwork Artwork => GetComponent<Artwork>();
    public string OscAddress => $"/artwork/{Artwork.Id}/line";

    // private void OnValidate()
    // {
    //     // This method will be called whenever 'myValue' is modified in the inspector
        
    //     // Add your desired response or logic here

    //     foreach(var lineTrail in _lineTrails)
    //     {
    //         lineTrail.SetLineCount(_trailLength);
    //     }
    // }

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in transform)
        {
            if (child.gameObject.activeSelf == false) continue;
            LineTrail lineTrail = null;
            if (child.GetComponent<LineTrail>() == null)
            {
                lineTrail = child.gameObject.AddComponent<LineTrail>();
            }

            if (lineTrail != null)
            {
                lineTrail.Initialize(_trailLength, endWidth, startWidth, lineMaterial);
                _lineTrails.Add(lineTrail);
            }
        }

        RegisterEndpoints();
    }

    public void RegisterEndpoints() {
        OscManager.Instance.AddEndpoint($"{OscAddress}/toggle", (OscDataHandle dataHandle) => {
            foreach(var trail in _lineTrails) 
            {
                trail.Toggle();
            }
            // Artwork.BackgroundFade(fade);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

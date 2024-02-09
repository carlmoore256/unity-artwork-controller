using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

// we make a router object, that can route to additional places

// maybe the artwork itself is an endpoint
// when we create endpoints for something, we pass in an object maybe?
// we could pass in some children objects that implement a certain


// consider creating an abstract factory to create artworks

[RequireComponent(typeof(FadeTransitionInsert))]
// [RequireComponent(typeof(ArtworkColorController))]
public class SegmentedPaintingArtwork
    : BaseArtwork,
        IComponentIterator,
        IMaskLayerIterator,
        IMovableIterator,
        IMotifIterator,
        INetworkEndpoint
{
    [SerializeField]
    private int _index;
    public int Index
    {
        get => _index;
        set => _index = value;
    }

    // public ITransitionInsert TransitionInsert { get; private set; }

    public string Address => $"/artwork/{Id}";

    // public string Id => gameObject.name.Split("Artwork__")[1];  

    // public string Id
    // {
    //     get
    //     {
    //         if (string.IsNullOrEmpty(_id))
    //         {
    //             _id = gameObject.name.Split("Artwork__")[1];
    //         }
    //         return _id;
    //     }
    // }

    // public override string Name => gameObject.name.Split("Artwork__")[1];

    // this is all any controller will ever need to access
    public List<Motif> Motifs = new List<Motif>();

    private List<Moveable> _allMoveables = new List<Moveable>();
    private List<MaskLayer> _allMaskLayers = new List<MaskLayer>();

    public IEnumerable<MaskLayer> AllMaskLayers => _allMaskLayers;

    public IEnumerable<Moveable> AllMoveables => _allMoveables;

    // public IEnumerable<Moveable> AllMoveables => Motifs.SelectMany(m => m.Moveables); // call these less as they are more expensive

    void Awake()
    {
        gameObject.name = gameObject.name.Replace("(Clone)", "");
        if (!gameObject.name.StartsWith("Artwork__"))
        {
            gameObject.name = $"Artwork__{gameObject.name}";
        }
    }

    // public ArtworkMetadata GetMetadata()
    // {
    //     return new ArtworkMetadata { id = Id, name = Id, };
    // }

    private void OnEnable()
    {
        Debug.Log($"Artwork {gameObject.name} enabled");
        InitializeMotifs();
        AddArtworkControllers();
        if (!gameObject.name.Contains("Artwork__"))
        {
            gameObject.name = $"Artwork__{gameObject.name}";
        }
        // var colorController = GetComponent<ArtworkColorController>();
        // colorController.ResetToTransparent();
        // colorController.FadeInEffect();

        Register($"artwork/{Id}");

        // TransitionInsert = GetComponent<FadeTransitionInsert>();
        // TransitionInsert.Initialize();
        // TransitionInsert.TransitionIn(2f);
    }

    // void Update()
    // {
    //     if (TransitionInsert != null)
    //     {
    //         Debug.Log($"TransitionInsert.PercentComplete {TransitionInsert.PercentComplete}");
    //     }
    // }

    private void OnDisable()
    {
        Unregister();
    }

    private IEnumerable<INetworkEndpoint> GetEndpoints()
    {
        return GetComponents<INetworkEndpoint>().Where(e => (object)e != this);
    }

    public void Register(string baseAddress)
    {
        foreach (var endpoint in GetEndpoints())
        {
            Debug.Log($"Registering endpoint {endpoint.Address}");
            endpoint.Register(baseAddress);
        }
    }

    public void Unregister()
    {
        foreach (var endpoint in GetEndpoints())
        {
            endpoint.Unregister();
        }
    }

    // public void Destroy()
    // {
    //     Destroy(gameObject);
    // }

    // public IInsert[] GetInserts()
    // {
    //     return GetComponents<IInsert>();
    // }

    /// <summary>
    /// Add motif monobehaviours on to all direct descendents
    /// </summary>
    private void InitializeMotifs()
    {
        foreach (Transform child in transform)
        {
            if (
                child.gameObject.CompareTag("backgroundPrimary")
                || child.gameObject.CompareTag("backgroundSecondary")
            )
            {
                Debug.Log("Skipping background", child.gameObject);
                continue;
            }

            if (child.gameObject.GetComponentsInChildren<MaskLayer>().Length == 0)
            {
                Debug.Log("Skipping non-mask layer", child.gameObject);
                continue;
            }
            var motif = child.GetComponent<Motif>();
            if (motif == null)
                motif = child.gameObject.AddComponent<Motif>();
            if (!child.gameObject.activeSelf)
                continue;
            Motifs.Add(motif);

            // motif.ForeachMaskLayer(ml => {
            //     ml.SetOpacity(0f);
            // });
        }
        _allMoveables = Motifs.SelectMany(m => m.Moveables).ToList();
        _allMaskLayers = Motifs.SelectMany(m => m.MaskLayers).ToList();
    }

    public List<Motif> GetNearbyMotifs(Motif motif, int maxNum, float distanceThresh)
    {
        var nearbyMotifs = new List<Motif>();
        foreach (var otherMotif in Motifs)
        {
            if (otherMotif == motif)
                continue;
            if (
                Vector3.Distance(motif.transform.position, otherMotif.transform.position)
                < distanceThresh
            )
            {
                nearbyMotifs.Add(otherMotif);
                if (nearbyMotifs.Count >= maxNum)
                    break;
            }
        }
        return nearbyMotifs;
    }

    /// Preferred API for interacting with pieces of the artwork, since it allows
    /// motif objects to determine what happens with sub-pieces

    # region IMotifIterator

    public int NumMotifs => Motifs.Count;

    public void ForeachMotif(Action<Motif> action)
    {
        foreach (var motif in Motifs)
        {
            action(motif);
        }
    }

    public void ForeachMotif(Action<Motif, float> action)
    {
        for (int i = 0; i < Motifs.Count; i++)
        {
            action(Motifs[i], (float)i / Motifs.Count);
        }
    }

    public Motif GetMotifAtNormalizedIndex(float normalizedIndex)
    {
        if (Motifs.Count == 0)
            return null;
        normalizedIndex = Mathf.Clamp01(normalizedIndex);
        Debug.Log("normalizedIndex" + normalizedIndex + " Number of Motifs " + Motifs.Count);
        var index = Mathf.FloorToInt(normalizedIndex * Motifs.Count);
        if (index == Motifs.Count)
            index = Motifs.Count - 1;
        if (index < 0)
            index = 0;
        return Motifs.ElementAt(Mathf.FloorToInt(normalizedIndex * Motifs.Count));
    }

    # endregion

    /// Call these iterators if you want to get fine grained control over single
    /// pieces within the artwork, rather than delegating a task to a motif

    # region IComponentIterator

    public void ForeachComponent<T>(Action<T> action)
        where T : Component
    {
        foreach (var motif in Motifs)
        {
            foreach (var component in motif.GetComponentsInChildren<T>())
            {
                action(component);
            }
        }
    }

    public void ForeachComponent<T>(Action<T, float> action)
        where T : Component
    {
        var components = gameObject.GetComponentsInChildren<T>();
        for (int i = 0; i < components.Length; i++)
        {
            action(components[i], i / components.Length);
        }
    }

    public T GetComponentAtNormalizedIndex<T>(float normalizedIndex)
        where T : Component
    {
        normalizedIndex = Mathf.Clamp01(normalizedIndex);
        var index = Mathf.FloorToInt(normalizedIndex * Motifs.Count);
        var components = GetComponentsInChildren<T>();
        if (components.Count() == 0)
            return null;
        if (index == Motifs.Count)
            index = components.Count() - 1;
        if (index < 0)
            index = 0;
        return GetComponentsInChildren<T>()[
            Mathf.FloorToInt(normalizedIndex * GetComponentsInChildren<T>().Length)
        ];
    }

    # endregion

    # region IMaskLayerIterator

    public int NumMaskLayers => Motifs.Sum(m => m.NumMaskLayers);

    public void ForeachMaskLayer(Action<MaskLayer> action)
    {
        foreach (var motif in Motifs)
        {
            motif.ForeachMaskLayer(action);
        }
    }

    public void ForeachMaskLayer(Action<MaskLayer, float> action)
    {
        var totalLayers = NumMaskLayers;
        int currentIndex = 0;
        foreach (var motif in Motifs)
        {
            foreach (var layer in motif.MaskLayers)
            {
                action(layer, (float)currentIndex / totalLayers);
                currentIndex++;
            }
        }
    }

    public MaskLayer GetMaskLayerAtNormalizedIndex(float normalizedIndex)
    {
        if (AllMaskLayers.Count() == 0)
            return null;
        normalizedIndex = Mathf.Clamp01(normalizedIndex);
        var index = Mathf.FloorToInt(normalizedIndex * AllMaskLayers.Count());
        if (index == AllMaskLayers.Count())
            index = AllMaskLayers.Count() - 1;
        return AllMaskLayers.ElementAt(index);
    }

    # endregion

    # region IMovableIterator

    public int NumMoveables => Motifs.Sum(m => m.NumMoveables);

    public void ForeachMoveable(Action<Moveable> action)
    {
        foreach (var motif in Motifs)
        {
            motif.ForeachMoveable(action);
        }
    }

    public void ForeachMoveable(Action<Moveable, float> action)
    {
        var totalMoveables = NumMoveables;
        int currentIndex = 0;
        foreach (var motif in Motifs)
        {
            foreach (var moveable in motif.Moveables)
            {
                action(moveable, (float)currentIndex / totalMoveables);
                currentIndex++;
            }
        }
    }

    public Moveable GetMoveableAtNormalizedIndex(float normalizedIndex)
    {
        if (AllMoveables.Count() == 0)
            return null;
        normalizedIndex = Mathf.Clamp01(normalizedIndex);
        var index = Mathf.FloorToInt(normalizedIndex * AllMoveables.Count());
        if (index == AllMoveables.Count())
            index = AllMoveables.Count() - 1;
        if (index < 0)
            index = 0;
        return AllMoveables.ElementAt(index);
    }

    # endregion


    public void AddArtworkControllers()
    {
        // instead of this, we should just get all objects that have a type of controller
        // these are essentially things that just expose an endpoint

        // add all the monobehaviours
        if (gameObject.GetComponent<PolyphonicMidiController>() == null)
            gameObject.AddComponent<PolyphonicMidiController>();

        // if (gameObject.GetComponent<ArtworkColorController>() == null)
        //     gameObject.AddComponent<ArtworkColorController>();

        /// if it has artworkColorcontroller, remove it
        ///
        if (gameObject.GetComponent<ArtworkColorController>() != null)
            Destroy(gameObject.GetComponent<ArtworkColorController>());

        if (gameObject.GetComponent<LineTrailController>() == null)
            gameObject.AddComponent<LineTrailController>();

        if (gameObject.GetComponent<SpritePhysicsController>() == null)
            gameObject.AddComponent<SpritePhysicsController>();

        gameObject.GetOrAddComponent<SinusoidalMotionInsert>();
        gameObject.GetOrAddComponent<RandomMotionInsert>();
        gameObject.GetOrAddComponent<MotifColorInsert>();
        gameObject.GetOrAddComponent<PhysicsInsert>();
        gameObject.GetOrAddComponent<InsertParameterRouter>();
    }
}

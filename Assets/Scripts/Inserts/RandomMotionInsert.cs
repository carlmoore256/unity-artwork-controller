using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "RandomMotionInsertParameters",
    menuName = "Inserts/RandomMotionParameters",
    order = 1
)]
public class RandomMotionInsertParameters : InsertParameters
{
    public ParameterValue<bool> enable = new(false);
    public RangedParameterValue randProb = new(0.5f, 0f, 1f, "Random probability");
    public RangedParameterValue randDist = new(0.5f, 0f, 10f, "Random distance");
}

public class RandomMotionInsert : MonoBehaviour, IInsert
{
    public string Id
    {
        get
        {
            if (string.IsNullOrEmpty(_id))
            {
                _id = Guid.NewGuid().ToString();
            }
            return _id;
        }
    }

    private string _id;
    public string Name => "Random Motion";

    private RandomMotionInsertParameters _parameters = new RandomMotionInsertParameters();

    private IMovableIterator _movableIterator;

    private void Awake()
    {
        _movableIterator = GetComponent<IMovableIterator>();
    }

    public InsertParameters GetParameters()
    {
        return _parameters;
    }

    // public void UpdateParameters(string json)
    // {
    //     // _parameters = parameters;
    // }

    void ApplyMotion(Moveable moveable, float normIndex)
    {
        if (UnityEngine.Random.Range(0f, 1f) > _parameters.randProb)
            return;
        var updatePos = new Vector3(
            UnityEngine.Random.Range(-1f, 1f) * _parameters.randDist,
            UnityEngine.Random.Range(-1f, 1f) * _parameters.randDist,
            UnityEngine.Random.Range(-1f, 1f) * _parameters.randDist
        );
        moveable.AddPosition(updatePos);
    }

    void Update()
    {
        if (_parameters.enable)
            _movableIterator.ForeachMoveable(ApplyMotion);
    }
}

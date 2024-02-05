using UnityEngine;

public interface IMovable {
    public TransformSnapshot CurrentSnapshot { get; }
    public TransformSnapshot AnchorSnapshot { get; }
    public TransformSnapshot TargetSnapshot { get; }

    public float BlendPercent { get; set; }   
}
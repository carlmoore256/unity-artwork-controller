using UnityEngine;

[System.Serializable]
public class TransformSnapshot {
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 Scale { get; set; }


    public TransformSnapshot(Vector3 position, Quaternion rotation, Vector3 scale) {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }

    public TransformSnapshot(Transform transform) {
        Position = transform.position;
        Rotation = transform.rotation;
        Scale = transform.localScale;
    }

    public TransformSnapshot(GameObject gameObject) : this(gameObject.transform) { }

    public TransformSnapshot Copy() {
        Vector3 newPosition = new Vector3(Position.x, Position.y, Position.z);
        Quaternion newRotation = new Quaternion(Rotation.x, Rotation.y, Rotation.z, Rotation.w);
        Vector3 newScale = new Vector3(Scale.x, Scale.y, Scale.z);
        return new TransformSnapshot(newPosition, newRotation, newScale);
    }

    public static TransformSnapshot Lerp(TransformSnapshot a, TransformSnapshot b, float t) {
        Vector3 newPosition = Vector3.Lerp(a.Position, b.Position, t);
        Quaternion newRotation = Quaternion.Slerp(a.Rotation, b.Rotation, t);
        Vector3 newScale = Vector3.Lerp(a.Scale, b.Scale, t);
        return new TransformSnapshot(newPosition, newRotation, newScale);
    }

}
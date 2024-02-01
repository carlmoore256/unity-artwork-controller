using UnityEngine;

public interface IVertexModifier
{
    public Vector3[] Modify(Vector3[] vertices, Vector3 position);
}

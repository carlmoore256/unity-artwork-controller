using UnityEngine;

[CreateAssetMenu(fileName = "PerlinModifier", menuName = "Procedural/PerlinModifier")]
public class PerlinModifier : ScriptableObject, IVertexModifier
{
    [SerializeField]
    private float _radius = 10f;

    [SerializeField]
    private float _height = 10f;

    [SerializeField]
    private float _density = 0.1f;

    [SerializeField]
    private Vector3 _noiseOffset = Vector3.zero;

    public Vector3[] Modify(Vector3[] vertices, Vector3 position)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 thisVertex = vertices[i] + position + _noiseOffset;

            var noise = Mathf.PerlinNoise(
                (thisVertex.x * _density * 0.1f) + _radius,
                (thisVertex.z * _density * 0.1f) + _radius
            );
            var thisHeight = noise * _height;

            var heightOctave = Mathf.PerlinNoise(
                (thisVertex.x * _density * thisHeight * 0.1f) + _radius,
                (thisVertex.z * _density * thisHeight * 0.1f) + _radius
            );

            thisHeight += heightOctave * thisHeight;

            vertices[i].y = thisHeight;
        }

        return vertices;
    }
}

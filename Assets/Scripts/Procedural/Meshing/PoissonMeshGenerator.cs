using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Topology;
using UnityEngine;

public class PoissonMeshGenerator : IMeshGenerator
{
    private Vector2 chunkSize;
    private float radius;
    private float boundaryPointSpacing;
    private int numSamplesBeforeRejection;

    public PoissonMeshGenerator(
        Vector2 chunkSize,
        float radius,
        float boundaryPointSpacing,
        int numSamplesBeforeRejection = 30
    )
    {
        this.chunkSize = chunkSize;
        this.radius = radius;
        this.boundaryPointSpacing = boundaryPointSpacing;
        this.numSamplesBeforeRejection = numSamplesBeforeRejection;
    }

    public (Vector3[] vertices, int[] triangles, Vector2[] uvs) Generate()
    {
        List<Vector2> boundaryPoints = GenerateBoundaryPoints(chunkSize, boundaryPointSpacing);
        List<Vector2> interiorPoints = GenerateInteriorPoints(
            chunkSize,
            radius,
            boundaryPointSpacing
        );
        List<Vector2> allPoints = new List<Vector2>(boundaryPoints);
        allPoints.AddRange(interiorPoints);

        // Convert 2D points to 3D vertices
        List<Vector3> vertices = new List<Vector3>();
        foreach (var point in allPoints)
        {
            vertices.Add(new Vector3(point.x, 0, point.y));
        }

        // Triangulate using the combined points
        int[] triangles = TriangulatePoints(allPoints);

        // reverse the triangles
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int temp = triangles[i];
            triangles[i] = triangles[i + 2];
            triangles[i + 2] = temp;
        }

        // Generate UVs (simplified example)
        Vector2[] uvs = new Vector2[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            uvs[i] = new Vector2(vertices[i].x / chunkSize.x, vertices[i].z / chunkSize.y);
        }

        return (vertices.ToArray(), triangles, uvs);
    }

    // Placeholder for triangulation
    private int[] TriangulatePoints(List<Vector2> points)
    {
        Polygon polygon = new Polygon();

        foreach (var point in points)
        {
            polygon.Add(new Vertex(point.x, point.y));
        }

        var mesh = polygon.Triangulate();
        List<int> triangles = new List<int>();

        foreach (Triangle triangle in mesh.Triangles)
        {
            for (int j = 0; j < 3; j++)
            {
                triangles.Add(triangle.GetVertex(j).ID);
            }
        }

        return triangles.ToArray();
    }

    public List<Vector2> GenerateInteriorPoints(
        Vector2 chunkSize,
        float radius,
        float boundaryPointSpacing
    )
    {
        Vector2 adjustedSize =
            chunkSize - new Vector2(boundaryPointSpacing * 2, boundaryPointSpacing * 2);
        Vector2 sampleRegionStart = new Vector2(boundaryPointSpacing, boundaryPointSpacing);

        var interiorPoints = PoissonDiskSampling.GeneratePoints(radius, adjustedSize);

        // Offset points to account for the boundary
        for (int i = 0; i < interiorPoints.Count; i++)
        {
            interiorPoints[i] += sampleRegionStart;
        }

        return interiorPoints;
    }

    public List<Vector2> GenerateBoundaryPoints(Vector2 chunkSize, float boundaryPointSpacing)
    {
        List<Vector2> boundaryPoints = new List<Vector2>();

        // Bottom and top edges
        for (float x = 0; x <= chunkSize.x; x += boundaryPointSpacing)
        {
            boundaryPoints.Add(new Vector2(x, 0));
            boundaryPoints.Add(new Vector2(x, chunkSize.y));
        }

        // Left and right edges (excluding corners already added)
        for (
            float y = boundaryPointSpacing;
            y <= chunkSize.y - boundaryPointSpacing;
            y += boundaryPointSpacing
        )
        {
            boundaryPoints.Add(new Vector2(0, y));
            boundaryPoints.Add(new Vector2(chunkSize.x, y));
        }

        return boundaryPoints;
    }

    public List<Vector2> GenerateChunkPoints(
        Vector2 chunkSize,
        float radius,
        float boundaryPointSpacing
    )
    {
        List<Vector2> boundaryPoints = GenerateBoundaryPoints(chunkSize, boundaryPointSpacing);
        List<Vector2> interiorPoints = GenerateInteriorPoints(
            chunkSize,
            radius,
            boundaryPointSpacing
        );

        List<Vector2> allPoints = new List<Vector2>();
        allPoints.AddRange(boundaryPoints);
        allPoints.AddRange(interiorPoints);

        return allPoints;
    }
}

using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralCylinder : MonoBehaviour {
    [Header("Cylinder Settings")]
    public int radialSegments = 20;

    float height = 2f;
    float radius = 1f;
    Mesh mesh;
    Vector3[] vertices;
    Vector2[] uvs;

    void Awake() {
        GenerateCylinder();
    }

    public void GenerateCylinder() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        int vertCount = radialSegments * 2;
        vertices = new Vector3[vertCount];
        uvs = new Vector2[vertCount];
        int[] triangles = new int[radialSegments * 6]; // 2 triangles per segment

        float angleStep = 2 * Mathf.PI / radialSegments;

        for(int i = 0; i < radialSegments; i++) {
            float angle = i * angleStep;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            // Bottom ring
            vertices[i] = new Vector3(x, 0, z);
            uvs[i] = new Vector2((float)i / radialSegments, 0);

            // Top ring
            vertices[i + radialSegments] = new Vector3(x, height, z);
            uvs[i + radialSegments] = new Vector2((float)i / radialSegments, 1);
        }

        int tri = 0;
        for(int i = 0; i < radialSegments; i++) {
            int next = (i + 1) % radialSegments;
            int bottomA = i;
            int bottomB = next;
            int topA = i + radialSegments;
            int topB = next + radialSegments;

            // Triangle 1
            triangles[tri++] = bottomA;
            triangles[tri++] = topA;
            triangles[tri++] = topB;

            // Triangle 2
            triangles[tri++] = bottomA;
            triangles[tri++] = topB;
            triangles[tri++] = bottomB;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }

    public Vector3[] GetVertices() => vertices;


    Vector3 CalculatePointOnCircle(float angle, float radius) {
        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
    }

    Vector3 CalculatePointOnEllipse(float angle, float minorRadius, float majorRadius) {
        float t = 1f / Mathf.Sqrt(
                (Mathf.Cos(angle) * Mathf.Cos(angle)) / (majorRadius * majorRadius) +
                (Mathf.Sin(angle) * Mathf.Sin(angle)) / (minorRadius * minorRadius)
            );
        return transform.position + t * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
    }

    Vector3 CalculateProjectedPoint(Vector3 point, float minorRadius, float majorRadius) {
        float x = point.x;

        float xRatioSquared = (x * x) / (majorRadius * majorRadius);

        float z = minorRadius * Mathf.Sqrt(1f - xRatioSquared);
        if(point.z < 0) z *= -1;

        return new Vector3(x, 0, z);
    }

    public void UpdateShape(float minorRadius, float circleRadius, float separationAngle, float height) {
        float angleStep = 2 * Mathf.PI / radialSegments;

        for(int i = 0; i < radialSegments; i++) {
            float angle = i * angleStep;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            Vector3 ellipsePoint = CalculatePointOnEllipse(angle, minorRadius, circleRadius);
            Vector3 circlePoint = CalculatePointOnCircle(angle + separationAngle, circleRadius);
            Vector3 projectedPoint = CalculateProjectedPoint(circlePoint, minorRadius, circleRadius);
            Vector3 perpendicularPoint = projectedPoint + Vector3.up * height;

            // Bottom ring
            vertices[i] = ellipsePoint;

            // Top ring
            vertices[i + radialSegments] = perpendicularPoint;
        }


        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}

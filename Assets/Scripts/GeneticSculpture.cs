using UnityEngine;

public class GeneticSculpture : MonoBehaviour
{
    public int numSculptures = 10;
    public Material sculptureMaterial;
    public int sculpturesPerRow = 4;
    public float distanceBetweenSculptures = 2.0f;
    public float pastelSaturation = 0.5f;
    public float pastelValue = 0.9f;
    public float polynomialNoiseScale = 0.1f;

    void Start()
    {
        for (int i = 0; i < numSculptures; i++)
        {
            // Calculate the row and column of the current sculpture
            int row = i / sculpturesPerRow;
            int column = i % sculpturesPerRow;

            // Create a new empty mesh
            Mesh mesh = new Mesh();

            // Define the number of vertices
            int numVertices = Random.Range(10, 20);
            Vector3[] vertices = new Vector3[numVertices];

            // Define the vertices of the mesh
            for (int j = 0; j < numVertices; j++)
            {
                float x = Random.Range(-1f, 1f);
                float y = Random.Range(-1f, 1f);
                float z = CalculateHeight(x, y);
                vertices[j] = new Vector3(x, y, z);
            }

            // Define the number of triangles
            int numTriangles = Random.Range(numVertices - 2, numVertices * 2);
            int[] triangles = new int[numTriangles * 3];

            // Define the triangles of the mesh
            int triangleIndex = 0;
            int vertexIndex = 0;
            while (vertexIndex < numVertices - 2 && triangleIndex < numTriangles)
            {
                triangles[triangleIndex * 3] = vertexIndex;
                triangles[triangleIndex * 3 + 1] = vertexIndex + 1;
                triangles[triangleIndex * 3 + 2] = vertexIndex + 2;
                vertexIndex++;
                triangleIndex++;
            }
            while (triangleIndex < numTriangles)
            {
                int vertex1 = Random.Range(0, numVertices);
                int vertex2 = Random.Range(0, numVertices);
                int vertex3 = Random.Range(0, numVertices);
                if (vertex1 != vertex2 && vertex1 != vertex3 && vertex2 != vertex3)
                {
                    triangles[triangleIndex * 3] = vertex1;
                    triangles[triangleIndex * 3 + 1] = vertex2;
                    triangles[triangleIndex * 3 + 2] = vertex3;
                    triangleIndex++;
                }
            }

            // Assign the vertices and triangles to the mesh
            mesh.vertices = vertices;
            mesh.triangles = triangles;

            // Recalculate the normals of the mesh
            mesh.RecalculateNormals();

            // Create a new material with a random pastel color
            Material newMaterial = new Material(sculptureMaterial);
            newMaterial.color = RandomPastelColor();

            // Assign the material to a new game object
            GameObject meshObject = new GameObject("Sculpture " + i);
            meshObject.AddComponent<MeshFilter>().mesh = mesh;
            meshObject.AddComponent<MeshRenderer>().material = newMaterial;
            // Position the sculpture in the grid
            float xPosition = column * distanceBetweenSculptures;
            float yPosition = row * distanceBetweenSculptures;
            meshObject.transform.position = new Vector3(xPosition, yPosition, 0f);
        }
    }

    // Returns a random pastel color
    Color RandomPastelColor()
    {
        float hue = Random.Range(-0.1f, 0.2f);
        float saturation = pastelSaturation + Random.Range(-0.5f, 0.2f);
        float value = pastelValue + Random.Range(-0.3f, 0.2f);
        return Color.HSVToRGB(hue, saturation, value);
    }

    // Calculates the height of the point (x, y) using a fifth-order polynomial function with added noise
    float CalculateHeight(float x, float y)
    {
        float noise = Mathf.PerlinNoise(x * polynomialNoiseScale, y * polynomialNoiseScale);
        float a = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);
        float c = Random.Range(0f, 1f);
        float d = Random.Range(0f, 1f);
        float e = Random.Range(0f, 1f);
        float f = Random.Range(0f, 1f);
        return a * Mathf.Pow(x, 5) + b * Mathf.Pow(y, 5) + c * Mathf.Pow(x, 4) * Mathf.Pow(y, 1) + d * Mathf.Pow(x, 3) * Mathf.Pow(y, 2) + e * Mathf.Pow(x, 2) * Mathf.Pow(y, 3) + f * Mathf.Pow(x, 1) * Mathf.Pow(y, 4) + noise * 0.5f;
    }
}

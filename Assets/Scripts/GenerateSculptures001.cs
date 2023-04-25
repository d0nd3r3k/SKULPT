using UnityEngine;

public class GenerateSculptures001 : MonoBehaviour
{
    public int numSculptures = 10;
    public Material sculptureMaterial;
    public int sculpturesPerRow = 4;
    public float distanceBetweenSculptures = 2.0f;
    public float mutationRate = 0.1f;
    public int numGenerations = 10;
    public int populationSize = 10;

    private Sculpture[] population;

    public float pastelSaturation = 0.5f;
    public float pastelValue = 0.9f;

    void Start()
    {
        // Set the random seed
        Random.InitState(System.DateTime.Now.Millisecond);

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
                vertices[j] = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
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

            // Create a new material with a random poppy color
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


        // Returns a random poppy color
        Color RandomPastelColor()
        {
            float hue = Random.Range(-0.1f, 0.2f);
            float saturation = pastelSaturation + Random.Range(-0.5f, 0.2f);
            float value = pastelValue + Random.Range(-0.3f, 0.2f);
            return Color.HSVToRGB(hue, saturation, value);
        }
    }


    // Select a parent by roulette wheel selection
    Sculpture SelectParentByRouletteWheelSelection()
    {
        float totalFitness = 0f;
        foreach (Sculpture sculpture in population)
        {
            totalFitness += sculpture.fitness;
        }
        float randomValue = Random.Range(0f, totalFitness);
        float currentFitness = 0f;
        foreach (Sculpture sculpture in population)
        {
            currentFitness += sculpture.fitness;
            if (currentFitness >= randomValue)
            {
                return sculpture;
            }
        }
        return population[populationSize - 1];
    }

    // Represents a sculpture with a set of vertices, triangles, and color
    class Sculpture : System.IComparable<Sculpture>
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Color color;
        public float fitness;

        // Returns a random poppy color
        Color RandomPoppyColor()
        {
            float hue = Random.Range(0.9f, 1.0f);
            float saturation = Random.Range(0.7f, 1.0f);
            float value = Random.Range(0.7f, 1.0f);
            return Color.HSVToRGB(hue, saturation, value);
        }

        public Sculpture(int numVertices, float x, float y, float z)
        {
            // Define the vertices of the mesh
            vertices = new Vector3[numVertices];
            for (int i = 0; i < numVertices; i++)
            {
                vertices[i] = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }

            // Define the triangles of the mesh
            int numTriangles = Random.Range(numVertices - 2, numVertices * 2);
            triangles = new int[numTriangles * 3];
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

            // Define the color of the mesh
            color = RandomPoppyColor();

            // Calculate the fitness of the sculpture
            EvaluateFitness();
        }

        // Crossover with another sculpture to create a child
        public Sculpture Crossover(Sculpture other)
        {
            int numVertices = vertices.Length;
            Sculpture child = new Sculpture(numVertices, 0f, 0f, 0f);
            if (other.vertices.Length != numVertices)
            {
                Debug.LogError("Vertices array length mismatch in crossover!");
                return child;
            }
            for (int i = 0; i < numVertices; i++)
            {
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    child.vertices[i] = vertices[i];
                }
                else
                {
                    child.vertices[i] = other.vertices[i];
                }
            }
            if (Random.Range(0f, 1f) < 0.5f)
            {
                child.color = color;
            }
            else
            {
                child.color = other.color;
            }
            child.EvaluateFitness();
            return child;
        }

        // Mutate the sculpture by randomly adjusting its vertices and color
        public void Mutate(float mutationRate)
        {
            int numVertices = vertices.Length;
            for (int i = 0; i < numVertices; i++)
            {
                if (Random.Range(0f, 1f) < mutationRate)
                {
                    vertices[i] = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                }
            }
            if (Random.Range(0f, 1f) < mutationRate)
            {
                color = RandomPoppyColor();
            }
            EvaluateFitness();
        }

        // Evaluate the fitness of the sculpture based on its color
        public void EvaluateFitness()
        {
            float red = color.r;
            float green = color.g;
            float blue = color.b;
            float total = red + green + blue;
            float redRatio = red / total;
            float greenRatio = green / total;
            float blueRatio = blue / total;
            fitness = Mathf.Min(redRatio, greenRatio, blueRatio);
        }

        // Compare sculptures based on their fitness
        public int CompareTo(Sculpture other)
        {
            return other.fitness.CompareTo(fitness);
        }
    }

   
}
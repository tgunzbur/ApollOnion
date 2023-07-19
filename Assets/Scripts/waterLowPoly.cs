using UnityEngine;
using System.Collections; 
using System.Collections.Generic;

public class waterLowPoly : MonoBehaviour {

    public float    size;
    public float    gridSize;

	public float    waveFrequency = 0.53f;
	public float    waveHeight = 0.48f;
	public float    waveLength = 0.71f;
    
    private Vector2 offSet;
    private MeshFilter meshFilter;

	private void Start () {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = GenerateMesh();
        offSet = new Vector2(Random.value , Random.value);
        MakeNoise();
	}

    private void Update () { 
		MakeNoise();
        offSet += new Vector2(Time.deltaTime * waveFrequency, Time.deltaTime * waveFrequency);
	}

    private Mesh GenerateMesh() {
        Mesh m = new Mesh();

        List<Vector3> verticies = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        for (int x = 0; x < gridSize + 1; x++) {
            for (int y = 0; y < gridSize + 1; y++) {
                verticies.Add(new Vector3(-size * 0.5f + size * (x / gridSize), 0, -size * 0.5f + size * (y / gridSize)));
                normals.Add(Vector3.up);
                uvs.Add(new Vector2(x / gridSize, y / gridSize));
            }
        }

        List<int> triangles = new List<int>();
        int vertCount = (int)gridSize + 1;

        for (int count = 0; count < vertCount * vertCount - vertCount; count++) {
            if ((count + 1) % vertCount == 0) {
                continue;
            }
            triangles.AddRange(new List<int>() {
                count + 1 + vertCount,
                count + vertCount, count,
                count,
                count + 1,
                count + 1 + vertCount
            });
        }

        m.SetVertices(verticies);
        m.SetNormals(normals);
        m.SetUVs(0, uvs);
        m.SetTriangles(triangles, 0);

        return m;
    }

	private void MakeNoise () {
        Vector3[] verticies = meshFilter.mesh.vertices;

		for (int count = 0; count < verticies.Length; count++) {
			verticies[count].y = CalculateHeight(verticies[count].x, verticies[count].z) * waveHeight;
        }

        meshFilter.mesh.vertices = verticies;
	}

    private float CalculateHeight(float x, float y) {
        return Mathf.PerlinNoise(x / gridSize * waveLength + offSet.x, y / gridSize * waveLength + offSet.y);
    }
}
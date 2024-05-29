using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MarchingCubes : MonoBehaviour
{
    [SerializeField][Range(5, 50)] private int m_Width = 10;
    [SerializeField][Range(5, 50)] private int m_Height = 10;

    [SerializeField][Range(0, 1)] private float m_HeightThreshold = 0.5f;

    [SerializeField][Range(0, 1)] private float m_NoiseResolution = 1.0f;

    [SerializeField] private bool m_VisualizeNoise = true;

    private float[,,] m_Heights;

    private List<Vector3> m_Vertices = new List<Vector3>();
    private List<int> m_Triangles = new List<int>();

    private MeshFilter m_MeshFilter;

    void Start()
    {
        m_MeshFilter = GetComponent<MeshFilter>();
        StartCoroutine(UpdateAll());
    }

    private IEnumerator UpdateAll()
    {
        while (true)
        {
            SetHeights();
            MarchCubes();
            SetMesh();
            yield return new WaitForSeconds(1);
        }
    }

    private void MarchCubes()
    {
        m_Vertices.Clear();
        m_Triangles.Clear();

        for (int x = 0; x < m_Width; x++)
        {
            for (int y = 0; y < m_Height; y++)
            {
                for (int z = 0; z < m_Width; z++)
                {
                    float[] cubeCorners = new float[8];

                    for (int i = 0; i < 8; i++)
                    {
                        Vector3Int corner = new Vector3Int(x, y, z) + MarchingCubesTables.Corners[i];
                        cubeCorners[i] = m_Heights[corner.x, corner.y, corner.z];
                    }

                    MarchCube(new Vector3(x, y, z), GetConfigurationIndex(cubeCorners));
                }
            }
        }
    }

    private void MarchCube(Vector3 position, int configurationIndex)
    {
        if (configurationIndex == 0 || configurationIndex == 255)
        {
            return;
        }

        int edgeIndex = 0;

        for (int t = 0; t < 5; t++)
        {
            for (int v = 0; v < 3; v++)
            {
                int triangleTableValue = MarchingCubesTables.Triangles[configurationIndex, edgeIndex];

                if (triangleTableValue == -1)
                {
                    return;
                }

                Vector3 edgeStart = position + MarchingCubesTables.Edges[triangleTableValue, 0];
                Vector3 edgeEnd = position + MarchingCubesTables.Edges[triangleTableValue, 1];

                Vector3 vertex = (edgeStart + edgeEnd) / 2;

                m_Vertices.Add(vertex);
                m_Triangles.Add(m_Vertices.Count - 1);

                edgeIndex++;
            }
        }
    }

    private int GetConfigurationIndex(float[] cubeCorners)
    {
        int configurationIndex = 0;

        for (int i = 0; i < 8; i++)
        {
            if (cubeCorners[i] > m_HeightThreshold)
            {
                configurationIndex |= 1 << i;
            }
        }

        return configurationIndex;
    }

    private void SetHeights()
    {
        m_Heights = new float[m_Width + 1, m_Height + 1, m_Width + 1];

        for (int x = 0; x < m_Width + 1; x++)
        {
            for (int y = 0; y < m_Height + 1; y++)
            {
                for (int z = 0; z < m_Width + 1; z++)
                {
                    float currentHeight = m_Height * Mathf.PerlinNoise(x * m_NoiseResolution, z * m_NoiseResolution);
                    float newHeight;

                    if (y > currentHeight)
                    {
                        newHeight = y - currentHeight;
                    }
                    else
                    {
                        newHeight = currentHeight - y;
                    }

                    m_Heights[x, y, z] = newHeight;
                }
            }
        }
    }

    private void SetMesh()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = m_Vertices.ToArray();
        mesh.triangles = m_Triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        m_MeshFilter.mesh = mesh;
    }

    private void OnDrawGizmosSelected()
    {
        if (m_VisualizeNoise == false || Application.isPlaying == false)
        {
            return;
        }

        for (int x = 0; x < m_Width + 1; x++)
        {
            for (int y = 0; y < m_Height + 1; y++)
            {
                for (int z = 0; z < m_Width + 1; z++)
                {
                    Gizmos.color = new Color(m_Heights[x, y, z], m_Heights[x, y, z], m_Heights[x, y, z]);
                    Gizmos.DrawSphere(new Vector3(x, y, z), 0.2f);
                }
            }
        }
    }
}

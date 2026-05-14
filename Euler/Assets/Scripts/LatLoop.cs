using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatLoop : MonoBehaviour
{
    public int m_numAz = 90;
    public int m_numRound = 8;
    public float m_radius = 10.0f;
    public float m_width = 0.1f;

    MeshFilter m_meshFilter;
    Mesh m_mesh;
    List<Polar> m_polar;

    // Start is called before the first frame update
    void Start()
    {
        BuildPolar();
        BuildMesh();
    }

    void BuildPolar()
    {
        m_polar = new List<Polar>();

        for (int j = 0; j < m_numAz; ++j)
        {
            float az = 2.0f * Mathf.PI * j / m_numAz - Mathf.PI;
            m_polar.Add(new Polar(az, 0.0f, m_radius + m_width));
        }
    }

    void UpdateVerts(float howFlat = 0.0f)
    {
        List<Vector3> verts = new List<Vector3>();
        foreach (Polar polar in m_polar)
        {
            Quaternion quatAz = Quaternion.AngleAxis(Mathf.Rad2Deg * polar.m_az, Vector3.up);
            Vector3 centerPoint = polar.ConvertTheVert(m_radius, howFlat);
            for (int i = 0; i < m_numRound; i++)
            {
                float ang = -2.0f * Mathf.PI * i / m_numRound;
                Quaternion quatLoop = Quaternion.AngleAxis(Mathf.Rad2Deg * ang, Vector3.right);
                Vector3 p = new Vector3(0.0f, 0.0f, m_width);
                p = quatLoop * p;
                p = quatAz * p;
                verts.Add(p + centerPoint);
            }
        }
        m_mesh.vertices = verts.ToArray();
    }

    void BuildMesh()
    {
        if (null == m_mesh)
        {
            m_meshFilter = GetComponent<MeshFilter>();
            if (null == m_meshFilter)
            {
                Debug.LogError(name + " Has No MeshFilter");
                return;
            }

            m_mesh = new Mesh
            {
                name = "LatLoop"
            };
        }
        else
        {
            m_mesh.Clear();
        }

        UpdateVerts(0.0f);
        List<int> indices = new List<int>();

        int index = 0;
        for (int j = 0; j < m_numAz - 1; ++j)
        {
            for (int i = 0; i < m_numRound; i++)
            {
                indices.Add(index + i);
                indices.Add(index + m_numRound + i);
                indices.Add(index + (i + 1) % m_numRound);

                indices.Add(index + (i + 1) % m_numRound);
                indices.Add(index + m_numRound + i);
                indices.Add(index + m_numRound + (i + 1) % m_numRound);
            }
            index += m_numRound;
        }
        for (int i = 0; i < m_numRound; i++)
        {
            indices.Add(index + i);
            indices.Add(i);
            indices.Add(index + (i + 1) % m_numRound);

            indices.Add(index + (i + 1) % m_numRound);
            indices.Add(i);
            indices.Add((i + 1) % m_numRound);
        }
        m_mesh.triangles = indices.ToArray();
        m_mesh.RecalculateNormals();
        m_mesh.RecalculateBounds();
        m_meshFilter.mesh = m_mesh;
    }

    public void Update()
    {
    }
}

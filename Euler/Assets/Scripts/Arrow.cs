using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int m_numRound = 4;
    public int m_numLong = 2;
    public float m_length = 1.0f;
    public float m_arrowRatio = 0.2f;
    public float m_shaftWidth = 0.1f;
    public float m_arrowWidth = 0.2f;
    public float m_curve = 20.0f;   // in degrees

    MeshFilter m_meshFilter;
    Mesh m_mesh;

    // Start is called before the first frame update
    void Start()
    {
        BuildMesh();
    }

    public void BuildMesh()
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
                name = "Arrow"
            };
        }
        else
        {
            m_mesh.Clear();
        }

        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();

        // make the cone
        verts.Add(new Vector3(0.0f, 0.0f, m_length));
        float z = (1.0f - m_arrowRatio) * m_length;
        for (int i = 0; i < m_numRound; i++)
        {
            float ang = 2.0f * Mathf.PI * i / m_numRound;
            float x = m_arrowWidth * Mathf.Cos(ang);
            float y = m_arrowWidth * Mathf.Sin(ang);
            verts.Add(new Vector3(x, y, z));
        }
        int index = 1;
        for (int i = 0; i < m_numRound; i++)
        {
            indices.Add(0);
            indices.Add(i + index);
            indices.Add(index + (i + 1) % m_numRound);

            indices.Add(i + index);
            indices.Add(index + m_numRound + (i + 1) % m_numRound);
            indices.Add(index + (i + 1) % m_numRound);

            indices.Add(i + index);
            indices.Add(index + m_numRound + i);
            indices.Add(index + m_numRound + (i + 1) % m_numRound);
        }
        // shaft
        for (int j = 0; j <= m_numLong; j++)
        {
            for (int i = 0; i < m_numRound; i++)
            {
                float ang = 2.0f * Mathf.PI * i / m_numRound;
                float x = m_shaftWidth * Mathf.Cos(ang);
                float y = m_shaftWidth * Mathf.Sin(ang);
                verts.Add(new Vector3(x, y, z));
            }
            z -= (1.0f - m_arrowRatio) * m_length / m_numLong;
        }
        index = m_numRound + 1;
        for (int j = 0; j < m_numLong; ++j)
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
        // end cap
        verts.Add(Vector3.zero);
        int center = verts.Count - 1;
        for (int i = 0; i < m_numRound; ++i)
        {
            indices.Add(index + i);
            indices.Add(center);
            indices.Add(index + (i + 1) % m_numRound);
        }

        // curve
        if (Mathf.Abs(m_curve) > 0.01f)
        {
            float angle = Mathf.Deg2Rad * m_curve;
            float r = m_length / angle;
            for (int i = 0; i < verts.Count; ++i)
            {
                Vector3 vert = verts[i];
                Vector3 newVert = Vector3.zero;
                angle = vert.z / m_length * Mathf.Deg2Rad * m_curve;
                Vector3 centerPos = new Vector3(0.0f, r * (Mathf.Cos(angle) - 1.0f), r * Mathf.Sin(angle));
                newVert.x = vert.x;
                newVert.y = vert.y * Mathf.Cos(angle);
                newVert.z = vert.y * Mathf.Sin(angle);
                verts[i] = newVert + centerPos;
            }
        }

        m_mesh.vertices = verts.ToArray();
        m_mesh.triangles = indices.ToArray();
        m_mesh.RecalculateNormals();
        m_mesh.RecalculateBounds();
        m_meshFilter.mesh = m_mesh;
    }
}

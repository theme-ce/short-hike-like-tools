using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RiverGenerator : MonoBehaviour
{
    [SerializeField] private float m_width = 3;
    [SerializeField] private List<RiverPoint> m_riverPoints = new List<RiverPoint>();

    public List<RiverPoint> RiverPoints => m_riverPoints;

    private void Start()
    {
        UpdateRiverPointsList();
    }

    public void UpdateRiverPointsList()
    {
        // Check if any child objects have been added or removed
        Transform[] children = transform.GetComponentsInChildren<Transform>();
        List<RiverPoint> currentPoints = new List<RiverPoint>(m_riverPoints);

        foreach (Transform child in children)
        {
            if (child.gameObject.name.StartsWith("RiverPoint"))
            {
                RiverPoint point = child.gameObject.GetComponent<RiverPoint>();

                if (!currentPoints.Contains(point))
                {
                    // New river point added
                    m_riverPoints.Add(point);
                    GenerateRiverMesh();
                }
                else
                {
                    // Existing river point removed
                    currentPoints.Remove(point);
                }
            }
        }

        // Check if any river points have been removed
        foreach (RiverPoint point in currentPoints)
        {
            m_riverPoints.Remove(point);
            GenerateRiverMesh();
        }
    }

    public void GenerateRiverMesh()
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        float totalLength = 0;
        for (int i = 0; i < m_riverPoints.Count - 1; i++)
        {
            Vector3 a = m_riverPoints[i].Left - transform.position;
            Vector3 b = m_riverPoints[i].Right - transform.position;
            Vector3 c = m_riverPoints[i + 1].Left - transform.position;
            Vector3 d = m_riverPoints[i + 1].Right - transform.position;

            List<Vector3> v = new List<Vector3>() { a, c, b, b, c, d };

            for (int j = 0; j < v.Count; j++)
            {
                vertices.Add(v[j]);
                triangles.Add(triangles.Count);
            }

            float segmentLength = Vector3.Distance(a, c);
            totalLength += segmentLength;

            float v0 = totalLength - segmentLength;
            float v1 = totalLength;

            uvs.AddRange(new List<Vector2>()
        {
            new Vector2(0, v0),
            new Vector2(0, v1),
            new Vector2(1, v0),
            new Vector2(1, v0),
            new Vector2(0, v1),
            new Vector2(1, v1)
        });
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < m_riverPoints.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_riverPoints[i].Left, 0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(m_riverPoints[i].Right, 0.1f);
        }
    }

#if UNITY_EDITOR
    private void OnTransformChildrenChanged()
    {
        UpdateRiverPointsList();
    }
#endif
}
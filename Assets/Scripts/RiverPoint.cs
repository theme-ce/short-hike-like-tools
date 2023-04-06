using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class RiverPoint : MonoBehaviour
{
    [SerializeField] private float m_width;
    [SerializeField] private float m_xzRotation;
    [SerializeField] private RiverGenerator m_riverGenerator;

    public Vector3 Left { get; private set; }
    public Vector3 Right { get; private set; }

    private void Update()
    {
        UpdateValue();
    }

    private void UpdateValue()
    {
        Quaternion rotation = Quaternion.AngleAxis(m_xzRotation, Vector3.up);

        Vector3 direction = rotation * transform.forward;
        Left = transform.position + direction.normalized * m_width;
        Right = transform.position - direction.normalized * m_width;
        m_riverGenerator.GenerateRiverMesh();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }

    private void OnSceneGUI()
    {
        // Convert mouse position to a ray in the world space
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        // Check if the ray intersects with the gizmo sphere
        if (Physics.SphereCast(ray, 0.1f, out RaycastHit hit))
        {
            // Check if the hit object is this RiverPoint
            if (hit.collider.gameObject == gameObject)
            {
                // Select the object
                Selection.activeGameObject = gameObject;
            }
        }
    }
#endif
}
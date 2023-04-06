using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RiverGenerator))]
public class RiverGeneratorEditor : Editor
{
    RiverGenerator riverMeshGenerator;

    void OnEnable()
    {
        riverMeshGenerator = (RiverGenerator)target;
        Undo.undoRedoPerformed += UpdateGenerator;
    }

    void OnDisable()
    {
        Undo.undoRedoPerformed -= UpdateGenerator;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate River Mesh"))
        {
            riverMeshGenerator.GenerateRiverMesh();
        }
    }

    void UpdateGenerator()
    {
        if (Application.isPlaying)
        {
            return;
        }

        // Clear and repopulate the river points list
        riverMeshGenerator.RiverPoints.Clear();
        RiverPoint[] riverPoints = riverMeshGenerator.GetComponentsInChildren<RiverPoint>();
        foreach (RiverPoint riverPoint in riverPoints)
        {
            riverMeshGenerator.RiverPoints.Add(riverPoint);
        }

        // Generate the river mesh
        riverMeshGenerator.GenerateRiverMesh();
    }
}

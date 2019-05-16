using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Node))]
public class NodeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Node node = (Node)target;
        if (GUILayout.Button("Build Object"))
        {
            node.OnMovableChanged();
        }
    }
}

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Chain))]
public class ChainInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Chain chain = (Chain)target;

        if (GUILayout.Button("Build Object"))
        {
            chain.SetRadiantAngle();
        }
    }
}

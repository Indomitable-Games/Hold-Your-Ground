using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(GenWorld))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GenWorld mapGen = (GenWorld)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.DrawMapInEditor();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.DrawMapInEditor();
        }
    }
}

using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(InventoryGridDrawer))]
public class InventoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        InventoryGridDrawer inv = (InventoryGridDrawer)target;

        if (DrawDefaultInspector())
        {
            
                inv.DrawGrid();
            
        }

        if (GUILayout.Button("Draw"))
        {
            inv.DrawGrid();
        }
    }
}

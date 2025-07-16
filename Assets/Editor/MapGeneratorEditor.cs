using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator _mapGenerator = (MapGenerator)target;

        if(DrawDefaultInspector())
            if(_mapGenerator._autoUpdate)
                _mapGenerator.DrawMapInEditor();

        if(GUILayout.Button("Generate Map"))
            _mapGenerator.DrawMapInEditor();
    }
}

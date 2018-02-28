using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Planet planet = (Planet)target;
        if (GUILayout.Button("Generate")) {
            planet.Build();
        }
    }
	
}

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaveManager))]
public class WaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        WaveManager waveManager = (WaveManager)target;

        if (GUILayout.Button("Start Wave"))
        {
            waveManager.StartWave();
        }
    }
}

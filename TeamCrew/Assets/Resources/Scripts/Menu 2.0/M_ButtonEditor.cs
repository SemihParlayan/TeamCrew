using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(M_Button))]
public class M_ButtonEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        M_Button b = target as M_Button;

        b.EditorUpdate();
    }
}
#endif

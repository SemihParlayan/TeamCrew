using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(VineBuilder_Object))]
public class VineBuilder_Object_Editor : Editor 
{
    private bool settingBottomPoint;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        VineBuilder_Object obj = (VineBuilder_Object)target;

        if (!settingBottomPoint)
        {
            if (GUILayout.Button("Set bottom point"))
            {
                settingBottomPoint = true;
            }
        }

        if (settingBottomPoint)
        {
            if (GUILayout.Button("Stop"))
            {
                settingBottomPoint = false;
            }
        }
    }

    void OnSceneGUI()
    {
        Event e = Event.current;
        VineBuilder_Object obj = (VineBuilder_Object)target;

        Ray r = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Vector3 mousePos = new Vector3(r.origin.x, r.origin.y);

        switch (e.type)
        {
            case EventType.mouseDown:
                {
                    if (e.modifiers == EventModifiers.Alt)
                        break;

                    if (e.button == 0)
                    {
                        int controlId = GUIUtility.GetControlID(FocusType.Passive);
                        GUIUtility.hotControl = controlId;
                        Event.current.Use();

                        if (settingBottomPoint)
                        {
                            settingBottomPoint = false;
                            if (obj.bottomPoint == null)
                            {
                                GameObject o = new GameObject("bottomPoint");
                                obj.bottomPoint = o.transform;
                                obj.bottomPoint.parent = obj.transform;
                            }
                            obj.bottomPoint.position = mousePos;
                        }
                    }
                    else
                    {
                        if (GUIUtility.hotControl != 0)
                            GUIUtility.hotControl = 0;
                    }
                    break;
                }
        }

        SceneView.RepaintAll();
    }
}

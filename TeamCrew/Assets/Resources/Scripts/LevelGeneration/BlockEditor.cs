using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Block))]
public class BlockEditor : Editor 
{
    void OnSceneGUI()
    {
        if (!Application.isPlaying)
        {
            Block block = (Block)target;
            Event e = Event.current;
            Vector3 mousePos = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
            mousePos.z = 0;
            switch (e.type)
            {
                case EventType.KeyDown:
                    if (e.keyCode == KeyCode.E)
                    {
                        block.SetEndPosition(mousePos);
                    }
                    if (e.keyCode == KeyCode.S)
                    {
                        block.SetStartPosition(mousePos);
                    }
                    break;
            }
        }
    }
}

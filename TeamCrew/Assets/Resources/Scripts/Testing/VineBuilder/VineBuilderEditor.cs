#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

[System.Serializable]
public class BuildPart
{
    public VineBuilder_Object obj;
    public Rigidbody2D body;
    public HingeJoint2D joint;

    public BuildPart()
    {

    }
    public BuildPart(BuildPart copy)
    {
        obj = copy.obj;
        body = copy.body;
        joint = copy.joint;
    }
    public void Reset()
    {
        obj = null;
        body = null;
        joint = null;
    }
}

[CustomEditor(typeof(VineBuilder))]
public class VineBuilderEditor : Editor
{
    private int number = 0;
    private int buildIndex = 0;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        VineBuilder builder = (VineBuilder)target;

        if (!builder.searchingForNewLink)
        {
            if (GUILayout.Button("Clear"))
            {
                number = 0;
                int count = builder.transform.childCount;
                for (int i = 0; i < count; i++)
                {
                    DestroyImmediate(builder.transform.GetChild(0).gameObject);
                }

                builder.part.Reset();
                builder.prevPart.Reset();
            }

            if (GUILayout.Button("Start searching for new link"))
            {
                builder.searchingForNewLink = true;
            }
        }
        else
        {
            if (GUILayout.Button("Stop"))
            {
                builder.searchingForNewLink = false;
            }
        }
        
        
    }
    void OnSceneGUI()
    {
        VineBuilder builder = (VineBuilder)target;


        Event e = Event.current;
        Ray r = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Vector3 mousePos = new Vector3(r.origin.x, r.origin.y);


        if (builder.searchingForNewLink)
        {
            SceneView.RepaintAll();
            builder.closest = GetClosestChild(builder, mousePos);
        }

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

                        AttachVine(builder, mousePos, builder.vines[buildIndex]);
                    }
                    //else if (e.button == 1)
                    //{
                    //    int controlId = GUIUtility.GetControlID(FocusType.Passive);
                    //    GUIUtility.hotControl = controlId;
                    //    Event.current.Use();

                    //    AttachVine(builder, mousePos, builder.vines[1]);
                    //}
                    else
                    {
                        if (GUIUtility.hotControl != 0)
                            GUIUtility.hotControl = 0;
                    }
                    break;
                }
            case EventType.keyDown:
                {
                    int controlId = GUIUtility.GetControlID(FocusType.Passive);
                    GUIUtility.hotControl = controlId;
                    Event.current.Use();

                    if (e.keyCode == KeyCode.Alpha1)
                        buildIndex = 0;
                    if (e.keyCode == KeyCode.Alpha2)
                        buildIndex = 1;
                    if (e.keyCode == KeyCode.Alpha3)
                        buildIndex = 2;
                    if (e.keyCode == KeyCode.Alpha4)
                        buildIndex = 3;
                    if (e.keyCode == KeyCode.Alpha5)
                        buildIndex = 4;
                    if (e.keyCode == KeyCode.Alpha6)
                        buildIndex = 5;
                    if (e.keyCode == KeyCode.Alpha7)
                        buildIndex = 6;
                    if (e.keyCode == KeyCode.Alpha8)
                        buildIndex = 7;
                    if (e.keyCode == KeyCode.Alpha9)
                        buildIndex = 8;
                    break;
                }
        }

        //Rotating
        if (builder.part.obj != null)
        {
            Vector3 diff = mousePos - builder.part.obj.transform.position;
            diff.Normalize();

            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            builder.part.obj.transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);
        }
    }


    private VineBuilder_Object GetClosestChild(VineBuilder builder, Vector3 mousePos)
    {
        Transform closest = null;
        float minDistance = float.MaxValue;

        for (int i = 0; i < builder.transform.childCount; i++)
        {
            Transform child = builder.transform.GetChild(i);
            Vector3 pos = child.position;
            float dist = Vector3.Distance(mousePos, pos);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = child;
            }
        }

        return closest.GetComponent<VineBuilder_Object>();
    }
    private void AttachVine(VineBuilder builder, Vector3 mousePos, VineBuilder_Object obj)
    {
        if (builder.searchingForNewLink)
        {
            builder.prevPart = new BuildPart();
            builder.prevPart.obj = builder.closest;
            builder.prevPart.body = builder.closest.GetComponentInChildren<Rigidbody2D>();
            builder.prevPart.joint = builder.closest.GetComponentInChildren<HingeJoint2D>();

            builder.searchingForNewLink = false;
        }
        else
        {
            if (builder.part.obj == null)
            {
                Vector3 spawnPos = (builder.prevPart.obj != null) ? builder.prevPart.obj.bottomPoint.position : mousePos;
                Transform vinePart = Instantiate(obj.transform, spawnPos, Quaternion.identity) as Transform;
                vinePart.parent = builder.transform; vinePart.name = number++.ToString();

                builder.part = new BuildPart();
                builder.part.obj = vinePart.GetComponentInChildren<VineBuilder_Object>();
                builder.part.body = vinePart.GetComponentInChildren<Rigidbody2D>();
                builder.part.joint = vinePart.GetComponentInChildren<HingeJoint2D>();

                if (builder.prevPart.obj != null)
                {
                    builder.part.body.isKinematic = false;
                    builder.part.body.mass = 40;

                    builder.part.joint.connectedBody = builder.prevPart.body;
                    builder.part.joint.autoConfigureConnectedAnchor = true;
                }
                else
                {
                    builder.part.body.isKinematic = true;
                    builder.part.body.mass = 10000;

                    builder.part.joint.connectedBody = null;
                    builder.part.joint.autoConfigureConnectedAnchor = false;
                }
            }
            else
            {
                builder.prevPart = new BuildPart(builder.part);
                builder.part.Reset();
            }
        }
    }
}

#endif

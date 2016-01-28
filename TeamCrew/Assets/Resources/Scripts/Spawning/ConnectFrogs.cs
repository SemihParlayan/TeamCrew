using UnityEngine;
using System.Collections;

public enum Hand
{
    Left,
    Right
}
public enum VersusGripPoint
{
    Backpack = 0,
    Kneepad = 1,
    Foot = 2,
    Hand = 3
}
public class ConnectFrogs : MonoBehaviour 
{
    public Respawn respawn;

    void Awake()
    {
        respawn = GetComponent<Respawn>();
        respawn.connectFrogs = this;
    }

    public Transform SpawnFrog(Transform prefab, Vector2 position, bool withTutorialHelpers = true)
    {
        GameObject o = Instantiate(prefab.gameObject, position, Quaternion.identity) as GameObject;

        if (!withTutorialHelpers)
        {
            Transform body = o.transform.FindChild("body");
            body.GetComponent<Line>().Remove();
        }

        return o.transform;
    }
    public Transform SpawnFrogConnected(Transform prefab, Transform connectedFrog, Hand handToGripWith, VersusGripPoint gripToGrab)
    {
        int sideModifier = (handToGripWith == Hand.Left) ? 1 : -1;
        Vector3 offset = GetOffSetToGrip(gripToGrab);
        offset.x *= sideModifier;

        Transform newFrog = SpawnFrog(prefab, connectedFrog.position + offset, false);
        FrogPrototype frog = newFrog.transform.GetComponentInChildren<FrogPrototype>();

        HandGrip handGrip = (handToGripWith == Hand.Left) ? frog.leftGripScript : frog.rightGripScript;
        handGrip.SetForcedGrip(true, false);

        if (gripToGrab == VersusGripPoint.Hand)
        {
            frog = connectedFrog.GetComponentInChildren<FrogPrototype>();
            handGrip = (handToGripWith == Hand.Left) ? frog.rightGripScript : frog.leftGripScript;
            handGrip.SetForcedGrip(true, false);
        }

        return newFrog;
    }

    Vector2 GetOffSetToGrip(VersusGripPoint grip)
    {
        switch(grip)
        {
            case VersusGripPoint.Backpack:
                return new Vector2(2, -0.25f);

            case VersusGripPoint.Kneepad:
                return new Vector2(2.2f, -1.65f);

            case VersusGripPoint.Foot:
                return new Vector2(2.25f, -2.9f);

            case VersusGripPoint.Hand:
                return new Vector2(4.05f, 0.0f);
        }

        return new Vector2(0.0f, 0.0f);
    }
}

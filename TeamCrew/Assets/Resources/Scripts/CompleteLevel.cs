using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using System;
using System.IO;
using System.Net.Sockets;
using System.Net;

public class CompleteLevel : MonoBehaviour
{
    void Start()
    {
        SetLevel();
    }
    void OnDrawGizmos()
    {
        SetLevel();
    }

    void SetLevel()
    {
        if (transform.position != new Vector3(-0.18f, 40.53f, 0f))
            transform.position = new Vector3(-0.18f, 40.53f, 0f);
    }
}

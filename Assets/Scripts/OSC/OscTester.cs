using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(OscJack.OscEventReceiver))]
public class OSCTester : MonoBehaviour
{

    public void OnReceiveInt(int value)
    {
        Debug.Log("OnReceiveInt: " + value);
    }

    public void OnReceiveIntFoo(int value)
    {
        Debug.Log("FOO: " + value);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAwake : MonoBehaviour
{
    public void Awake()
    {
        Debug.Log("Test awake");
    }

    public void Start()
    {
        Debug.Log("test start");
    }
    
    public void OnEnable()
    {
        Debug.Log("test on enable");
    }
}

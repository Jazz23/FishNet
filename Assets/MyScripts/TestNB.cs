using FishNet.Object;
using UnityEngine;

public class TestNB : NetworkBehaviour
{
    public override void OnStartClient() => Debug.Log("hi");

    public void Test()
    {
        
    }
}
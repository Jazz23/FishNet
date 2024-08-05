using FishNet.Object;
using UnityEngine;

namespace Stuff
{
    public class TestNb : NetworkBehaviour
    {
        public override void OnStartClient()
        {
            Debug.Log("Started");
        }
    }
}
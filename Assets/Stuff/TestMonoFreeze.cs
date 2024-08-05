using System;
using System.Threading;
using FishNet;
using UnityEngine;

namespace Stuff
{
    public class TestMonoFreeze : MonoBehaviour
    {
        private void Awake()
        {
            foreach (var go in gameObject.scene.GetRootGameObjects())
            {
                go.SetActive(false);
            }
        }
    }
}
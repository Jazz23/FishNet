using UnityEngine;

namespace Stuff
{
    public class TestMono : MonoBehaviour
    {
        public void Start()
        {
            var mesh = GetComponent<MeshFilter>().mesh;
        }
    }
}
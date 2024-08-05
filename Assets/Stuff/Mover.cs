using System;
using UnityEngine;

namespace Stuff
{
    /// <summary>
    /// Moves the object slowly back and forth 1 unit
    /// </summary>
    public class Mover : MonoBehaviour
    {
        private Vector3 _startPos;
        private Vector3 _endPos;
        private float _speed = 1f;

        private void Start()
        {
            _startPos = transform.position;
            _endPos = _startPos + Vector3.right;
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(_startPos, _endPos, Mathf.PingPong(Time.time * _speed, 1f));
        }
    }
}
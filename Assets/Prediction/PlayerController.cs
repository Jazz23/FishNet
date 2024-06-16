using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prediction
{
    public partial class PlayerController : NetworkBehaviour
    {
        [SerializeField]
        private float moveRate = 4f;

        private Vector2 _moveDirection;
        private InputAction _moveAction;
        
        public override void OnStartClient()
        {
            if (!IsOwner) return;
            
            GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
            SubscribeMovement();
        }
        public override void OnStopClient() => UnsubscribeMovement();

        private void Update()
        {
            if (_moveDirection.magnitude > 0.01f)
                Move(_moveDirection.x, _moveDirection.y);
        }
        
        private void Move(float hor, float ver)
        {
            float gravity = -10f * Time.deltaTime;
            //If ray hits floor then cancel gravity.
            Ray ray = new Ray(transform.position + new Vector3(0f, 0.05f, 0f), -Vector3.up);
            if (Physics.Raycast(ray, 0.1f + -gravity))
                gravity = 0f;

            /* Moving. */
            Vector3 direction = new Vector3(
                0f,
                gravity,
                ver * moveRate * Time.deltaTime);

            transform.position += transform.TransformDirection(direction);
            transform.Rotate(new Vector3(0f, hor * 100f * Time.deltaTime, 0f));
        }
    }
}
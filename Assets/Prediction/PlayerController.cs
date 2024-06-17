using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prediction
{
    [RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
    public partial class PlayerController : NetworkBehaviour
    {
        public PlayerStats currentPlayerSettings;

        private Rigidbody _playerRb;
        private Camera _camera;
        
        public override void OnStartClient()
        {
            if (!IsOwner)
            {
                enabled = false;
                return;
            }
            
            FindReferences();
            _camera.gameObject.SetActive(true);
            SubscribeMovement();
        }

        public override void OnStartServer() => enabled = false;

        private void FindReferences()
        {
            _playerRb = GetComponent<Rigidbody>();
            _camera = GetComponentInChildren<Camera>(true);
        }

        public override void OnStopClient() => UnsubscribeMovement();

        private void Update()
        {
            if (_moveDirection.sqrMagnitude > 0.01f)
                Move(_moveDirection);
            if (_lookDirection.sqrMagnitude > 0.01f)
                Look(_lookDirection);
        }
        
        private void Move(Vector2 direction)
        {
            var (inputX, inputY) = direction;
            // camera transform forward accurate movement after shooting but slows down when aiming at the ground,
            // transform.forward is opposite of this issue, project on plane is just a fancy way of setting the y velocity to 0
            var playerForward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized;
            var playerRight = _camera.transform.right; // changed to camera.right instead of using transform .
                                                           // right due to axis becoming missaligned with eachother for some reason
        
            var scaledMoveDirection = (playerRight * inputX + playerForward * inputY)
                                      * currentPlayerSettings.moveSpeed;
            var clampMovement = Vector3.ClampMagnitude(scaledMoveDirection, currentPlayerSettings.moveSpeed);

            _playerRb.MovePosition(transform.position + clampMovement * Time.deltaTime);
        }
        
        private void Look(Vector2 lookDirection)
        {
            var (mouseInputX, mouseInputY) = lookDirection;
            transform.Rotate(Vector3.up * (mouseInputX * currentPlayerSettings.lookSensitivity));
        
            // X AXIS
            _camera.transform.Rotate(Vector3.right * (-mouseInputY * currentPlayerSettings.lookSensitivity));
        }
    }
}
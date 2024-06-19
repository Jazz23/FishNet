using System;
using FishNet.Object;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prediction
{
    [RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
    public partial class PlayerController : NetworkBehaviour
    {
        public PlayerStats currentPlayerSettings;
        [Header("PlayerMovement")]
        [SerializeField] private float jumpRayLength; // give a small thrseshold for leniancy, used for grounded checks

        private Rigidbody _playerRb;
        private Camera _camera;
        private bool _isOnGround;
        private CapsuleCollider _playerMainCapsuleCollider;
        
        public override void OnStartClient()
        {
            if (!IsOwner)
            {
                enabled = false; // Don't need update for other players
                return;
            }
            
            FindReferences();
            _camera.gameObject.SetActive(true); // Enable camera for *this* instance of player since it's us
            SubscribeToActions();
            Cursor.lockState = CursorLockMode.Locked; // Disable cursor since we're an FPS game
            Cursor.visible = false;
        }

        public override void OnStartServer() => enabled = false;

        private void FindReferences()
        {
            _actionMap = GetComponent<PlayerInput>().currentActionMap;
            _moveAction = _actionMap.FindAction("move");
            _lookAction = _actionMap.FindAction("look");
            _jumpAction = _actionMap.FindAction("jump");
            _playerMainCapsuleCollider = GetComponentInChildren<CapsuleCollider>();
            _playerRb = GetComponent<Rigidbody>();
            _camera = GetComponentInChildren<Camera>(true);
        }

        public override void OnStopClient() => UnsubscribeToActions();

        private void Update()
        {
            RaycastForGrounded();
            if (!_isOnGround)
                Fall();
        }

        private void FixedUpdate()
        {
            if (_moveAction.ReadValue<Vector2>() is { sqrMagnitude: > 0.01f } moveDir)
                Move(moveDir);
        }

        private void LateUpdate()
        {
            if (_lookAction.ReadValue<Vector2>() is { sqrMagnitude: > 0.01f } lookDir)
                Look(lookDir);
        }

        private void Fall()
        {
            _playerRb.AddForce(-Vector3.up * currentPlayerSettings.fallMultiplier, ForceMode.Force);
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

        private void OnJump(InputAction.CallbackContext ctx)
        {
            if (!_isOnGround) return;
            
            _playerRb.velocity = _playerRb.velocity.WithY(currentPlayerSettings.jumpHeight);
        }
        
        //Raycast for grounded
        private void RaycastForGrounded()
        {
            Vector3[] corners = GetColliderCorners();

            foreach (Vector3 corner in corners)
            {
                Ray ray = new Ray(corner, -transform.up);

                Debug.DrawRay(ray.origin, ray.direction * jumpRayLength, Color.red);

                if (Physics.Raycast(ray, out RaycastHit hit, jumpRayLength)) // if one of the rays hits something then return is on ground, this ensures only 1 ray is drawn if something is being hit and draws more the second that one stops hitting
                {
                    _isOnGround = true;
                    return;
                }
            }

            _isOnGround = false; // if loop finishes with no rays hitting then set isonground to false
        }
        
        private Vector3[] GetColliderCorners() // drawing points along the bottem of the capsule collider at 4 corners 
        {
            float offset = 0.2f;//0.2f;
            Vector3 point1 = _playerMainCapsuleCollider.bounds.center + Vector3.down * (_playerMainCapsuleCollider.height * 0.5f - offset);

            Vector3[] corners = new Vector3[4];

            corners[0] = point1 + Vector3.right * (_playerMainCapsuleCollider.radius * 0.5f);
            corners[1] = point1 - Vector3.right * (_playerMainCapsuleCollider.radius * 0.5f);
            corners[2] = point1 + Vector3.forward * (_playerMainCapsuleCollider.radius * 0.5f);
            corners[3] = point1 - Vector3.forward * (_playerMainCapsuleCollider.radius * 0.5f);

            return corners;
        }
    }
}
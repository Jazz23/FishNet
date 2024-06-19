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
        private bool _isSprinting;
        private float _currentMoveSpeed;
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
            _playerMainCapsuleCollider = GetComponentInChildren<CapsuleCollider>();
            _playerRb = GetComponent<Rigidbody>();
            _camera = GetComponentInChildren<Camera>(true);
        }

        public override void OnStopClient() => UnsubscribeToActions();

        private void Update()
        {
            RaycastForGrounded();
            UpdateCurrentMoveSpeed();
            
            if (!_isOnGround)
                Fall();
        }

        private void FixedUpdate()
        {
            if (_moveAction.ReadValue<Vector2>() is { sqrMagnitude: > 0.01f } moveDir)
                Move(moveDir, _currentMoveSpeed);
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

        private void Move(Vector2 direction, float moveSpeed)
        {
            var (inputX, inputY) = direction;
            // camera transform forward accurate movement after shooting but slows down when aiming at the ground,
            // transform.forward is opposite of this issue, project on plane is just a fancy way of setting the y velocity to 0
            var playerForward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized;
            var playerRight = _camera.transform.right; // changed to camera.right instead of using transform .
                                                           // right due to axis becoming missaligned with eachother for some reason
        
            var scaledMoveDirection = (playerRight * inputX + playerForward * inputY)
                                      * moveSpeed;
            var clampMovement = Vector3.ClampMagnitude(scaledMoveDirection, moveSpeed);

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

        private void OnStartSprint(InputAction.CallbackContext obj)
        {
            _isSprinting = true;
        }

        private void OnStopSprint(InputAction.CallbackContext obj)
        {
            _isSprinting = false;
        }
    }
}
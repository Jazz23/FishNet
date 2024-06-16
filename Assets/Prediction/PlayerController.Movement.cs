using UnityEngine;
using UnityEngine.InputSystem;

namespace Prediction
{
    public partial class PlayerController
    {
        private void SubscribeMovement()
        {
            _moveAction = GetComponent<PlayerInput>().currentActionMap.FindAction("move");
            _moveAction.started += OnMove;
            _moveAction.performed += OnMove;
            _moveAction.canceled += OnMove;
        }

        private void UnsubscribeMovement()
        {
            _moveAction.started -= OnMove;
            _moveAction.performed -= OnMove;
            _moveAction.canceled -= OnMove;
        }

        private void OnMove(InputAction.CallbackContext obj)
        {
            _moveDirection = obj.ReadValue<Vector2>();
        }
    }
}
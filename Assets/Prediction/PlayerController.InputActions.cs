using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prediction
{
    public partial class PlayerController
    {
        private InputAction _lookAction;
        private InputAction _moveAction;
        private Vector2 _lookDirection;
        private Vector2 _moveDirection;

        private void SubscribeMovement()
        {
            var actionMap = GetComponent<PlayerInput>().currentActionMap;
            _moveAction = actionMap.FindAction("move");
            _lookAction = actionMap.FindAction("look");
            SubscribeAction(_moveAction, OnMove);
            SubscribeAction(_lookAction, OnLook);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        private void UnsubscribeMovement()
        {
            UnsubscribeAction(_moveAction, OnMove);
            UnsubscribeAction(_lookAction, OnLook);
        }

        private void SubscribeAction(InputAction action, Action<InputAction.CallbackContext> callback)
        {
            action.started += callback;
            action.performed += callback;
            action.canceled += callback;
        }

        private void UnsubscribeAction(InputAction action, Action<InputAction.CallbackContext> callback)
        {
            _moveAction.started -= callback;
            _moveAction.performed -= callback;
            _moveAction.canceled -= callback;
        }

        private void OnMove(InputAction.CallbackContext obj)
        {
            _moveDirection = obj.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext obj)
        {
            _lookDirection = obj.ReadValue<Vector2>();
        }
    }
}
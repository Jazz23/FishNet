using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prediction
{
    public partial class PlayerController
    {
        private InputActionMap _actionMap;
        private InputAction _lookAction;
        private InputAction _moveAction;
        private InputAction _jumpAction;

        private void SubscribeToActions()
        {
            _jumpAction.started += OnJump;
        }
        
        private void UnsubscribeToActions()
        {
            _jumpAction.started -= OnJump;
        }
    }
}
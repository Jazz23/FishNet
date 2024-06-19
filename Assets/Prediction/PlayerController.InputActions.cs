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
        private InputAction _sprintAction;

        private void SubscribeToActions()
        {
            _moveAction = _actionMap.FindAction("move");
            _lookAction = _actionMap.FindAction("look");
            _jumpAction = _actionMap.FindAction("jump");
            _sprintAction = _actionMap.FindAction("sprint");
            
            _jumpAction.started += OnJump;
            _sprintAction.started += OnStartSprint;
            _sprintAction.canceled += OnStopSprint;
        }
        
        private void UnsubscribeToActions()
        {
            _jumpAction.started -= OnJump;
            _sprintAction.started -= OnStartSprint;
            _sprintAction.canceled -= OnStopSprint;
        }
    }
}
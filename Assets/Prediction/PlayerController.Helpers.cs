using UnityEngine;

namespace Prediction
{
    public partial class PlayerController
    {
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

        private void UpdateCurrentMoveSpeed()
        {
            var targetSpeed = _isSprinting ? currentPlayerSettings.sprintSpeed : currentPlayerSettings.moveSpeed;
            _currentMoveSpeed = Mathf.Lerp(_currentMoveSpeed, targetSpeed, Time.deltaTime * 8f);
        }
    }
}
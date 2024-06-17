using UnityEngine;

namespace Prediction
{
    [CreateAssetMenu(fileName = "Player", menuName = "CustomAssets/PlayerSettings")]
    public class PlayerStats : ScriptableObject
    {
        [Header("Player Settings")]
        public float moveSpeed;
        public float sprintSpeed;
        public float jumpHeight;
        public float fallMultiplier; // heck
        public float lookSensitivity;
    }
}
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Source.Player.Input
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private Key _reloadKey;

        public event Action ShootButtonPressed;
        public event Action ReloadButtonPressed;

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
                ShootButtonPressed?.Invoke();
            
            if (Keyboard.current[_reloadKey].wasPressedThisFrame)
                ReloadButtonPressed?.Invoke();
        }
    }
}
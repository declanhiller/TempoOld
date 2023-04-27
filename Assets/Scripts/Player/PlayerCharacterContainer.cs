using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
    public class PlayerCharacterContainer : MonoBehaviour {



        public event Action OnDodge;
        public event Action OnDodgeCancel;
        public void Dodge(InputAction.CallbackContext context) {
            if (context.performed) {
                OnDodge.Invoke();
            } else if (context.canceled) {
                OnDodgeCancel.Invoke();
            }
        }
        
        public event Action OnJump;
        public void Jump(InputAction.CallbackContext context) {
            if (!context.performed) return;
            OnJump.Invoke();
        }
        
        public Vector3 movementInputVector { get; private set; }
        public void OnMovementInput(InputAction.CallbackContext context)
        {
            movementInputVector = context.ReadValue<Vector2>();
        }
        
        public event Action OnLightAttackTrigger;
        public event Action OnLightAttackRelease;
        public void Light(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnLightAttackTrigger?.Invoke();
            } else if (context.canceled)
            {
                OnLightAttackRelease?.Invoke();
            }
        }

        
        public event Action OnHeavyAttackTrigger;
        public event Action OnHeavyAttackRelease;
        public void Heavy(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnHeavyAttackTrigger?.Invoke();
            } else if (context.canceled)
            {
                OnHeavyAttackRelease?.Invoke();
            }
        }

        
        public event Action OnSpecialAttackTrigger;
        public event Action OnSpecialAttackRelease;
        public void Special(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnSpecialAttackTrigger?.Invoke();
            } else if (context.canceled)
            {
                OnSpecialAttackRelease?.Invoke();
            }
        }
        
    }
}
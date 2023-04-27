using System;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
    public class PlayerUIController : MonoBehaviour {
        
        [SerializeField] private float navThreshold = 0.3f;
        [SerializeField] private float resetValue = 0.1f;
        [SerializeField] private PlayerBase player;
        private bool xActive = true;
        private bool yActive = true;
        
        
        public void OnNavigate(InputAction.CallbackContext context) {
            if (!context.performed) return;

            Vector2 direction = context.ReadValue<Vector2>();

            if (Math.Abs(direction.x) < resetValue) {
                xActive = true;
            }
            
            if (Math.Abs(direction.y) < resetValue) {
                yActive = true;
            }

            UIGlobalManager.Direction passThroughDirection = UIGlobalManager.Direction.ZERO;
            
            if (direction.x > navThreshold && xActive) {
                passThroughDirection = UIGlobalManager.Direction.RIGHT;
                xActive = false;
            } else if (direction.x < -navThreshold && xActive) {
                passThroughDirection = UIGlobalManager.Direction.LEFT;
                xActive = false;
            } else if (direction.y > navThreshold && yActive) {
                passThroughDirection = UIGlobalManager.Direction.UP;
                yActive = false;
            } else if (direction.y < -navThreshold && yActive) {
                passThroughDirection = UIGlobalManager.Direction.DOWN;
                yActive = false;
            }
            
            UIGlobalManager.INSTANCE.Navigate(passThroughDirection);
            
        }

        public void OnSubmit(InputAction.CallbackContext context) {
            if (!context.performed) return;
            UIGlobalManager.INSTANCE.Submit(player.playerNumber);
        }

        public void OnBack(InputAction.CallbackContext context) {
            if (!context.performed) return;
            UIGlobalManager.INSTANCE.Back(player.playerNumber);
        }

        public void OnMenu(InputAction.CallbackContext context) {
            if (!context.performed) return;
            UIGlobalManager.INSTANCE.PullupMenu();
        }
        
        public void OnMenuThroughPlayer(InputAction.CallbackContext context) {
            if (!context.performed) return;
            UIGlobalManager.INSTANCE.PullupMenu(GetComponent<PlayerInput>());
        }

    }
}
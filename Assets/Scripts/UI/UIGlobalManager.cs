using System;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class UIGlobalManager : PersistentManager<UIGlobalManager>
    {

        [SerializeField] public GameObject selectedObject;

        [SerializeField] public GameObject escMenuObj;

        [SerializeField] private AudioSource audioSource;

        [SerializeField] private float audioSourceDelay = 0.1f;
        
        public IGlobalUI currentlySelectedUI { get; set; }

        private IGlobalUI previouslySelectedUIElementBeforeMenu;

        private void Start()
        {
            currentlySelectedUI = selectedObject.GetComponent<IGlobalUI>();
        }
        
        public void Navigate(Direction direction)
        {
            INavigatableUI navigatableUI = currentlySelectedUI as INavigatableUI;
            if (navigatableUI == null) return;
            switch (direction) {
                case Direction.RIGHT:
                    navigatableUI.Right();
                    break;
                case Direction.LEFT:
                    navigatableUI.Left();
                    break;
                case Direction.UP:
                    navigatableUI.Up();
                    break;
                case Direction.DOWN:
                    navigatableUI.Down();
                    break;
            }
        }

        public enum Direction {
            ZERO, LEFT, RIGHT, UP, DOWN
        }

        public void Submit(int playerNumber)
        {
            IPressableUI pressableUI = currentlySelectedUI as IPressableUI;
            if (pressableUI == null) return;
            PlayUiAudio();

            pressableUI.Press(playerNumber);
            
        }

        public void Back(int playerNumber) {
            IBackableUI backableUI = currentlySelectedUI as IBackableUI;
            if (backableUI == null) return;
            PlayUiAudio();

            backableUI.Back(playerNumber);
        }

        protected override void LoadNewData(UIGlobalManager newInstance) {
            if (newInstance.selectedObject != null) {
                currentlySelectedUI = newInstance.selectedObject.GetComponent<IGlobalUI>();
            }
            escMenuObj = newInstance.escMenuObj;
        }

        public void PullupMenu() {
            previouslySelectedUIElementBeforeMenu = currentlySelectedUI;
            if (escMenuObj == null) return;
            escMenuObj.SetActive(true);
            EscMenu escMenu = escMenuObj.GetComponent<EscMenu>();
            currentlySelectedUI = escMenu;
            escMenu.OnClose = OnClose;
        }

        private PlayerInput input;
        public void PullupMenu(PlayerInput input) {
            previouslySelectedUIElementBeforeMenu = currentlySelectedUI;
            if (escMenuObj == null) return;
            input.SwitchCurrentActionMap("UI");
            PlayUiAudio();
            this.input = input;
            escMenuObj.SetActive(true);
            EscMenu escMenu = escMenuObj.GetComponent<EscMenu>();
            currentlySelectedUI = escMenu;
            escMenu.OnClose = OnClose;
        }

        public void OnClose() {
            if (input != null) {
                input.SwitchCurrentActionMap("Player");
                input = null;
            }
            currentlySelectedUI = previouslySelectedUIElementBeforeMenu;
        }

        public void PlayUiAudio() {
            audioSource.time = audioSourceDelay;
            audioSource.Play();
        }
        
        
    }
}
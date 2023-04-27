using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI {
    public class UIButtonScrollerV2 : MonoBehaviour, INavigatableUI, IPressableUI, IBackableUI {
        [SerializeField] private GameObject[] buttons;

        [SerializeField] private GameObject logo;
        
        [SerializeField] private float radius;
        
        [SerializeField] private float rotationAnimationTime = 0.2f;
        [SerializeField] private float scaleAnimationTime = 0.3f;

        [SerializeField] private float fullLength;
        [SerializeField] private float textOffset;
        [SerializeField] private float iconOffset;

        private RectTransform rectTransform;

        private bool isOpen;
        
        private float currentIndex;

        private int bufferSize;
        private float degreesOfSeperationInBuffer;

        private int mainButtonIndex;

        private Button[] buttonsData;

        public void Start() {

            rectTransform = GetComponent<RectTransform>();
            
            bufferSize = buttons.Length - 1;
            degreesOfSeperationInBuffer = 180f / (bufferSize + 1);

            buttonsData = new Button[buttons.Length];
            for (int i = 0; i < buttons.Length; i++) {
                Button button = new Button() {
                    button = buttons[i],
                    // image = buttons[i].GetComponent<Image>(),
                    // text = buttons[i].GetComponentInChildren<TextMeshProUGUI>(),
                    transform = buttons[i].GetComponent<RectTransform>()
                };
                buttonsData[i] = button;
            }
            
            
            mainButtonIndex = 0;

            SetOpeningPositions();
            
        }

        private void SetOpeningPositions() {
            RotateButton(buttonsData[mainButtonIndex], 0);
            OffsetButton(buttonsData[mainButtonIndex], -fullLength);
            
            int tempIndex = mainButtonIndex;
            float degreesTracker = 90;
            for (int i = 1; i < buttonsData.Length; i++) {
                tempIndex = GetIndexFromChange(tempIndex, 1);
                Button button = buttonsData[tempIndex];
                degreesTracker += degreesOfSeperationInBuffer;
                RotateButton(button, degreesTracker);
                OffsetButton(button, -fullLength);
            }
            
        }

        private void OpenUp() {
            
            StartCoroutine(GoToTargetOffset(buttonsData[mainButtonIndex], -textOffset));
            
            int tempIndex = mainButtonIndex;
            float degreesTracker = 90;
            for (int i = 1; i < buttonsData.Length; i++) {
                tempIndex = GetIndexFromChange(tempIndex, 1);
                Button button = buttonsData[tempIndex];
                degreesTracker += degreesOfSeperationInBuffer;
                StartCoroutine(GoToTargetOffset(button, -iconOffset));
            }

            isOpen = true;
        }

        private void Collapse() {
            for (int i = 0; i < buttonsData.Length; i++) {
                StartCoroutine(GoToTargetOffset(buttonsData[i], -fullLength));
            }

            isOpen = false;
        }
        
        private void Submit() {
            isOpen = false;
            StartCoroutine(GoToTargetOffset(buttonsData[mainButtonIndex], -fullLength, OnCollapseWithSubmit));
            
            int tempIndex = mainButtonIndex;
            for (int i = 1; i < buttonsData.Length; i++) {
                tempIndex = GetIndexFromChange(tempIndex, 1);
                Button button = buttonsData[tempIndex];
                StartCoroutine(GoToTargetOffset(button, -fullLength));
            }

        }

        public void OnCollapseWithSubmit() {
            CircleScrollerButton button = buttonsData[mainButtonIndex].button.GetComponent<CircleScrollerButton>();
            button.CallButton();
        }
        
        

        private void ShiftUp() {
            UIGlobalManager.INSTANCE.PlayUiAudio();
            mainButtonIndex = GetIndexFromChange(mainButtonIndex, -1);
            
            StartCoroutine(RotateToDegrees(buttonsData[mainButtonIndex], 360, -textOffset));
        
            int tempIndex = mainButtonIndex;
            float degreesTracker = 90;
            for (int i = 1; i < buttonsData.Length; i++) {
                tempIndex = GetIndexFromChange(tempIndex, 1);
                Button button = buttonsData[tempIndex];
                degreesTracker += degreesOfSeperationInBuffer;
                StartCoroutine(RotateToDegrees(button, degreesTracker, -iconOffset));
            }
        
        }

        private void ShiftDown() {
            UIGlobalManager.INSTANCE.PlayUiAudio();
            mainButtonIndex = GetIndexFromChange(mainButtonIndex, 1);
            
            StartCoroutine(RotateToDegrees(buttonsData[mainButtonIndex], 0, -textOffset));
        
            int tempIndex = mainButtonIndex;
            float degreesTracker = 270;
            for (int i = 1; i < buttonsData.Length; i++) {
                tempIndex = GetIndexFromChange(tempIndex, -1);
                Button button = buttonsData[tempIndex];
                degreesTracker -= degreesOfSeperationInBuffer;
                StartCoroutine(RotateToDegrees(button, degreesTracker, -iconOffset));
            }
        }
        
        

        private void RotateButton(Button button, float degree) {
            Vector2 position = CalculatePosition(degree, radius);
            button.transform.anchoredPosition = position;
            button.transform.right = button.transform.anchoredPosition - rectTransform.anchoredPosition;

            // Color assetColor = new Color(button.image.color.r, button.image.color.g, button.image.color.b,
            //     opacity);

            // button.image.color = assetColor;
            // button.text.faceColor = assetColor;

            button.currentDegrees = degree;
            button.radius = radius;
        }

        private void OffsetButton(Button button, float offset) {
            Vector2 position = CalculatePosition(button.currentDegrees, radius);
            button.transform.anchoredPosition = position;
            Vector3 direction = button.transform.anchoredPosition - rectTransform.anchoredPosition;
            button.transform.right = direction;
            Vector3 offsetDistance = direction.normalized * offset;
            position = new Vector2(position.x + offsetDistance.x, position.y + offsetDistance.y);
            button.transform.anchoredPosition = position;
            button.currentOffsetFromRadius = offset;
        }


        private Vector2 CalculatePosition(float degree, float radius) {
            return new Vector2(radius * Mathf.Cos(degree * Mathf.Deg2Rad),
                radius * Mathf.Sin(degree * Mathf.Deg2Rad)) + rectTransform.anchoredPosition;
        }


        IEnumerator RotateToDegrees(Button button, float targetDegrees, float targetOffset) {

            float lerpTime = 0;

            float multiplier = 1 / rotationAnimationTime;

            float buttonStartDegrees = button.currentDegrees;
            float buttonStartOffset = button.currentOffsetFromRadius;

            
            while (lerpTime <= rotationAnimationTime) {
                float degrees = Mathf.LerpAngle(buttonStartDegrees, targetDegrees, lerpTime * multiplier);
                float offset = Mathf.LerpAngle(buttonStartOffset, targetOffset, lerpTime * multiplier);

                RotateButton(button, degrees);
                OffsetButton(button, offset);

                lerpTime += Time.deltaTime;
                
                yield return new WaitForEndOfFrame();
            }
            
            RotateButton(button, targetDegrees);
            OffsetButton(button, targetOffset);

        }
        
        IEnumerator GoToTargetOffset(Button button, float targetOffset, Action onEnd) {

            yield return StartCoroutine(GoToTargetOffset(button, targetOffset));
            
            onEnd.Invoke();
            
        }
        
        IEnumerator GoToTargetOffset(Button button, float targetOffset) {
        
            float lerpTime = 0;
        
            float multiplier = 1 / scaleAnimationTime;
            
            float startingOffset = button.currentOffsetFromRadius;


            while (lerpTime <= scaleAnimationTime) {

                float currentOffset = Mathf.Lerp(startingOffset, targetOffset, lerpTime * multiplier);
                
                OffsetButton(button, currentOffset);

                lerpTime += Time.deltaTime;
                
                yield return new WaitForEndOfFrame();
            }
            
            // RotateButton(button, button.currentDegrees);
        
        }

        private int GetIndexFromChange(int index, int change) {
            int targetIndex = index + change;
            if (targetIndex < 0) {
                targetIndex = buttonsData.Length + targetIndex;
            } else if (targetIndex >= buttonsData.Length) {
                targetIndex = targetIndex - buttonsData.Length;
            }
            
            return targetIndex;
        }


        public void Down()
        {
            if (!isOpen) return;
            ShiftUp();
        }

        public void Up()
        {
            if (!isOpen) return;
            ShiftDown();
        }

        public void Right() {
            if (!isOpen) {
                OpenUp();
                UIGlobalManager.INSTANCE.PlayUiAudio();
            } else {
                
            }
        }

        public void Left()
        {
            if (!isOpen) return;
            Collapse();
            UIGlobalManager.INSTANCE.PlayUiAudio();
            //go back an element
        }

        public void Press(int playerNumber) {
            if (!isOpen) {
                OpenUp();
                UIGlobalManager.INSTANCE.PlayUiAudio();
            } else {
                Submit();
                UIGlobalManager.INSTANCE.PlayUiAudio();
            }
        }

        public void Back(int playerNumber) {
            if (!isOpen) return;
            Collapse();
            UIGlobalManager.INSTANCE.PlayUiAudio();
        }
    }
}
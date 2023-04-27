using System;
using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
    public class EscMenu : MonoBehaviour, INavigatableUI, IPressableUI, IBackableUI {
        
        
        [SerializeField] private float scaleMultiplier;
        [SerializeField] private float pulseTimer;
        private Vector3 startScale;
        private Vector3 endScale;

        [SerializeField] private GameObject yesButton;
        [SerializeField] private GameObject noButton;

        private RectTransform rectTransform;

        private bool isYesSelected;
        
        public Action OnClose;

        private void Start() {

        }
        
        private void Awake() {
            rectTransform = noButton.GetComponent<RectTransform>();
            startScale = rectTransform.localScale;
            endScale = rectTransform.localScale * scaleMultiplier;
        }

        private void OnDestroy() {
            OnClose.Invoke();
        }

        public void Press(int playerNumber) {
            OnClose.Invoke();
            if (isYesSelected) {
                SceneManager.LoadScene("StartScreen");
            } else {
                gameObject.SetActive(false);
            }
        }

        public void Back(int playerNumber) {
            OnClose.Invoke();
            gameObject.SetActive(false);
        }

        public void Down() {
            //Do nothing
        }

        public void Up() {
            //Do nothing
        }
        
        IEnumerator ButtonScaler() {

            float timer = 0;
            Vector3 start = startScale;
            Vector3 target = endScale;
            while (true) {
                Vector3 scale = Vector3.Lerp(start, target, timer / pulseTimer);

                rectTransform.localScale = scale;
            
                timer += Time.deltaTime;
                if (timer >= pulseTimer) {
                    (start, target) = (target, start); //You learn something new about c# everyday lmao
                    timer = 0;
                }
                yield return new WaitForEndOfFrame();
            }
        }
        

        public void Right() {
            if (!isYesSelected) {
                rectTransform.localScale = startScale;
                isYesSelected = true;
                StopCoroutine(ButtonScaler());
                rectTransform.localScale = startScale;
                rectTransform = yesButton.GetComponent<RectTransform>();
                UIGlobalManager.INSTANCE.PlayUiAudio();
            }
        }

        public void Left() {
            if (isYesSelected) {
                rectTransform.localScale = startScale;
                isYesSelected = false;
                StopCoroutine(ButtonScaler());
                rectTransform.localScale = startScale;
                rectTransform = noButton.GetComponent<RectTransform>();
                UIGlobalManager.INSTANCE.PlayUiAudio();
            }
        }
        
        private void OnDisable() {
            StopCoroutine(ButtonScaler());
            rectTransform.localScale = startScale;
            rectTransform = noButton.GetComponent<RectTransform>();
        }

        private void OnEnable() {
            rectTransform.localScale = startScale;
            rectTransform = noButton.GetComponent<RectTransform>();
            StartCoroutine(ButtonScaler());
        }
    }
}
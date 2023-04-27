using System;
using System.Collections;
using UnityEngine;

namespace UI {
    public class UIPulser : MonoBehaviour {
        
        [SerializeField] private float scaleMultiplier = 1.05f;
        [SerializeField] private float pulseTimer = 0.5f;
        private Vector3 startScale;
        private Vector3 endScale;

        private void Awake() {
            startScale = transform.localScale;
            endScale = transform.localScale * scaleMultiplier;
        }

        private void Start() {
            StartCoroutine(ButtonScaler());
        }


        IEnumerator ButtonScaler() {

            float timer = 0;
            Vector3 start = startScale;
            Vector3 target = endScale;
            while (true) {
                Vector3 scale = Vector3.Lerp(start, target, timer / pulseTimer);

                transform.localScale = scale;
            
                timer += Time.deltaTime;
                if (timer >= pulseTimer) {
                    (start, target) = (target, start); //You learn something new about c# everyday lmao
                    timer = 0;
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
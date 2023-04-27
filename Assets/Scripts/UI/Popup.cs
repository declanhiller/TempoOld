using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class Popup : MonoBehaviour, IPressableUI {

    [SerializeField] private float scaleMultiplier;
    [SerializeField] private float pulseTimer;
    private Vector3 startScale;
    private Vector3 endScale;

    private IGlobalUI UiThatTriggeredThis;

    [SerializeField] private RectTransform buttonRectTransform;
    
    private void Awake() {
        startScale = buttonRectTransform.localScale;
        endScale = buttonRectTransform.localScale * scaleMultiplier;
    }
    

    public void Press(int playerNumber) {
        gameObject.SetActive(false);
    }

    IEnumerator ButtonScaler() {

        float timer = 0;
        Vector3 start = startScale;
        Vector3 target = endScale;
        while (true) {
            Vector3 scale = Vector3.Lerp(start, target, timer / pulseTimer);

            buttonRectTransform.localScale = scale;
            
            timer += Time.deltaTime;
            if (timer >= pulseTimer) {
                (start, target) = (target, start); //You learn something new about c# everyday lmao
                timer = 0;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnDisable() {
        StopCoroutine(ButtonScaler());
        buttonRectTransform.localScale = startScale;
        UIGlobalManager.INSTANCE.currentlySelectedUI = UiThatTriggeredThis;
    }

    private void OnEnable() {
        UiThatTriggeredThis = UIGlobalManager.INSTANCE.currentlySelectedUI;
        UIGlobalManager.INSTANCE.currentlySelectedUI = this;
        StartCoroutine(ButtonScaler());
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMapJoiner : MonoBehaviour, IPressableUI, IBackableUI {

    [SerializeField] private GameObject[] messages = new GameObject[4];
    [SerializeField] private GameObject letsBopMessage;
    [SerializeField] private GameObject backgroundBar;
    [SerializeField] private GameObject letsBopText;
    private PlayerManager playerManager;
    private bool isActive;
    private bool pressed;

    [Header("Lets Bop Animation Settings")] 
    [SerializeField] private float backgroundFadeDuration = 0.1f;
    [SerializeField] private Color targetBackgroundColor;
    [SerializeField] private float barScaleDuration = 0.2f;
    [SerializeField] private float letsBopTextOpacityDuration = 0.1f;
    [SerializeField] private float letsBopTextScaleDuration = 0.1f;
    [SerializeField] private float letsBopTextScaleOvershoot = 0.8f;
    [SerializeField] private float letsBopTextScaleOvershootDuration = 0.3f;

    private void Start() {
        pressed = false;
        playerManager = PlayerManager.INSTANCE;
    }

    public void Press(int playerNumber) {
        if (isActive) {
            if (pressed) return;
            pressed = true;
            GameManager.INSTANCE.GoToTowerOfHeaven();
        } else {
            messages[playerNumber].SetActive(true);
        }
        
    }

    private void CheckIfAllActive() {
        int counter = 0;
        foreach (GameObject message in messages) {
            if (message.activeInHierarchy) {
                counter++;
            }
        }

        if (counter <= 1) {
            if (isActive) {
                //run the animation to make the Let's bop dissapear
            }
            letsBopMessage.SetActive(false);
            isActive = false;
            return;
        }
        
        if (isActive != (counter == playerManager.players.Count)) {
            isActive = counter == playerManager.players.Count;
            letsBopMessage.SetActive(isActive);
            if (isActive) {
                PlayLetsBopAnimation();
            } else {
                //Play animation to make Let's bop dissapear
            }
        }
    }

    private void Update() {
        CheckIfAllActive();
    }

    public void Back(int playerNumber) {
        messages[playerNumber].SetActive(false);
    }

    public void PlayLetsBopAnimation() {
        StartCoroutine(ScaleText());
    }

    IEnumerator ScaleText() {
        RectTransform rectTransform = letsBopText.GetComponent<RectTransform>();
        Image image = letsBopText.GetComponent<Image>();
        Color startColor = new Color(1, 1, 1, 0);
        Color endColor = new Color(1, 1, 1, 1);
        Vector3 startScale = new Vector3(25, 25, 25);
        Vector3 firstTargetScale = new Vector3(letsBopTextScaleOvershoot, letsBopTextScaleOvershoot, letsBopTextScaleOvershoot);
        Vector3 actualTarget = new Vector3(1, 1, 1);
        rectTransform.localScale = startScale;
        image.color = startColor;
        
        
        letsBopText.SetActive(false);
        yield return StartCoroutine(ScaleBar());
        letsBopText.SetActive(true);
        
        
        float timer = 0;
        while (timer < letsBopTextScaleDuration) {
            Vector3 scale = Vector3.Lerp(startScale, firstTargetScale, timer / letsBopTextScaleDuration);
            rectTransform.localScale = scale;
            
            Color color = Color.Lerp(startColor, endColor, timer / letsBopTextScaleDuration);
            image.color = color;
            
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        rectTransform.localScale = firstTargetScale;

        timer = 0;
        while (timer < letsBopTextScaleOvershootDuration) {
            Vector3 scale = Vector3.Lerp(firstTargetScale, actualTarget, timer / letsBopTextScaleOvershootDuration);
            rectTransform.localScale = scale;

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();

        }

        rectTransform.localScale = actualTarget;

    }
    
    IEnumerator ScaleBar() {
        RectTransform rectTransform = backgroundBar.GetComponent<RectTransform>();
        Vector3 start = new Vector3(0, 1, 1);
        Vector3 target = new Vector3(1, 1, 1);
        rectTransform.localScale = start;
        yield return StartCoroutine(LetsBopBackgroundFade());
        
        float timer = 0;
        while (timer < barScaleDuration) {
            Vector3 scale = Vector3.Lerp(start, target, timer / barScaleDuration);
            rectTransform.localScale = scale;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        rectTransform.localScale = target;

    }
    
    IEnumerator LetsBopBackgroundFade() {
        Image background = letsBopMessage.GetComponent<Image>();
        Color startColor = new Color(1, 1, 1, 0);
        background.color = startColor;
        float backgroundTimer = 0;
        while (backgroundTimer < backgroundFadeDuration) {
            Color color = Color.Lerp(startColor, targetBackgroundColor, backgroundTimer / backgroundFadeDuration);
            background.color = color;
            backgroundTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        background.color = targetBackgroundColor;
    }
}

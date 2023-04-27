using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinMessage : MonoBehaviour, IPressableUI {

    [SerializeField] private GameObject background;
    [SerializeField] private GameObject backer;
    [SerializeField] private GameObject messageText;
    
    [Header("Animation Settings")] 
    [SerializeField] private float backgroundFadeDuration = 0.1f;
    [SerializeField] private Color targetBackgroundColor;
    [SerializeField] private float barScaleDuration = 0.2f;
    [SerializeField] private float textStartScale = 5f;
    [SerializeField] private float letsBopTextScaleDuration = 0.1f;
    [SerializeField] private float letsBopTextScaleOvershoot = 0.8f;
    [SerializeField] private float letsBopTextScaleOvershootDuration = 0.3f;

    public void Trigger(int playerNumberThatWon) {
        messageText.GetComponent<TextMeshProUGUI>().text = "P" + playerNumberThatWon + " Wins!";
        Color color = new Color(1, 1, 1);
        switch (playerNumberThatWon) {
            case 1:
                color = new Color(1, 0, 0);
                break;
            case 2:
                color = new Color(0, 0, 1);
                break;
            case 3:
                color = new Color(0, 1, 0);
                break;
            case 4:
                color = new Color(1, 0, 1);
                break;
        }

        StartCoroutine(ScaleText(color));

    }


    IEnumerator ScaleText(Color endColor) {
        RectTransform rectTransform = messageText.GetComponent<RectTransform>();
        TextMeshProUGUI image = messageText.GetComponent<TextMeshProUGUI>();
        Color startColor = new Color(1, 1, 1, 0);
        Vector3 startScale = new Vector3(textStartScale, textStartScale, textStartScale);
        Vector3 firstTargetScale = new Vector3(letsBopTextScaleOvershoot, letsBopTextScaleOvershoot, letsBopTextScaleOvershoot);
        Vector3 actualTarget = new Vector3(1, 1, 1);
        rectTransform.localScale = startScale;
        image.color = startColor;
        
        
        messageText.SetActive(false);
        yield return StartCoroutine(ScaleBar());
        messageText.SetActive(true);
        
        
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
        canPress = true;

    }
    
    
    IEnumerator ScaleBar() {
        RectTransform rectTransform = backer.GetComponent<RectTransform>();
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
        Image background = this.background.GetComponent<Image>();
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

    private bool canPress;
    public void Press(int playerNumber) {
        if (!canPress) return;
        SceneManager.LoadScene("MapSelectScene");
    }
}

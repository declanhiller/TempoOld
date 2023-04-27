using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BeatPulser : MonoBehaviour {
    [SerializeField] private Vector3 pulseScale;
    [SerializeField] private float pulseUpDuration;
    [SerializeField] private float pulseDownDuration;

    [SerializeField] private float spritePulseDuration;
    [SerializeField] private Image image;
    [SerializeField] private Sprite restingSprite;
    [SerializeField] private Sprite pulsingSprite;
    private Coroutine currentCoroutine;

    public void PulseUp() {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(PulseUpWee());
    }

    public void PulseDown()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(PulseDownWee());
        StartCoroutine(SpeakerPulse());
    }

    private IEnumerator SpeakerPulse() {
        image.sprite = pulsingSprite;
        yield return new WaitForSeconds(spritePulseDuration);
        image.sprite = restingSprite;
    }


    IEnumerator PulseUpWee() {
        
        Vector3 startScale = new Vector3(1, 1, 1);
        transform.localScale = startScale;
        Vector3 endScale = pulseScale;

        float startTime = (float) AudioSettings.dspTime;

        float timer = 0;
        while (timer < pulseUpDuration) {
            Vector3 scale = Vector3.Lerp(startScale, endScale, timer / pulseDownDuration);
            transform.localScale = scale;
            timer = (float) (AudioSettings.dspTime - startTime);
            yield return new WaitForEndOfFrame();
        }

        transform.localScale = endScale;

        yield return null;

    }

    IEnumerator PulseDownWee()
    {
        transform.localScale = pulseScale;
        Vector3 startScale = pulseScale;
        Vector3 endScale = new Vector3(1, 1, 1);

        float timer = 0;
        while (timer < pulseDownDuration) {
            Vector3 scale = Vector3.Lerp(startScale, endScale, timer / pulseDownDuration);
            transform.localScale = scale;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.localScale = endScale;
    }
    
}

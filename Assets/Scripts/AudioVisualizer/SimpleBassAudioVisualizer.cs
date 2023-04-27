using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBassAudioVisualizer : MonoBehaviour, IVisualizer {
    
    [SerializeField] private float startScale;
    [SerializeField] private float scaleMulti = 10;
    
    private float currentFreq;
    private float peakFreq;
    private float lastUpdate;

    [SerializeField] private float releaseTime = 2.5f;

    private void Start() {
    }

    // Update is called once per frame
    void Update() {

        currentFreq = Mathf.Lerp(peakFreq, 0, (Time.unscaledTime - lastUpdate) / releaseTime);
        
        float scale = currentFreq * scaleMulti / 10 + startScale;
        transform.localScale = new Vector3(scale, scale, scale);
    }

    void SetTarget(float targetFreq) {
        if (targetFreq > currentFreq) {
            peakFreq = targetFreq;
            currentFreq = targetFreq;
            lastUpdate = Time.unscaledTime;
        }
    }

    public void SetSpectrumData(float[] samples) {
        float[] freqBands = new float[8];
        int count = 0;
        for (int i = 0; i < 8; i++) {
            float average = 0;
            int sampleCount = (int) Mathf.Pow(2, i) * 2;
            if (i == 7) {
                sampleCount += 2;
            }
        
            for (int j = 0; j < sampleCount; j++) {
                average += samples[count] * (count + 1);
                count++;
            }
        
            average /= count;
        
            freqBands[i] = average;
        }
        
        float lowFreqBand = freqBands[1];
        SetTarget(lowFreqBand);
        
    }
}

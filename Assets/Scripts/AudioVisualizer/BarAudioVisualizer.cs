using System;
using UnityEngine;

public class BarAudioVisualizer : MonoBehaviour, IVisualizer {

    [SerializeField] private GameObject[] bars;

    [SerializeField] private Direction direction;

    [SerializeField] private float startScale;
    [SerializeField] private float scaleMultiplier;
    

    private float[] spectrumData;
    private float[] targetFreqBand = new float[8];
    private float[] currentFreqBand = new float[8];
    [SerializeField] private float toleranceRatio = 0.8f;
    [SerializeField] private float barDecreaseRate = 0.1f;

    private void Start() {
        
    }

    private void Update() {
        UpdateBands();
        DecreaseBands();
    }

    private void UpdateBands() {
        for (int i = 0; i < bars.Length; i++) {
            GameObject bar = bars[i];
            float xScale = bar.transform.localScale.x;
            float yScale = bar.transform.localScale.y;

            if (targetFreqBand[i] > currentFreqBand[i] && currentFreqBand[i] / targetFreqBand[i] < toleranceRatio) { //increase 
                currentFreqBand[i] = targetFreqBand[i];
            } else if (targetFreqBand[i] < currentFreqBand[i] && targetFreqBand[i] / currentFreqBand[i] < toleranceRatio) { //decrease
                currentFreqBand[i] = targetFreqBand[i];
            } else {
                return;
            }

            if (currentFreqBand[i] < 0) {
                currentFreqBand[i] = 0;
            }

            currentFreqBand[i] = targetFreqBand[i];
            
            float outputtedScale = currentFreqBand[i] * scaleMultiplier + startScale;
        
            switch (direction) {
                case Direction.X:
                    xScale = outputtedScale;
                    break;
                case Direction.Y:
                    yScale = outputtedScale;
                    break;
            }
            
            bar.transform.localScale = new Vector3(xScale, yScale, bar.transform.localScale.z);
        }

    }

    private void DecreaseBands() {
        for (int i = 0; i < bars.Length; i++) {
            GameObject bar = bars[i];
            float xScale = bar.transform.localScale.x;
            float yScale = bar.transform.localScale.y;
            
            
            float outputtedScale = (currentFreqBand[i] - barDecreaseRate * Time.deltaTime) * scaleMultiplier;
            if (outputtedScale < 0) {
                outputtedScale = 0;
            }
            
            switch (direction) {
                case Direction.X:
                    xScale = outputtedScale;
                    break;
                case Direction.Y:
                    yScale = outputtedScale;
                    break;
            }
            
            bar.transform.localScale = new Vector3(xScale, yScale, bar.transform.localScale.z);
        }
    }

    void MakeFrequencyBands() {
        int count = 0;
    
        for (int i = 0; i < 8; i++) {
    
            float average = 0;
            int sampleCount = (int) Mathf.Pow(2, i) * 2;
    
            if (i == 7) {
                sampleCount += 2;
            }

            for (int j = 0; j < sampleCount; j++) {
                average += spectrumData[count] * (count + 1);
                count++;
                average /= count;
                targetFreqBand[i] = average;
            }
        }
    }
    
    
    public void SetSpectrumData(float[] samples) {
        this.spectrumData = samples;
        MakeFrequencyBands();
    }
    
    
    enum Direction {
        X, Y
    }
    
}
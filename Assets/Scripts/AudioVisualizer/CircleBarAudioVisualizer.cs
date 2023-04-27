using System;
using UnityEngine;

public class CircleBarAudioVisualizer : MonoBehaviour, IVisualizer {

    [SerializeField] private Transform center;
    [SerializeField] private float radius;
    [SerializeField] private GameObject simpleBarPrefab;
    [SerializeField] private int numberOfBars;
    [SerializeField] private float scaleMultiplier;
    [SerializeField] private float startScale = 0f;
    private GameObject[] bars;
    private float[] freqData;
    private float[] spectrumData;

    private void Start() {
        bars = new GameObject[numberOfBars];
        freqData = new float[numberOfBars];
        float degree = 0;
        for (int i = 0; i < numberOfBars; i++) {
            GameObject instantiate = Instantiate(simpleBarPrefab);
            instantiate.transform.SetParent(transform);
            Vector2 spawnPos =
                new Vector2(radius * Mathf.Cos(degree * Mathf.Deg2Rad) + center.position.x, 
                    radius * Mathf.Sin(degree * Mathf.Deg2Rad) + center.position.y);
            instantiate.transform.position = spawnPos;

            instantiate.transform.up = instantiate.transform.position - center.transform.position;

            bars[i] = instantiate;
            
            degree += 360f / numberOfBars;
        }
    }
    
    private void Update() {
        UpdateVisuals();
    }

    private void UpdateVisuals() {
        for (int i = 0; i < bars.Length; i++) {
            float outputtedScale = freqData[i] * scaleMultiplier + startScale;
            bars[i].transform.localScale = new Vector3(bars[i].transform.localScale.x, outputtedScale,
                bars[i].transform.localScale.z);
        }
    }

    private void MakeFrequencyBands() {
        int numberOfBarsPer = spectrumData.Length / numberOfBars;
        int spectrumDataIndexTracker = 0;
        for (int i = 0; i < bars.Length; i++) {
            int tempSpectrumDataIndexTracker = spectrumDataIndexTracker;
            float average = 0;
            for (int j = tempSpectrumDataIndexTracker; j < tempSpectrumDataIndexTracker + numberOfBarsPer; j++) {
                average += spectrumData[j];
                spectrumDataIndexTracker = j;
            }

            freqData[i] = average / numberOfBarsPer;
        }
    }

    public void SetSpectrumData(float[] samples) {
        spectrumData = samples;
        MakeFrequencyBands();
    }
}
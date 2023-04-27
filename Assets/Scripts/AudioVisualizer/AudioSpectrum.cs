using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class AudioSpectrum : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public static float[] samples = new float[512];

    [SerializeReference] private GameObject[] visualizersObjects;
    private IVisualizer[] visualizers;

    private void Awake() {
        visualizers = new IVisualizer[visualizersObjects.Length];
        for (int i = 0; i < visualizersObjects.Length; i++) {
            visualizers[i] = visualizersObjects[i].GetComponent<IVisualizer>();
        }
    }


    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        foreach (IVisualizer visualizer in visualizers) {
            visualizer.SetSpectrumData(samples);
        }
    }

    void GetSpectrumAudioSource() {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }


    
}

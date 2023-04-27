using System;
using System.Collections;
using System.Collections.Generic;
using AudioFramework;
using UnityEngine;
using UnityEngine.Events;

public class Beat : MonoBehaviour
{
    //Song beats per minute
//This is determined by the song you're trying to sync up to
    [SerializeField] private float songBpm;
    [SerializeField, Tooltip("In seconds")] private float songOffset;
    
    [SerializeField] private AudioSource music;

    [SerializeField] private Interval[] intervals;

    private void Start() {
        // music.Play();
        
        music.PlayScheduled(AudioSettings.dspTime + 2);
        
    }

    private void Update() {
        foreach (Interval interval in intervals) {
            float sampledTime = ((music.timeSamples) / (music.clip.frequency * interval.GetIntervalLength(songBpm))) - songOffset;
            interval.CheckForNewInterval(sampledTime);
        }
        
    }
}

[Serializable]
public class Interval {
    [SerializeField] private float steps;
    [SerializeField] private UnityEvent trigger;
    [SerializeField] private HitsoundTester tester;
    private int lastInterval;

    public float GetIntervalLength(float bpm) {
        return 60f / (bpm * steps);
    }

    public void CheckForNewInterval(float interval) {
        if (Mathf.FloorToInt(interval) != (lastInterval)) {
            Debug.Log("Interval: " + interval + ", Last Interval: " + lastInterval);
            lastInterval = Mathf.FloorToInt(interval);
            // trigger.Invoke();
        }
    }
    
}

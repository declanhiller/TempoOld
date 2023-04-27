using System;
using System.Collections;
using System.Collections.Generic;
using AudioFramework;
using UnityEngine;

public class MetronomeBar : MonoBehaviour {

    private Metronome metronome;
    
    public void Setup(Metronome metronome, float startTime, float endTime, Vector3 startPosition, Vector3 endPosition) {
        this.metronome = metronome;
        StartCoroutine(Move(startTime, endTime, startPosition, endPosition));
    }

    IEnumerator Move(float startTime, float endTime, Vector3 startPosition, Vector3 endPosition) {
        
        float songPosition = metronome.songPosition;
        
        while (songPosition <= endTime) {
            Vector3 positionOfBar = Vector3.Lerp(startPosition, endPosition, (songPosition - startTime) / (endTime - startTime));
            transform.position = positionOfBar;
            songPosition = metronome.songPosition;
            yield return new WaitForEndOfFrame();
        }
        
        Destroy(gameObject);
    }
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using AudioFramework;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameStartCountdown : MonoBehaviour {

    public UnityEvent OnComplete;
    public UnityEvent OnNextBeat; //maybe?????

    [SerializeField] private TextMeshProUGUI text;
    
    // [SerializeField] private float 

    [SerializeField] private Color countingDownColor;
    [SerializeField] private Color goColor;


    private Metronome metronome;
    private int counter = 0;

    private void Start() {
        text.color = countingDownColor;
        metronome = GameObject.FindGameObjectWithTag("Metronome").GetComponent<Metronome>();
        text.text = "";
    }

    public void Count() {
        counter++;
        switch (counter) {
            case 1:
                text.text = "1";
                break;
            case 2:

                text.text = "2";
                break;
            case 3:
                text.text = "3";
                break;
            case 4:
                text.color = goColor;
                text.text = "GO";
                metronome.StartGamePlayBeats();
                OnComplete.Invoke();
                break;
            case 5:
                text.text = "";
                metronome.OnBeat.RemoveListener(Count);
                gameObject.SetActive(false);
                break;
        }
    }

}

using System;
using UnityEngine.Events;

namespace AudioFramework {
    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [RequireComponent(typeof(AudioSource))]
    public class Metronome : MonoBehaviour {

        [SerializeField] private float accuracy = 0.1f;

        [SerializeField] private AudioSource musicSource;

        [SerializeField] private float bpm = 140.0F;
        public float secPerBeat { get; private set; }
        
        [SerializeField] private float offset;

        private float dspStartTime;

        public float songPosition { get; private set; }

        private float lastBeatPosition;
        private float nextBeatPosition;

        //Triggers all the time
        public UnityEvent OnBeat;

        private bool countdownFinished;
        //Only triggers after countdown
        public UnityEvent OnGameplayBeat;

        [SerializeField]
        private float beforeBeatTriggerTime;
        public UnityEvent OnBeforeBeatATinyBit;


        private bool hasStarted;
        
        
        private void Start() {
            
        }

        public void Startup() {
            secPerBeat = 60f / bpm;
            dspStartTime = (float) AudioSettings.dspTime;
            musicSource.Play();
            hasStarted = true;
        }
        
        
        void Update() {
            if (!hasStarted) return;
            songPosition = (float) (AudioSettings.dspTime - dspStartTime) - offset;

            if (songPosition > nextBeatPosition - beforeBeatTriggerTime) {
                if (countdownFinished) {
                    OnBeforeBeatATinyBit.Invoke();
                }
            }
            
            if (songPosition > nextBeatPosition) {
                lastBeatPosition = nextBeatPosition;
                nextBeatPosition += secPerBeat;
                OnBeat.Invoke();
                if (countdownFinished) {
                    OnGameplayBeat.Invoke();
                }
            }
            

            
        }

        
        public Accuracy CheckHit() {

            if ((songPosition >= lastBeatPosition && songPosition <= lastBeatPosition + accuracy) || 
                (songPosition <= nextBeatPosition && songPosition >= nextBeatPosition + accuracy)) {
                return Accuracy.THREE_HUNDRED;
            }
            
            return Accuracy.MISS;
        }


        public enum Accuracy {
            THREE_HUNDRED, ONE_HUNDRED, FIFTY, MISS
        }

        public void StartGamePlayBeats() {
            countdownFinished = true;
        }
    }
}
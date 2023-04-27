using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AudioFramework {
    public class HitsoundTester : MonoBehaviour {
        [SerializeField] private AudioSource hitsound;
        [SerializeField] private Metronome metronome;
        

        public void Play(InputAction.CallbackContext context) {
            if (!context.performed) return;
            Metronome.Accuracy checkHit = metronome.CheckHit();
            if (checkHit == Metronome.Accuracy.THREE_HUNDRED) {
                hitsound.Play();
            }
            
        }

    }
}
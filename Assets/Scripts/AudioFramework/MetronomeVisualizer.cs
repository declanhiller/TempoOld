using System;
using UnityEngine;

namespace AudioFramework {
    public class MetronomeVisualizer : MonoBehaviour {
        [SerializeField] private GameObject leftBar;
        [SerializeField] private GameObject rightBar;
        [SerializeField] private Metronome metronome;

        [SerializeField] private GameObject barParent;

        [SerializeField] private int beatsToLast = 4;
        
        [SerializeField] private Transform rightStart;
        [SerializeField] private Transform leftStart;
        [SerializeField] private Transform end;

        private void Start() {
            metronome.OnBeat.AddListener(OnBeat);
        }

        public void OnBeat() {
            GameObject rightBarObj = Instantiate(rightBar, rightStart.position, Quaternion.identity, barParent.transform);
            GameObject leftBarObj = Instantiate(leftBar, leftStart.position, Quaternion.identity, barParent.transform);

            MetronomeBar rightMetronomeBar = rightBarObj.GetComponent<MetronomeBar>();
            rightMetronomeBar.Setup(metronome, metronome.songPosition, metronome.songPosition + (metronome.secPerBeat * beatsToLast), rightStart.position, end.position);
            
            MetronomeBar leftMetronomeBar = leftBarObj.GetComponent<MetronomeBar>();
            leftMetronomeBar.Setup(metronome, metronome.songPosition, metronome.songPosition + (metronome.secPerBeat * beatsToLast), leftStart.position, end.position);
            
        }

    }
}
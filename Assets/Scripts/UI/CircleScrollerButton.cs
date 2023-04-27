using UnityEngine;
using UnityEngine.Events;

namespace UI {
    public class CircleScrollerButton : MonoBehaviour {
        [SerializeField] private UnityEvent OnSubmit;

        public void CallButton() {
            OnSubmit.Invoke();
        }
        
    }
}
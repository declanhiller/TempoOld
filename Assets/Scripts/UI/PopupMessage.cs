using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class PopupMessage : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        private void Start()
        {
            Invoke(nameof(Disappear), 3f);
        }

        void Disappear()
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        public void SetMessage(string message)
        {
            text.text = message;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNumber : MonoBehaviour {
    
    
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject arrow;
    [SerializeField] private Transform playerTarget;
    private Camera camera;

    private void Start() {
        camera = Camera.main;
    }

    public void InitPlayerNumber(Transform target, string playerNumber, Color playerColor) {
        text.color = playerColor;
        arrow.GetComponent<Image>().color = playerColor;
        text.text = playerNumber;
        playerTarget = target;
    }

    private void LateUpdate() {
        transform.position = camera.WorldToScreenPoint(playerTarget.position);
    }
}

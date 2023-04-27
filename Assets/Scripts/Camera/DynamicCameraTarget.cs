using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCameraTarget : MonoBehaviour
{
    public float speed { get; private set; }
    public Vector3 position { get; private set; }

    [SerializeField] public TargetType targetType;
    
    private void Start() {
        position = transform.position;
    }

    private void Update() {
        speed = (position - transform.position).magnitude;
        position = transform.position;
    }

    public enum TargetType {
        PLAYER, DEATH_EFFECT
    }
}

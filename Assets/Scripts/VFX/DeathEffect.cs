using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DeathEffect : MonoBehaviour {
    [Header("Linear Particle System Settings")]
    [SerializeField] private ParticleSystem firstLinearSystem;
    [SerializeField] private ParticleSystem secondLinearSystem;
    [SerializeField] private ParticleSystem explosionSystem;
    [SerializeField] private float boundsForLinearEffect = 0.3f;
    [SerializeField] private float repeatTimerForLinearSystem = 0.3f;
    [SerializeField] private float minSize = 0.1f;
    [SerializeField] private float maxSize = 0.2f;
    [Space(10f)]
    [Header("Explosion Particle System Settings")]
    [SerializeField] private float placeholder;
    
    private void Start() {
        ParticleSystem.ShapeModule firstEdge = firstLinearSystem.shape;
        firstEdge.position = new Vector3(firstEdge.position.x, -boundsForLinearEffect, firstEdge.position.z);
        ParticleSystem.ShapeModule secondEdge = secondLinearSystem.shape;
        secondEdge.position = new Vector3(secondEdge.position.x, boundsForLinearEffect, secondEdge.position.z);
        ParticleSystem.MainModule firstMain = firstLinearSystem.main;
        ParticleSystem.MainModule secondMain = secondLinearSystem.main;
        float startSize = (minSize + maxSize) / 2;
        firstMain.startSize = startSize;
        secondMain.startSize = startSize;
    }

    
    public void Play(Vector2 position) {
        transform.position = position;
        firstLinearSystem.Play();
        secondLinearSystem.Play();
        explosionSystem.Play();
        StartCoroutine(LinearEffect(firstLinearSystem, false, true));
        StartCoroutine(LinearEffect(secondLinearSystem, true, false));
    }

    private IEnumerator LinearEffect(ParticleSystem system, bool goLeft, bool goUp) {
        float positionTimer = 0;
        float sizeTimer = repeatTimerForLinearSystem / 2;
        ParticleSystem.ShapeModule edge = system.shape;
        ParticleSystem.MainModule main = system.main;
        float startPosition = goLeft ? boundsForLinearEffect : -boundsForLinearEffect;
        float targetPosition = goLeft ? -boundsForLinearEffect : boundsForLinearEffect;
        float startSize = goUp ? minSize : maxSize;
        float targetSize = goUp ? maxSize : minSize;
        while (system.isPlaying) {
            float position = Mathf.Lerp(startPosition, targetPosition, positionTimer / repeatTimerForLinearSystem);
            float size = Mathf.Lerp(startSize, targetSize, sizeTimer / repeatTimerForLinearSystem);
            edge.position = new Vector3(edge.position.x, position, edge.position.z);
            main.startSize = size;
            positionTimer += Time.deltaTime;
            sizeTimer += Time.deltaTime;
            if (positionTimer >= repeatTimerForLinearSystem) {
                (targetPosition, startPosition) = (startPosition, targetPosition);
                positionTimer = 0;
            }
            
            if (sizeTimer >= repeatTimerForLinearSystem) {
                (startSize, targetSize) = (targetSize, startSize);
                sizeTimer = 0;
            }
            
            yield return new WaitForEndOfFrame();
        }
    }
}

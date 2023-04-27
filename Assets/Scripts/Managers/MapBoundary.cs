using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;

public class MapBoundary : MonoBehaviour {
    
    [SerializeField] private GameObject deathEffectPrefab;
    private BoxCollider2D collider;
    public bool isActive;

    private void Start() {
        isActive = true;
        collider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (!isActive) return;
        TriggerDeathEffect(other.transform.position);
        other.GetComponent<HurtboxController>().KillPlayer();
    }

    private void TriggerDeathEffect(Vector3 position) {
        GameObject instantiate = Instantiate(deathEffectPrefab, position, Quaternion.identity);
        if (position.x >= collider.bounds.max.x) {
            instantiate.transform.rotation = Quaternion.Euler(0, 0, 90);
        } else if (position.x <= collider.bounds.min.x) {
            instantiate.transform.rotation = Quaternion.Euler(0, 0, 270);
        } else if (position.y >= collider.bounds.max.y) {
            instantiate.transform.rotation = Quaternion.Euler(0, 0, 180);
        } else if (position.y <= collider.bounds.min.y) {
            instantiate.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        instantiate.GetComponent<DeathAnimationController>().Play();
    }
}

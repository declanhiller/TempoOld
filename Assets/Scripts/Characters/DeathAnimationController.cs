using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAnimationController : MonoBehaviour {

    // [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private Animator animator;
    [SerializeField] private float length;

    public void Play() {
        Camera.main.GetComponent<DynamicCamera>().AddTarget(GetComponent<DynamicCameraTarget>());
        animator.SetTrigger("Play");
        Invoke(nameof(StopTarget), length);
    }

    private void StopTarget() {
        Camera.main.GetComponent<DynamicCamera>().RemoveTarget(GetComponent<DynamicCameraTarget>());
        Destroy(gameObject);
    }
    
}

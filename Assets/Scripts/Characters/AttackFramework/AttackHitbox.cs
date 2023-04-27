using System;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.AttackFramework {
    
    [RequireComponent(typeof(Collider2D))]
    public abstract class AttackHitbox : MonoBehaviour {

        public Action<GameObject> OnHit;
        [SerializeField] protected HurtboxController attachedHurtbox;

        private void Awake() {
            
        }


        private void Update() {
            List<HurtboxController> checkForHit = CheckForHit();
            if (checkForHit.Count == 0) return;
            foreach (HurtboxController hurtboxController in checkForHit) {
                OnHit.Invoke(hurtboxController.gameObject);
            }
        }

        public virtual List<HurtboxController> CheckForHit() {
            throw new NotImplementedException();
        }

        // private void OnTriggerEnter2D(Collider2D other) {
        //     if (other == playerHitBoxIsAttachedToo) return;
        //     Debug.Log("Hit: " + other.transform.parent.name);
        //     // OnHit?.Invoke(other.gameObject);
        // }

        // private void OnTriggerStay2D(Collider2D other) {
        //     if (other == playerHitBoxIsAttachedToo) return;
        //     Debug.Log("No fucking way");
        // }
        //
        // private void OnEnable() {
        //     Debug.Log("Active hitbox");
        //     hitbox.enabled = true;
        // }
        //
        // private void OnDisable() {
        //     Debug.Log("Inactive hitbox");
        //     hitbox.enabled = false;
        // }
        
    }
}
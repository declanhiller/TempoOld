using System;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.AttackFramework {
    public class CircleHitbox : AttackHitbox {
        
        [SerializeField] private CircleCollider2D collider;
        [SerializeField] protected LayerMask hurtboxMask;


        public override List<HurtboxController> CheckForHit() {
            Collider2D[] collisions = Physics2D.OverlapCircleAll(collider.offset + (Vector2) collider.gameObject.transform.position,
                collider.radius, hurtboxMask);
            List<HurtboxController> hurtboxes = new List<HurtboxController>();
            foreach (Collider2D collider in collisions) {
                HurtboxController hurtbox = collider.GetComponent<HurtboxController>();
                if (hurtbox == attachedHurtbox) continue;
                hurtboxes.Add(hurtbox);
            }
            return hurtboxes;
        }
    }
}
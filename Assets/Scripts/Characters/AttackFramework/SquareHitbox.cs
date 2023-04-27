using System;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.AttackFramework {
    public class SquareHitbox : AttackHitbox {

        [SerializeField] private BoxCollider2D collider;
        [SerializeField] protected LayerMask hurtboxMask;

        public override List<HurtboxController> CheckForHit()
        {
            Vector2 position;
            if (transform.parent.localScale.x > 0)
            {
                position = (Vector2) collider.gameObject.transform.position + collider.offset;
            }
            else
            {
                position = (Vector2) collider.gameObject.transform.position;
                position = new Vector2(position.x - collider.offset.x, position.y);
            }
            Collider2D[] collisions = Physics2D.OverlapBoxAll(position, collider.size, 0, hurtboxMask);
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
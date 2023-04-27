
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using AudioFramework;
using Characters.AttackFramework;
using Unity.VisualScripting;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Characters.TamborineTurtle.Attacks
{
    
    [Serializable]
    //This entire class is a fuck u to everything cool about programming
    //Be my guest if ya wanna read and lose braincells
    public class ClapAttack : IAttack {

        [SerializeField] private float windUpTime;
        [SerializeField] private float activeTime;
        [SerializeField] private float recoveryTime;
        [SerializeField] private float givenHitstun;
        [SerializeField] private float damage;
        [SerializeField] private float knockbackMulti;
        [SerializeField] private float minKnockbackForce;
        private float chargeMinimum = 1;
        [SerializeField] private float chargeMaximum;
        [SerializeField] private float chargeRate;
        [SerializeField] private Color failedAttackColor;
        private float chargeMultiplier;
        [SerializeField] private AttackHitbox hitbox;
        [SerializeField] private float endTimeWhereButtonsCanBeQueued;
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private SpriteRenderer renderer;

        private float startTime; //update all data based off of start time instead of increments
        
        private List<GameObject> alreadyCollidedPlayers = new List<GameObject>();

        private AttackState attackState;
        
        private float timeElapsedSinceStart;

        private float totalLength;
        
        private bool shouldExit;

        private bool failedPress;
        private bool failed;
        
        public void Enter(Metronome.Accuracy accuracy)
        {
            
            released = false;
            chargeMultiplier = chargeMinimum;
            charged = false;

            triggered = false;
            
            alreadyCollidedPlayers.Clear();
            startTime = Time.time;
            timeElapsedSinceStart = 0;
            attackState = AttackState.STARTUP;
            movement.StartAttack();
            shouldExit = false;
            totalLength = windUpTime + activeTime + recoveryTime;
            hitbox.OnHit = HitPlayer;
            animator.SetBool("InAttackAnimation", true);
            animator.SetTrigger("StartTurtleClap");
            if (accuracy == Metronome.Accuracy.MISS) {
                renderer.color = failedAttackColor;
                failedPress = true;
                failed = true;
            } else {
                failedPress = false;
                failed = false;
            }
        }
        
        public void Tick() {
            timeElapsedSinceStart = Time.time - startTime;
            
            switch (attackState) {
                case AttackState.STARTUP:
                    StartupTick();
                    break;
                case AttackState.ACTIVE:
                    ActiveTick();
                    break;
                case AttackState.RECOVERY:
                    RecoveryTick();
                    break;
            }
        }

        private bool triggered;
        
        private void RecoveryTick() {
            if (Time.time - startTime >= windUpTime + activeTime + recoveryTime) { //End Recovery
                Debug.Log("End Recovery");
                shouldExit = true;
            }
        }

        private void ActiveTick() {
            // if (charged) {
            //     if (Time.time - startTime >= activeTime) { //End Active
            //         hitbox.gameObject.SetActive(false);
            //         attackState = AttackState.RECOVERY;
            //     }
            // } else {
            //     if (Time.time - startTime >= activeTime + windUpTime) { //End Active
            //         hitbox.gameObject.SetActive(false);
            //         attackState = AttackState.RECOVERY;
            //     }
            // }
            
            // Debug.Log(hitbox.co);
            
            if (Time.time - startTime >= windUpTime + activeTime) { //End Active
                Debug.Log("End Active");
                attackState = AttackState.RECOVERY;
            }

        }


        public void ActivateHitbox() {
            if (failed) return;
            hitbox.gameObject.SetActive(true);
        }

        public void DeactiveHitbox() {
            if (failed) return;
            hitbox.gameObject.SetActive(false);
        }
        

        private bool charged;
        private void StartupTick() {
            
            if (Time.time - startTime >= windUpTime) {
                
                Debug.Log("End Windup");
                
                
                if (chargeMultiplier < chargeMaximum) {
                    chargeMultiplier += chargeRate * Time.deltaTime;
                } else { 
                    chargeMultiplier = chargeMaximum;
                }
                
                if (released || failedPress) {
                    if (charged && failedRelease) {
                        renderer.color = failedAttackColor;
                        failed = true;
                    }
                    animator.SetTrigger("TurtleClapRelease");
                    attackState = AttackState.ACTIVE;
                }
                else {
                    charged = true;
                }
            }
        }

        public void HitPlayer(GameObject player) {
            foreach (GameObject alreadyCollidedPlayer in alreadyCollidedPlayers) {
                if (alreadyCollidedPlayer == player) return;
            }
            alreadyCollidedPlayers.Add(player);
            HurtboxController hurtbox = player.GetComponent<HurtboxController>();
            hurtbox.TakeDamage(damage);
            float xScale = hitbox.transform.parent.localScale.x;
            int scale = xScale > 0 ? 1 : -1;
            hurtbox.TakePercentageKnockback(new Vector2(scale, 0.3f).normalized, knockbackMulti * chargeMultiplier, minKnockbackForce, givenHitstun);
        }

        public void Exit() {


            renderer.color = new Color(1, 1, 1, 1);
            movement.EndAttack();
            animator.SetBool("InAttackAnimation", false);
        }

        public bool ShouldExit()
        {
            return shouldExit;
        }
        

        public bool CanQueueMoveAfterwards() {
            return false;
        }

        public void QueueMove(Action move) {
            
        }

        private bool released;
        private bool failedRelease;
        public void Release(Metronome.Accuracy accuracy) {
            released = true;
            if (accuracy == Metronome.Accuracy.MISS) {
                Debug.Log("failed release");
                failedRelease = true; 
            } else {
                Debug.Log("succeeded release");
                failedRelease = false;
            }
            // Debug.Log("Button released");
        }
    }
}
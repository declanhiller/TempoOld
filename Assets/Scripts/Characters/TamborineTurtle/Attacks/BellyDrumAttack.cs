using System;
using System.Collections.Generic;
using AudioFramework;
using Characters.AttackFramework;
using Unity.VisualScripting;
using UnityEngine;

namespace Characters.TamborineTurtle.Attacks {
    [Serializable]
    public class BellyDrumAttack : IAttack{
        [SerializeField] private float windUpTime;
        [SerializeField] private float activeTime;
        [SerializeField] private float recoveryTime;
        [SerializeField] private float givenHitstun;
        [SerializeField] private float damage;
        [SerializeField] private float fixedKnockback;
        [SerializeField] private Color failedAttackColor;

        [SerializeField] private AttackHitbox hitbox;
        [SerializeField] private float endTimeWhereButtonsCanBeQueued;
        
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private SpriteRenderer renderer;

        private float startTime; //update all data based off of start time instead of increments

        private Action queuedMove;

        private List<GameObject> alreadyCollidedPlayers = new List<GameObject>();

        private AttackState attackState;
        
        private float timeElapsedSinceStart;

        private float totalLength;
        
        private bool shouldExit;
        private bool canQueueMoves;
        
        private bool failedAttack;

        
        public void Enter(Metronome.Accuracy accuracy)
        {
            alreadyCollidedPlayers.Clear();
            startTime = Time.time;
            timeElapsedSinceStart = 0;
            attackState = AttackState.STARTUP;
            shouldExit = false;
            canQueueMoves = false;
            totalLength = windUpTime + activeTime + recoveryTime;
            hitbox.OnHit = HitPlayer;
            movement.StartAttack();
            animator.SetBool("InAttackAnimation", true);
            animator.SetTrigger("GroundBellyDrum");
            if (accuracy == Metronome.Accuracy.MISS) {
                Debug.Log("Failed attack");
                renderer.color = failedAttackColor;
                failedAttack = true;
            } else {
                failedAttack = false;
            }
        }

        public void Tick() {
            timeElapsedSinceStart = Time.time - startTime;

            if (totalLength - timeElapsedSinceStart <= endTimeWhereButtonsCanBeQueued) {
                canQueueMoves = true;
            }
            
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

        private void RecoveryTick() {
            if (Time.time - startTime >= windUpTime + activeTime + recoveryTime) { //End Recovery
                shouldExit = true;
            }
        }

        private void ActiveTick() {
            if (timeElapsedSinceStart >= windUpTime + activeTime) { //End Active
                if (!failedAttack) {
                    hitbox.gameObject.SetActive(false);
                }
                attackState = AttackState.RECOVERY;
            }
        }

        private void StartupTick() {
            if (timeElapsedSinceStart >= windUpTime) { //End windup
                if (!failedAttack) {
                    hitbox.gameObject.SetActive(true);
                }
                attackState = AttackState.ACTIVE;
            }
        }

        public void HitPlayer(GameObject player) {
            foreach (GameObject alreadyCollidedPlayer in alreadyCollidedPlayers) {
                if (alreadyCollidedPlayer == player) return;
            }
            alreadyCollidedPlayers.Add(player);

            Vector2 forceDirection = ((player.transform.position - hitbox.transform.position)).normalized;
            
            HurtboxController hurtbox = player.GetComponent<HurtboxController>();
            hurtbox.TakeDamage(damage);
            hurtbox.TakeFixedKnockBack(forceDirection, fixedKnockback, givenHitstun);
        }

        public void Exit() {
            renderer.color = new Color(1, 1, 1, 1);
            movement.EndAttack();
            animator.SetBool("InAttackAnimation", false);
            alreadyCollidedPlayers.Clear();
            queuedMove?.Invoke();
        }

        public bool ShouldExit()
        {
            return shouldExit;
        }
        

        public bool CanQueueMoveAfterwards() {
            return false;
        }

        public void QueueMove(Action move) {
            queuedMove = move;
        }
    }
}
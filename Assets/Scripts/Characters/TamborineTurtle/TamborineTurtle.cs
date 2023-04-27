using System;
using System.Collections;
using AudioFramework;
using Characters.AttackFramework;
using Characters.TamborineTurtle.Attacks;
using Player;
using UnityEngine;

namespace Characters.TamborineTurtle
{
    //TODO: so much hardcode :(
    //Literally how complex input behavior is handled makes me want to crawl in a hole and die
    
    //also didnt realized how fucking bad the state machine impl was until i started doing the special attack lmao
    public class TamborineTurtle : CharacterBase
    {
        [Header("Necessary Attack Components")]
        private IAttack currentAttack;
        [SerializeField] private TailWhipAttack whipAttack;
        [SerializeField] private AerialTailWhipAttack aerialWhipAttack;
        [SerializeField] private BellyDrumAttack aoeAttack;
        [SerializeField] private AerialBellyDrumAttack aerialBellyDrumAttack;
        [SerializeField] private ClapAttack clapAttack;
        [SerializeField] private PlayerMovement movementController;
        private PlayerCharacterContainer container;
        public Metronome metronome;

        [Header("Attack Audio Components")]
        [SerializeField] private AudioSource attackAudioSource;
        [SerializeField] private AudioClip tailWhipSound;
        [SerializeField] private AudioClip clapAttackSound;
        [SerializeField] private AudioClip bellyDrumSound;
        
        private void Start() {
            hurtbox.OnDeath += Respawn;
            container = GetComponentInParent<PlayerCharacterContainer>();
            container.OnLightAttackTrigger += OnLightAttack;
            container.OnSpecialAttackTrigger += OnSpecialAttack;
            container.OnHeavyAttackTrigger += OnHeavyAttack;
            container.OnHeavyAttackRelease += OnHeavyRelease;
        }


        private void OnLightAttack()
        {
            if (currentAttack == null) {
                LightAttack();
                return;
            }
        }

        private void LightAttack() {
            Debug.Log("Light Attack!");
            
            if (movementController.IsConsideredGrounded()) {
                whipAttack.Enter(metronome.CheckHit());
                currentAttack = whipAttack;
            } else {
                aerialWhipAttack.Enter(metronome.CheckHit());
                currentAttack = aerialWhipAttack;
            }
            
        }
        
        private void Update() {
            if (currentAttack == null) return;
            if (currentAttack.ShouldExit()) {
                currentAttack.Exit();
                currentAttack = null;
            } else {
                currentAttack.Tick();
            }
        }


        private void OnSpecialAttack()
        {
            if (currentAttack == null) {
                SpecialAttack();
                return;
            }
        }

        private void SpecialAttack() {
            Debug.Log("Special Attack!");
            
            if (movementController.IsConsideredGrounded()) {
                aoeAttack.Enter(metronome.CheckHit());
                currentAttack = aoeAttack;
            } else {
                aerialBellyDrumAttack.Enter(metronome.CheckHit());
                currentAttack = aerialBellyDrumAttack;
            }

        }

        private void OnHeavyAttack()
        {
            if (currentAttack == null) {
                HeavyAttack();
                return;
            }
        }

        //this is a fix because I have no fucking clue anymore bro
        public void OnHeavyAttackAnimationStart() {
            clapAttack.ActivateHitbox();
        }

        public void OnHeavyAttackHitboxEnd() {
            clapAttack.DeactiveHitbox();
        }

        private void HeavyAttack() {
            Debug.Log("Heavy Attack");
            clapAttack.Enter(metronome.CheckHit());
            currentAttack = clapAttack;
        }

        private void OnHeavyRelease() {
            ClapAttack attack = currentAttack as ClapAttack;
            
            attack?.Release(metronome.CheckHit());
        }

        public override void HitByAttack() {
            if (currentAttack == null) return;
            currentAttack.Exit();
            currentAttack = null;
        }

        public void PlayTailWhipSFX() // used for animation events
        {
            attackAudioSource.PlayOneShot(tailWhipSound);
        }
        
        public void PlayClapAttackSFX() // used for animation events
        {
            attackAudioSource.PlayOneShot(clapAttackSound);
        }
        
        public void PlayBellyDrumSFX() // used for animation events
        {
            attackAudioSource.PlayOneShot(bellyDrumSound);
        }
    }
}
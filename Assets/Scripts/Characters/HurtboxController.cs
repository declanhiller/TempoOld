using System;
using System.Collections;
using UnityEngine;
using Random = System.Random;

namespace Characters {
    [RequireComponent(typeof(Collider2D))]
    public class HurtboxController : MonoBehaviour {
        [Header("Necessary Hurtbox Components")]
        public float percentage;
        [SerializeField] private Rigidbody2D rb;
        private bool isInHitstun;
        [SerializeField] private float dragAfterHit = 10;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private DeathEffect deathEffect;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private CharacterBase character;
        public DynamicCamera dynamicCamera;
        
        public PlayerStatTracker StatTracker { private get; set; }
        public event Action OnDeath;
        public int deathTotal { get; private set; }

        [Header("Getting Hit Audio Components")] 
        [SerializeField] private AudioSource getHitAudioSource;
        [SerializeField] private AudioClip[] hitAudioClips = new AudioClip[3];
        [SerializeField] private AudioClip deathSFX;
        private Random rand = new Random();

        private void Start() {
            deathTotal = 0;
            percentage = 0;
        }
        public void KillPlayer() {
            // deathEffect.Play(transform.position);
            getHitAudioSource.PlayOneShot(deathSFX);
            dynamicCamera.PlayPlayerDieShake();
            percentage = 0;
            StatTracker.UpdatePercentage(0);
            rb.velocity = Vector2.zero;
            StatTracker.UpdateDeath(++deathTotal);
            OnDeath?.Invoke();
        }

        //funny var name :D
        public void TakeDamage(float owiedamage) {
            getHitAudioSource.PlayOneShot(hitAudioClips[RandomClip()]);
            dynamicCamera.PlayPlayerHitShake();
            percentage += owiedamage;
            StatTracker.UpdatePercentage(percentage);
        }

        private int RandomClip()
        {
            int clipNumber = rand.Next(0, hitAudioClips.Length - 1);
            return clipNumber;
        }
        
        public void TakeFixedKnockBack(Vector2 forceDirection, float knockbackForce, float hitStunLength) {
            rb.velocity = Vector2.zero;
            // rb.AddForce(new Vector2(horizontalForce,verticalForce), ForceMode2D.Impulse);
            // isInHitstun = true;
            
            if (forceDirection.y < 0) {
                RaycastHit2D hit = Physics2D.Raycast(rb.position, forceDirection, 1.5f, groundMask);
                if (hit.collider != null) {
                    if (Math.Abs(hit.normal.y) > Math.Abs(hit.normal.x * 1.5f)) {
                        forceDirection.Set(forceDirection.x, 0);
                        forceDirection = forceDirection.normalized;
                    }
                }
            }
            
            playerMovement.HitByAttack(hitStunLength);
            character.HitByAttack();
            StartCoroutine(Knockback(forceDirection * knockbackForce));
            // StartCoroutine(Hitstun(hitStunLength));
        }


        public void TakePercentageKnockback(Vector2 forceDirection, float knockbackMulti, float minForce, float hitstunLength) {
            rb.velocity = Vector2.zero;
            if (forceDirection.y < 0) {
                RaycastHit2D hit = Physics2D.Raycast(rb.position, forceDirection, 1.5f, groundMask);
                if (hit.collider != null) {
                    if (Math.Abs(hit.normal.y) > Math.Abs(hit.normal.x * 1.5f)) {
                        forceDirection.Set(forceDirection.x, 0);
                        forceDirection = forceDirection.normalized;
                    }
                }
            }

            
            
            Vector2 force = forceDirection * (percentage /  3) * knockbackMulti;
            if (force.magnitude < minForce)
            {
                force = forceDirection * minForce;
            }
            
            character.HitByAttack();
            playerMovement.HitByAttack(hitstunLength);
            StartCoroutine(Knockback(force));
            isInHitstun = true;
        }

        IEnumerator Knockback(Vector2 force) {

            
            rb.AddForce(force, ForceMode2D.Impulse);

            yield return new WaitForSeconds(0.1f);

            // float targetVelocity = 10;
            // while (rb.velocity.x < 10) {
            //     rb.AddForce(new Vector2(100, 20), ForceMode2D.Force);
            //     yield return new WaitForFixedUpdate();
            // }

            //slow down

            int scale = force.x > 0 ? -1 : 1;
            // float targetVelocity = force.x > 0 ? 2 : -2;
            if (scale < 0)
            {
                float targetVelocity = 2;
                while (rb.velocity.x > targetVelocity) {
                    rb.AddForce(new Vector2(dragAfterHit * scale , 0), ForceMode2D.Force);
                    yield return new WaitForFixedUpdate();
                }
            }
            else
            {
                float targetVelocity = -2;
                while (rb.velocity.x < targetVelocity) {
                    rb.AddForce(new Vector2(dragAfterHit * scale , 0), ForceMode2D.Force);
                    yield return new WaitForFixedUpdate();
                }
            }

            // float slowDownForce = 200;
            // rb.AddForce(new Vector2(horizontalForce,verticalForce), ForceMode2D.Impulse);
            // yield return new WaitForSeconds(0.1f);
            // Vector2 direction = new Vector2(-horizontalForce, 0).normalized;
            // float timer = 5f;
            // float start = rb.velocity.x;
            // float target = 0;
            // while (rb.velocity.x > 0) {
            //     float xVelocity = Mathf.Lerp(start, target, timer / 5);
            //     rb.velocity = new Vector2(xVelocity, 0);
            //     // rb.AddForce(direction * slowDownForce, ForceMode2D.Force);
            //     timer -= Time.fixedDeltaTime;
            //     yield return new WaitForFixedUpdate();
            // }
            //
            // rb.velocity = new Vector2(0, rb.velocity.y);
        } 
    }
}
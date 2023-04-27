using System.Collections;
using UnityEngine;

namespace Characters {
    public class CharacterRespawner : MonoBehaviour{
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private CharacterBase character;
        [SerializeField] private HurtboxController hurtbox;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private GameObject deathEffect;
        [SerializeField] private float respawnTime = 2f;
        
        [SerializeField] public Transform respawnPoint;

        public void Kill() {
            playerMovement.enabled = false;
            character.enabled = false;
            hurtbox.enabled = false;
            rb.isKinematic = true;
            transform.position = respawnPoint.position;
            StartCoroutine(Respawn());

        }

        IEnumerator Respawn() {
            yield return new WaitForSeconds(respawnTime);
            playerMovement.enabled = true;
            character.enabled = true;
            hurtbox.enabled = true;
            rb.isKinematic = false;
        }

    }
}
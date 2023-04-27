using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters
{
    public abstract class CharacterBase : MonoBehaviour {

        [SerializeField] protected HurtboxController hurtbox;
        [SerializeField] public Transform playerNumberTarget;
        [SerializeField] private CharacterRespawner respawner;

        private void Start() {
        }

        public void Respawn() {
            respawner.Kill();
        }

        public virtual void HitByAttack() {
            
        }


    }
}
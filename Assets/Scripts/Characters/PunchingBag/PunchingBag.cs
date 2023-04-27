namespace Characters.PunchingBag {
    public class PunchingBag : CharacterBase {
        
        private void Start() {
            hurtbox.OnDeath += Respawn;
        }
        
        public void TakeDamage(float damage) {
            throw new System.NotImplementedException();
        }
        
    }
}
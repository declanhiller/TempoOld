using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    // [SerializeField] private List<Attack> allAttacks = new List<Attack>();
    // [SerializeField] private Attack[] startAttacks;
    public CircleCollider2D[] attackHitboxes;
    
    //MOVE THESE TO A SEPERATE SCRIPT LATER!!! COMMUNICATING ABOUT INTERFACE LATER !
    public float percentage;
    private Rigidbody2D rb2d;

    public void Start()
    {
        percentage = 0f;
        rb2d = GetComponent<Rigidbody2D>();
    }
    

    private void Update()
    {
        //placeholder keydowns!!-- just to make sure code is functioning
        // INPUT KNOCKBACK AND DAMAGE ARE PLACEHOLDER. GOD FORBID WE HARDCODE THIS.
        if (Input.GetKeyDown(KeyCode.J))
        {
            LaunchAttack(attackHitboxes[0], new Vector2(0.4f,0), 100);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            LaunchAttack(attackHitboxes[1], new Vector2(0,1), 10);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LaunchAttack(attackHitboxes[2], new Vector2(0.1f, 0.1f), 30);
        }
    }
    
    private void LaunchAttack(CircleCollider2D col, Vector2 atkKnockback, float damage)
    {
        Vector2 knockbackDirection = (transform.position - col.transform.position).normalized;
        Collider2D[] cols = Physics2D.OverlapCircleAll(col.bounds.center,col.radius * col.transform.localScale.x,LayerMask.GetMask("Hitboxes"));
        foreach (Collider2D c in cols)
        {
            if (c.transform.parent.parent == transform)
            {
                continue;
            }
            Debug.Log(c.name);
            c.GetComponentInParent<AttackController>().TakeDamage(damage, atkKnockback);
        }
    }
    
    // THESE FUNCTIONS SHOULD BE PUT SOMEWHERE ELSE.
    public void TakeDamage(float owiedamage, Vector2 dknockback)
    {
        percentage += owiedamage;
        TakeKnockback(percentage, dknockback);
    }

    public void TakeKnockback(float percent, Vector2 knockback)
    {
        // 0.1f will probably be replaced by the character's resistance to knockback, e.g rigidbody mass
        rb2d.velocity = Vector2.zero;
        knockback = knockback * percent * 0.1f;
        rb2d.AddForce(knockback, ForceMode2D.Impulse);
    }

}
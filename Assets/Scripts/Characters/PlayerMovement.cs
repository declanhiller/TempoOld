using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    // necessary components
    [Header("Necessary Movement Components")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private float groundCheckXShrink = 0.85f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private PhysicsMaterial2D frictionlessMaterial;
    private Rigidbody2D rb;
    [SerializeField] private Collider2D environmentalCollider;
    private PlayerCharacterContainer characterContainer;
    private int movementDirection; // which way the player is moving
    [SerializeField] private ParticleSystem onHitParticles;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Movement Audio Components")]
    [SerializeField] private AudioSource movementAudioSource; // source for all movement
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip dodgeSound;
    [SerializeField] private AudioClip airDodgeSound;

    // acceleration and deceleration values
    [Header("Acceleration Values")]
    [SerializeField] private float groundedMoveAcceleration = 3f;
    [SerializeField] private float aerialMoveAcceleration = 2f;
    [SerializeField] private float dashMoveAcceleration = 9f; // needed to allow quick dashing back and forth
    [SerializeField] private float moveDeceleration = 0.25f;
    private float currentMoveAcceleration;
    
    // movement speeds
    [Header("Movement Speed Values")]
    [SerializeField] private float groundedMovementSpeed = 8f;
    [SerializeField] private float aerialMovementSpeed = 8f;
    [SerializeField] private float dashMovementSpeed = 24f;
    [SerializeField] private float dashMovementSpeedInAir = 24f;
    [SerializeField] private float runMovementSpeed = 12f;
    [SerializeField] private float dashTime = 0.075f;
    // time it takes to input dash jump after dash time expires: 
    [SerializeField] private float dashAirTime = 0.075f;
    private float currentMaxMovementSpeed;
    private bool isDashing; // used to retain dash timing
    private bool isDashingInAir; // used to retain dash timing in air
    private bool isRunning; // used to retain dash speed while holding down dash

    // jump height
    [Header("Jump Values")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float maxJumps = 3f; // max jumps from the air
    private float currentJumps; // used as a current jump counter
    
    // jump buffer variables
    [Header("Buffer Values")]
    [SerializeField] private float jumpBufferTime = 0.1f;
    private float jumpBufferCount;

    // coyote time variables
    [SerializeField] private float coyoteTimeTime = 0.1f;
    private float coyoteTimeCount;

    // gravity scales to modify gravity when/when not fastfalling
    [Header("Gravity Scale Values")]
    [SerializeField] private float defaultGravityScale = 3f;
    [SerializeField] private float fastfallGravityScale = 3.5f;
    private float additionalGravityScale;
    
    // max fall speeds so they are capped
    [Header("Falling Values")]
    [SerializeField] private float defaultMaxFallSpeed = 10f;
    [SerializeField] private float fastfallMaxFallSpeed = 20f;
    [SerializeField] private float ledgeTouchMaxFallSpeed = 5f;
    [SerializeField] [Range(0f, 0.71f)] private float fastfallInputThreshold = 0.675f;
    [SerializeField] private float platformFallTime = 0.1f;
    private float currentMaxFallSpeed;
    private bool isFastfalling;
    
    // dodge timing values
    [Header("Dodge Timing Values")] 
    [SerializeField] private float spotDodgeTime = 0.25f;
    [SerializeField] private float airDodgeTime = 0.4f;
    [SerializeField] private float spotDodgeCooldown = 2f;
    [SerializeField] private float airDodgeCooldown = 2.5f;
    [SerializeField] private Animator dodgeAnimator;
    private float currentDodgeTime;
    private float currentDodgeCooldown;
    private bool canDodge;

    [Space(10)]
    [Header("Animator Controller")] 
    [SerializeField] private Animator animator;

    [SerializeField] private float idleVelocityMax = 0.05f;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = defaultGravityScale;
        
        environmentalCollider = GetComponent<Collider2D>();

        movementDirection = 0;
        
        currentMaxMovementSpeed = groundedMovementSpeed;
        currentMoveAcceleration = groundedMoveAcceleration;
        isDashing = false;
        isRunning = false;

        currentMaxFallSpeed = defaultMaxFallSpeed;
        isFastfalling = false;
        additionalGravityScale = 0f; // default 0
        
        currentJumps = maxJumps;

        jumpBufferCount = 0f;
        coyoteTimeCount = coyoteTimeTime;

        canDodge = true;
    }

    private void Start() {
        characterContainer = GetComponentInParent<PlayerCharacterContainer>();
        characterContainer.OnJump += OnJump;
        characterContainer.OnDodge += OnDodgeDash;
        characterContainer.OnDodgeCancel += OnDodgeDashCancel;
    }

    private void Update()
    {
        EditValuesBasedOnGrounded();
        CheckMovementDirection();
    }

    private bool previouslyGrounded;
    private bool wasTouchingLedge;
    private void EditValuesBasedOnGrounded()
    {
        // reset jumps to max when grounded
        // if different, aerial and grounded movement speeds are toggled between here
        if (IsConsideredGrounded()) 
        {
            previouslyGrounded = true;
            environmentalCollider.sharedMaterial = null;
            animator.SetBool("InAir", false);
            currentJumps = maxJumps;
            
            if (isDashing)
            {
                currentMaxMovementSpeed = dashMovementSpeed;
                currentMoveAcceleration = dashMoveAcceleration;
            }
            else if (isRunning)
            {
                currentMaxMovementSpeed = runMovementSpeed;
                currentMoveAcceleration = groundedMoveAcceleration;
            }
            else
            {
                currentMaxMovementSpeed = groundedMovementSpeed;
                currentMoveAcceleration = groundedMoveAcceleration;
            }

            coyoteTimeCount = coyoteTimeTime;

            currentDodgeTime = spotDodgeTime;
            currentDodgeCooldown = spotDodgeCooldown;

            wasTouchingLedge = false;
        }
        else if (IsTouchingLedge() && !wasTouchingLedge)
        {
            currentJumps = maxJumps;
            wasTouchingLedge = true;
        }
        else
        {
            animator.SetBool("InAir", true);
            environmentalCollider.sharedMaterial = frictionlessMaterial;
            if (previouslyGrounded) {
                animator.SetTrigger("StartFall");
            }
            
            previouslyGrounded = false;
            
            if (isDashingInAir)
            {
                currentMaxMovementSpeed = dashMovementSpeedInAir;
            }
            else
            {
                currentMaxMovementSpeed = aerialMovementSpeed;
            }
            
            currentMoveAcceleration = aerialMoveAcceleration;
            
            // start the coroutine only if the ticking has not begun yet
            if (Mathf.Abs(coyoteTimeCount - coyoteTimeTime) < Mathf.Epsilon)
            {
                StartCoroutine(CoyoteTimer());
            }
            
            currentDodgeTime = airDodgeTime;
            currentDodgeCooldown = airDodgeCooldown;
        }
    }
    
    private void CheckMovementDirection()
    {
        // checks the sign of movement so it can be relayed to Move()
        //     this is used to damp motion back to 0 when no side input is being pressed
        if (rb.velocity.x > 0) {
            if (!inAttack) {
                transform.localScale = new Vector3(1, 1, 1);
            }
            movementDirection = 1;
        }
        else if (rb.velocity.x < 0)
        {
            if (!inAttack) {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            movementDirection = -1;
        }
        else
        {
            movementDirection = 0;
        }
    }

    private void FixedUpdate()
    {
        Move();
        Fall();
        ModifyGravity();
        CapFallSpeed();
    }

    // new move method -- using math and velocity!
    private void Move() {
        if (inHitstun) return;
        // if (stopPlayerMovement) return;
        float newXVelocity = rb.velocity.x;
        Vector3 movementInput = characterContainer.movementInputVector;
        if (inAttack && IsConsideredGrounded()) {
            movementInput = Vector3.zero;
        }
        if (movementInput.x != 0)
        {
            // the new x velocity will be the velocity after an acceleration value is added
            // no need for time.deltaTime bc this occurs in FixedUpdate()
            newXVelocity += movementInput.x * currentMoveAcceleration;
            newXVelocity = Mathf.Clamp(newXVelocity, -currentMaxMovementSpeed, 
                currentMaxMovementSpeed);
        }
        else if (movementInput.x == 0 && movementDirection != 0) // if no input but movement still exists
        {
            newXVelocity -= movementDirection * moveDeceleration;

            if (movementDirection > 0)
            {
                newXVelocity = Mathf.Clamp(newXVelocity, 0f, currentMaxMovementSpeed);
            }
            else if (movementDirection < 0)
            {
                newXVelocity = Mathf.Clamp(newXVelocity, -currentMaxMovementSpeed, 0f);
            }
        }

        if (!isRunning) {
            if (Math.Abs(rb.velocity.x) < idleVelocityMax) { 
                animator.SetInteger("Speed", 0);
            } else { 
                animator.SetInteger("Speed", 1);
            }
        } else {
            if (Math.Abs(rb.velocity.x) < idleVelocityMax) {
                animator.SetInteger("Speed", 0);
            } else {
                animator.SetInteger("Speed", 2);
            }
        }


        rb.velocity = new Vector2(newXVelocity, rb.velocity.y);
    }
    
    public void OnJump()
    {
        // if the character is not fastfalling
        // then the jump will be triggered
        if (!isFastfalling)
        {
            if (currentJumps > 0) // jumps normally if current jumps are >= 0
            {
                Jump();
            }
            else
            {
                jumpBufferCount = jumpBufferTime; // sets the initial count value to start ticking down
                StartCoroutine(JumpWithBuffer()); // starts the coroutine to tick jBC down
            }
        }
    }

    IEnumerator JumpWithBuffer()
    {
        while (jumpBufferCount > 0)
        {
            // if player hits the ground within the jumpBufferTime time, then the if will execute
            //     this value is being counted down from with jumpBufferCount
            if (IsConsideredGrounded())
            {
                Jump();
            }
            jumpBufferCount -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator CoyoteTimer()
    {
        while (coyoteTimeCount > 0)
        {
            coyoteTimeCount -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private void Jump() {
        if (inHitstun) return;
        if (inAttack) return;
        // this is needed to exit the coroutine loop
        // there should not be any more wait time after the jump has happened
        jumpBufferCount = 0f;

        // as soon as the jump happens, the coyote timer has to hit 0 to stop the loop
        if (coyoteTimeCount > 0)
        {
            coyoteTimeCount = 0f;
        }
        else // if the coyote counter has gone to 0 or below, that is when jumps can be subtracted
        {
            currentJumps--;
        }
        
        movementAudioSource.PlayOneShot(jumpSound);
        
        // equation below makes the gravity scale tunable without affecting jump height
        //     NOTE: the actual height will always be a little less than the specified height
        //     this effect is amplified when the gravity scale is higher
        // gravity scale (ie, downward acceleration) and jump height can now be treated separately
        float jumpVelocity = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * rb.gravityScale));
        animator.SetTrigger("Jump");
        animator.SetBool("IsInJumpAnimation", true);
        rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
    }

    public void JumpAnimationEnded() {
        animator.SetBool("IsInJumpAnimation", false);
        animator.SetTrigger("StartFall");
    }

    private void Fall()
    {
        // this serves the same purpose as performed
        if (characterContainer.movementInputVector.y < -fastfallInputThreshold && !inAttack) // if the fastfall is activated past a threshold
        {
            if (!IsConsideredGrounded()) // fastfall only if aerial
            {
                if (inHitstun) return;
                currentMaxFallSpeed = fastfallMaxFallSpeed;

                // this is how much MORE gravity will be used
                // then, this value will be multiplied by gravity (in ModifyGravity) to get the additional velocity needed
                additionalGravityScale = fastfallGravityScale - defaultGravityScale;
            
                isFastfalling = true;
            }
            else if (IsTouchingPlatform()) // platform fall if touching a platform
            {
                if (inAttack) return;
                if (inHitstun) return;
                StartCoroutine(PlatformFall());
            }
        } 
        else // this serves the same purpose as canceled
        {
            if (IsTouchingLedge())
            {
                currentMaxFallSpeed = ledgeTouchMaxFallSpeed;
            }
            else
            {
                currentMaxFallSpeed = defaultMaxFallSpeed;
            }

            // the additional acceleration will reset to 0 whenever fastfall is not being pressed
            additionalGravityScale = 0f;
            
            isFastfalling = false;
        }
    }

    IEnumerator PlatformFall()
    {
        Collider2D platformCollider = GetPlatformCollider();
        Physics2D.IgnoreCollision(environmentalCollider, platformCollider, true);
        yield return new WaitForSeconds(platformFallTime);
        Physics2D.IgnoreCollision(environmentalCollider, platformCollider, false);
    }
    
    private void CapFallSpeed()
    {
        float newYVelocity = rb.velocity.y;
        newYVelocity = Mathf.Clamp(newYVelocity, -currentMaxFallSpeed, float.MaxValue);
        rb.velocity = new Vector2(rb.velocity.x, newYVelocity);
    }

    private void ModifyGravity()
    {
        float newYVelocity = rb.velocity.y;
        
        // get the additional gravity acceleration
        float additionalAcceleration = Physics2D.gravity.y * additionalGravityScale;
        newYVelocity += additionalAcceleration;
        
        rb.velocity = new Vector2(rb.velocity.x, newYVelocity);
    }

    public void OnDodgeDash()
    {
        // ground -- L or R, dash; neutral, spot dodge
        // air -- 8 directions, dodge; neutral, spot dodge

        if (inAttack) return;
        if (inHitstun) return;
        if (IsConsideredGrounded() && characterContainer.movementInputVector.x != 0) // dash stuff
        {
            StartCoroutine(Dash());
            animator.SetTrigger("Dash");
            dodgeAnimator.gameObject.SetActive(true);
            dodgeAnimator.SetTrigger("Dash");
            animator.SetInteger("Speed", 2);

            isRunning = true; // if the button is held (emulated behavior), the player will run
        }
        else if (canDodge) // neutral dodge
        {
            StartCoroutine(Dodge());
        }
    }

    private bool inAttack;
    public void StartAttack() {
        inAttack = true;
    }

    public void EndAttack() {
        inAttack = false;
    }
    
    public void OnDodgeDashCancel() {
        animator.SetInteger("Speed", 0);
        isRunning = false;
    }

    IEnumerator Dash()
    {
        movementAudioSource.PlayOneShot(dashSound);
        isDashing = true;
        isDashingInAir = true;
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        yield return new WaitForSeconds(dashAirTime);
        isDashingInAir = false;
    }

    private float flashTime = 0.05f;
    IEnumerator Dodge()
    {
        animator.SetTrigger("Dodge");

        if (IsConsideredGrounded())
        {
            SetAndPlayMovementSound(dodgeSound);
        }
        else
        {
            SetAndPlayMovementSound(airDodgeSound);
        }
        
        canDodge = false;
        float timer = 0;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color startColor = Color.white;
        Color endColor = new Color(0.5f, 0.78f, 0.96f);
        // Color endColor = new Color(1, 1, 1, 0.5f);
        float flashTimer = 0;
        while (timer < currentDodgeTime) {
            if (flashTimer > flashTime) {
                flashTimer = 0;
                (startColor, endColor) = (endColor, startColor);
            }

            Color color = Color.Lerp(startColor, endColor, flashTimer / flashTime);
            spriteRenderer.color = color;
            flashTimer += Time.deltaTime;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(currentDodgeCooldown);
        canDodge = true;
    }
    
    public bool IsConsideredGrounded()
    {
        // return type is collider2d, but converts to true if anything is found
        bool isGrounded = IsTouchingGround() || IsTouchingPlatform();
        return isGrounded;
    }

    private bool IsTouchingGround()
    {
        bool isTrueGrounded = Physics2D.OverlapBox(groundCheck.position,
            new Vector2(environmentalCollider.bounds.size.x * groundCheckXShrink, 
                environmentalCollider.bounds.size.y / 1.5f), 0f, groundLayer);
        return isTrueGrounded;
    }

    private bool IsTouchingPlatform()
    {
        bool isTouchingPlatform = Physics2D.OverlapBox(groundCheck.position,
            new Vector2(environmentalCollider.bounds.size.x * groundCheckXShrink, 
                environmentalCollider.bounds.size.y / 1.5f), 0f, platformLayer);
        return isTouchingPlatform;
    }

    private bool IsTouchingLedge()
    {
        bool isGrabbingLedge = Physics2D.OverlapBox(ledgeCheck.position,
            new Vector2(environmentalCollider.bounds.size.x,
                environmentalCollider.bounds.size.y * 0.85f), 0f, groundLayer);
        return isGrabbingLedge;
    }

    private Collider2D GetGroundCollider()
    {
        Collider2D groundCollider = Physics2D.OverlapBox(groundCheck.position,
            new Vector2(environmentalCollider.bounds.size.x * groundCheckXShrink, 
                environmentalCollider.bounds.size.y / 1.5f), 0f, groundLayer);
        return groundCollider;
    }
    
    private Collider2D GetPlatformCollider()
    {
        Collider2D platformCollider = Physics2D.OverlapBox(groundCheck.position,
            new Vector2(environmentalCollider.bounds.size.x * groundCheckXShrink, 
                environmentalCollider.bounds.size.y / 1.5f), 0f, platformLayer);
        return platformCollider;
    }

    private Collider2D GetLedgeCollider()
    {
        Collider2D ledgeCollider = Physics2D.OverlapBox(ledgeCheck.position,
            new Vector2(environmentalCollider.bounds.size.x,
                environmentalCollider.bounds.size.y * 0.85f), 0f, groundLayer);
        return ledgeCollider;
    }

    private bool inHitstun;
    private bool stopPlayerMovement;
    public void HitByAttack(float hitstun) {
        StartCoroutine(CheckForJoystickModification());
        StartCoroutine(HitstunTimer(hitstun));
        stopPlayerMovement = true;
    }

    IEnumerator HitstunTimer(float timer) {
        inHitstun = true;
        onHitParticles.Play();
        spriteRenderer.color = Color.gray;
        animator.SetBool("InHitstun", true);
        yield return new WaitForSeconds(timer);
        animator.SetBool("InHitstun", false);
        inHitstun = false;
        onHitParticles.Stop();
        spriteRenderer.color = Color.white;
        
    }

    IEnumerator CheckForJoystickModification() {
        while (true) {
            if (Math.Abs(characterContainer.movementInputVector.x) > 0.05f || Math.Abs(characterContainer.movementInputVector.y) > 0.05f) {
                stopPlayerMovement = false;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Enable again");
    }

    private void SetAndPlayMovementSound(AudioClip clip)
    {
        movementAudioSource.clip = clip;
        movementAudioSource.Play();
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, 
            new Vector2(environmentalCollider.bounds.size.x * groundCheckXShrink, 
                environmentalCollider.bounds.size.y / 1.5f));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(ledgeCheck.position, new Vector2(environmentalCollider.bounds.size.x, 
            environmentalCollider.bounds.size.y * 0.85f));
    }
}

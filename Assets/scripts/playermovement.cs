using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;


public class playermovement : MonoBehaviour
{
    public LayerMask groundLayer;
    private enum AnimationState{idle, run, jump, fall, doublejump}
    private BoxCollider2D box;
    private Rigidbody2D rb;
    private int jumpCount = 0;
    private Animator Animator;
    private AnimationState currentState = AnimationState.idle;
    private SpriteRenderer SpriteRenderer;
    private bool jumpButtonDown = false;
    private PlatformEffector2D currentEffector;
    public bool canMove = false;
    [Header("Dash")]
        [SerializeField] private float dashSpeed = 20f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashCooldown = 1f;//make dash cooldown not come up if still in the air
        private bool isDashing = false;
        private bool canDash = true;
    [Header("Movement")]
        [SerializeField]private float maxRunSpeed = 10f;
        [SerializeField] private float jumpHeight = 10f; 
    [Header("Bullet")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float bulletSpeed;
    [Header("shooting")]
        [SerializeField] private float fireRate = 0.25f;       // Cooldown between shots
        [SerializeField] private float inputBufferTime = 0.05f; // Window for diagonal input
        private float nextFireTime = 0f;
        private bool isCheckingInput = false;
    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        box = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        StartCoroutine(EnableMovementDelay());
    }

    void Update()
    {
        if (!canMove) return;
        if (Input.GetKeyDown(KeyCode.Space) && canDash){
            StartCoroutine(Dash());
            if(!OnGround()){
                canDash = false;
            }
        }
        if (!OnGround() && jumpCount == 0){
            jumpCount = 1;
        }
        // SKIP all other movement if we are currently dashing
        if (isDashing){
            return; 
        }
        if (OnGround() && rb.velocity.y <= 0){
            jumpCount = 0;
            canDash = true;
        }
        float xInput = 0;

        if (Input.GetKey(KeyCode.RightArrow)) {
            xInput = 1;
        } else if (Input.GetKey(KeyCode.LeftArrow)) {
            xInput = -1;
        } 
        // Horizontal Movement
        if (xInput != 0) {
            rb.velocity = new Vector2(xInput * maxRunSpeed, rb.velocity.y);
            SpriteRenderer.flipX = xInput < 0;
        } else {
            // Apply friction/stopping logic
            rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, 0, 1.5f), rb.velocity.y);
        }
        // Flip sprite
        if (xInput != 0) SpriteRenderer.flipX = xInput < 0;
        //jumping      
        jumpButtonDown = Input.GetKeyDown(KeyCode.UpArrow);
        if (jumpButtonDown && jumpCount < 2){
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            jumpCount++;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)){
            rb.velocity = new Vector2(rb.velocity.x, -10);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
        // Find the effector if we are standing on a platform
        // We use a Raycast to see if there's a platform below us
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2f, groundLayer);
            
            if (hit.collider != null)
            {
                currentEffector = hit.collider.GetComponent<PlatformEffector2D>();
                if (currentEffector != null)
                {
                    StartCoroutine(FallThrough());
                }
            }
        }
        

        //SHOOTING
        if (Time.time >= nextFireTime){
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)){
                if (!isCheckingInput) 
                {
                    StartCoroutine(ProcessShootDirection());
                }
            }
        }
        if (!MenuScript.isPaused){
            UpdateAnimation();
        }
        
    }

    private bool OnGround(){
        bool onGround = Physics2D.BoxCast(transform.position, box.size, 0f,Vector2.down,.1f, groundLayer);
        return onGround;
    }

    private void UpdateAnimation(){
        if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))&& OnGround()){
            currentState = AnimationState.run;}
        else if (!OnGround() && Input.GetKey(KeyCode.DownArrow)){
            currentState = AnimationState.fall;}
        else if (!OnGround() && jumpButtonDown && jumpCount < 3){
            currentState = AnimationState.doublejump;}
        else if (!OnGround() && (currentState==AnimationState.doublejump) && (rb.velocity.y>0)){
            currentState = AnimationState.doublejump;}
        else if (!OnGround() ){
            currentState = AnimationState.jump;}
        else{
            currentState = AnimationState.idle;}

        if (currentState == AnimationState.run){
            Animator.CrossFade("run",0, 0);}
        else if (currentState == AnimationState.doublejump){
            Animator.CrossFade("doublejump",0, 0);}
        else if (currentState == AnimationState.fall){
            Animator.CrossFade("fall",0, 0);}
        else if (currentState == AnimationState.jump){
            Animator.CrossFade("jump",0, 0);}
        else if (currentState == AnimationState.idle){
            Animator.CrossFade("idle",0, 0);}
    }
    private IEnumerator Dash(){
        canDash = false;
        isDashing = true;
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 dashDirection = new Vector2(x, y).normalized;
        rb.velocity = dashDirection * dashSpeed;
        SpriteRenderer.flipX = !SpriteRenderer.flipX;
        // Freeze gravity so the dash doesn't sag downwards
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        Animator.CrossFade("dash", 0, 0);
        Color dashCyan = new Color32(20, 209, 255, 255);
        SpriteRenderer.color = Color.Lerp(Color.white, dashCyan, 0.3f);
        yield return new WaitForSeconds(dashDuration);
        SpriteRenderer.color = Color.white;
        SpriteRenderer.flipX = !SpriteRenderer.flipX;
        rb.velocity = rb.velocity * 0.2f; 

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
    }
    public void Shoot(Vector3 direction){
        GameObject newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);//comeback here
        newBullet.GetComponent<Rigidbody2D>().velocity = direction.normalized*bulletSpeed;
        Destroy(newBullet, 0.5f);
    }
    private IEnumerator ProcessShootDirection() 
    {
        isCheckingInput = true;
        float timer = 0;

        // Wait for a few frames to see if another key is pressed
        while (timer < inputBufferTime) 
        {
            timer += Time.deltaTime;
            yield return null; 
        }

        // Now calculate the final direction based on what keys are CURRENTLY held
        float x = 0;
        float y = 0;

        if (Input.GetKey(KeyCode.W)) y = 1;
        else if (Input.GetKey(KeyCode.S)) y = -1;

        if (Input.GetKey(KeyCode.D)) x = 1;
        else if (Input.GetKey(KeyCode.A)) x = -1;

        Vector3 finalDir = new Vector3(x, y, 0).normalized;

        // Only shoot if we actually have a direction (prevents shooting if keys were released)
        if (finalDir != Vector3.zero) 
        {
            Shoot(finalDir);
            nextFireTime = Time.time + fireRate; // Set the cooldown
        }

        isCheckingInput = false;
    }
    private IEnumerator FallThrough()
    {
        // Find the specific platform collider below us
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
        
        if (hit.collider != null)
        {
            Collider2D platformCollider = hit.collider;
            Collider2D playerCollider = GetComponent<Collider2D>();

            // Turn off collisions between JUST the player and THIS platform
            Physics2D.IgnoreCollision(playerCollider, platformCollider, true);

            yield return new WaitForSeconds(0.4f); // Wait for player to clear the platform

            // Turn collisions back on
            if (platformCollider != null) // Check in case platform was destroyed
            {
                Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
            }
        }
    }
    public void DisableMovement() 
    {
        canMove = false;
        rb.velocity = Vector2.zero;
    }
    public IEnumerator EnableMovementDelay(float time = 0.1f) 
    {
        yield return new WaitForSeconds(time); 
        canMove = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 1. Check if we are colliding with a Platform
        if (collision.gameObject.CompareTag("platform"))
        {
            // 2. Check if the player is actually standing on top (grounded)
            if (OnGround())
            {
                // Set the platform as the parent so the player moves with it
                transform.parent = collision.transform;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("platform"))
        {
            // 3. When we leave the platform or jump, remove the parent
            transform.parent = null;
        }
    }
    
}
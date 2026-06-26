using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;


public class playermovement : MonoBehaviour
{
    public LayerMask groundLayer;
    private enum AnimationState{idle, run, jump, fall}
    private BoxCollider2D box;
    private Rigidbody2D rb;
    private Animator Animator;
    private AnimationState currentState = AnimationState.idle;
    private SpriteRenderer SpriteRenderer;
    private bool jumpButtonDown = false;
    private PlatformEffector2D currentEffector;
    public bool canMove = false;
    [Header("Movement")]
        [SerializeField]private float maxRunSpeed = 10f;
        [SerializeField] private float jumpHeight = 10f; 


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
        if (jumpButtonDown && OnGround()){
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
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
        else if (!OnGround() && rb.velocity.y > 0){
            currentState = AnimationState.jump;}
        else if (!OnGround()){
            currentState = AnimationState.fall;}
        else{
            currentState = AnimationState.idle;}

        if (currentState == AnimationState.run){
            Animator.CrossFade("run",0, 0);}
        else if (currentState == AnimationState.fall){
            Animator.CrossFade("fall",0, 0);}
        else if (currentState == AnimationState.jump){
            Animator.CrossFade("jump",0, 0);}
        else if (currentState == AnimationState.idle){
            Animator.CrossFade("idle",0, 0);}
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
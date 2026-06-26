using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_collision : MonoBehaviour
{
    private Vector2 checkpoint;
    public MenuScript menuScript;
    private void OnEnable() => countdownTimer.OnTimerEnd += Die;
    private void OnDisable() => countdownTimer.OnTimerEnd -= Die;
    void Start()
    {
        checkpoint = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other){
    if (other.gameObject.CompareTag("enemy")){
        StartCoroutine(PlayerDeathSequence());
    }
        else if (other.gameObject.CompareTag("checkpoint"))
        {
            checkpoint = other.transform.position;
            Debug.Log("Checkpoint Saved!");
        }
    }

    public IEnumerator PlayerDeathSequence()
    {
        GameData.Deaths += 1;
        playermovement movement = GetComponent<playermovement>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Animator anim = GetComponent<Animator>();
        if (movement != null) movement.DisableMovement();
        rb.bodyType = RigidbodyType2D.Static;
        if (anim != null) anim.Play("death");
        yield return new WaitForSeconds(0.6f); 
        anim.Play("idle");
        transform.position = checkpoint;
        rb.bodyType = RigidbodyType2D.Dynamic;
        if (movement != null)
        {
            StartCoroutine(movement.EnableMovementDelay());
        }
    }

    private IEnumerator HitStop(float duration)
    {
        Time.timeScale = 0f; 
        yield return new WaitForSecondsRealtime(duration); 
        Time.timeScale = 1f; 
    }
    public void Die() {
    // Play animation, show game over screen, etc.
        Debug.Log("Player has died due to timer expiration.");
        
        StartCoroutine(PlayerDeathSequence());
        menuScript.deathScreen();
    }
    
}
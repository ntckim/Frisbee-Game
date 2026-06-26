using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class exit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        playermovement potentialPlayer = collision.GetComponent<playermovement>();
        if (potentialPlayer != null){
            int current = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(1);
            SceneManager.LoadScene(current+1);
        }
    }
}

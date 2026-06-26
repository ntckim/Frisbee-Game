using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sawmovement : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform pointA;
    public Transform pointB;
    public Transform pointC;
    public Transform pointD;
    public float speed = 6f;
    private Vector3 nextPosition;
    void Start()
    {
        nextPosition = pointB.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);
        if (transform.position == nextPosition){
            if (nextPosition == pointA.position){
                nextPosition = pointB.position;
            }
            else if (nextPosition == pointB.position){
                nextPosition = pointC.position;
            }
            else if (nextPosition == pointC.position){
                nextPosition = pointD.position;
            }
            else if (nextPosition == pointD.position){
                nextPosition = pointA.position;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.CompareTag("Player")){
            collision.gameObject.transform.parent = transform;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingPlatform : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
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
            nextPosition = (nextPosition == pointA.position)?pointB.position : pointA.position;
        }    
    }
    
}

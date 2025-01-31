using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 targetDestination;
    bool isMoving = false;
    float speed = 3;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((targetDestination - transform.position).normalized), 0.01f);
            transform.position += transform.forward * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, targetDestination) < 0.1f)
            {
                isMoving = false;
                transform.position = targetDestination;

            }
        }
    }


    internal void setDestination(Vector3 destination)
    {
        targetDestination = new Vector3( destination.x , transform.position.y , destination.z);
        isMoving = true;
    }
}

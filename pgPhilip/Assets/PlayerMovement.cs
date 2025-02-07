using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 targetDestination;
    bool isMoving = false;
    float speed = 6f;
    float rotationSpeed = 15f;
    Vector3 movementDirection;

    void Update()
    {
        if (isMoving)
        {
            if (movementDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            transform.position += movementDirection * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, targetDestination) < 0.01f)
            {
                isMoving = false;
                transform.position = targetDestination;
            }
        }
    }

    internal void setDestination(Vector3 destination)
    {
        if (Vector3.Distance(transform.position, destination) < 0.1f)
        {
            return;
        }

        targetDestination = new Vector3(destination.x, transform.position.y, destination.z);
        movementDirection = (targetDestination - transform.position).normalized;
        isMoving = true;
    }

    public void SetRotationSpeed(float newRotationSpeed)
    {
        rotationSpeed = newRotationSpeed;
    }
}

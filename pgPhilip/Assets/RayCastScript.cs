using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastScript : MonoBehaviour
{
    PlayerMovement thePlayer;
    float raycastCooldown = 0.1f;
    float nextRaycastTime = 0f;

    void Start()
    {
        thePlayer = FindObjectOfType<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetMouseButton(1) && Time.time >= nextRaycastTime)
        {
            nextRaycastTime = Time.time + raycastCooldown;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log("Raycast hit: " + hit.collider.name);
                thePlayer.setDestination(hit.point);
            }
        }
    }
}

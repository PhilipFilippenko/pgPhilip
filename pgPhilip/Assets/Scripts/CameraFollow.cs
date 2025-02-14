using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    PlayerController player;
    float distance = 15f;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        transform.position = player.transform.position - transform.forward * distance;
    }
}
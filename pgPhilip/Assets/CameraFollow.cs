using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    PlayerMovement thePlayer;
    float dist = 10;
    // Start is called before the first frame update
    void Start()
    {
        thePlayer = FindObjectOfType<PlayerMovement>();
    }


    void Update()
    {
        transform.position = thePlayer.transform.position - dist *transform.forward;
    }
}
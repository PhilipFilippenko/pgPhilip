using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    PlayerMovement player;
    float distance = 20f;
    float scrollSpeed = 20f;
    Vector3 lastMousePos;
    Vector3 lastDragPos;

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.F1))
            transform.position = player.transform.position - transform.forward * distance;
        else
        {
            EdgeScrolling();
            RaycastDrag();
        }
    }

    void EdgeScrolling()
    {
        Vector3 move = Vector3.zero;
        Vector3 mousePos = Input.mousePosition;

        if (mousePos.x < 10) move.x = -1;
        if (mousePos.x > Screen.width - 10) move.x = 1;
        if (mousePos.y < 10) move.z = -1;
        if (mousePos.y > Screen.height - 10) move.z = 1;

        transform.position += move * scrollSpeed * Time.deltaTime;
    }

    void RaycastDrag()
    {
        if (Input.GetMouseButtonDown(2))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
                lastMousePos = hit.point;
        }

        if (Input.GetMouseButton(2))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && Vector3.Distance(hit.point, lastMousePos) > 0.01f)
            {
                transform.position -= hit.point - lastMousePos;
                lastMousePos = hit.point;
            }
        }
    }
}

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public PlayerController player;
    public float distance = 15f;
    public float smoothing = 0.01f;
    public float cursorStrength = 3f;
    public float fixedY = 12f;
    public float minCursorDistance = 3f;

    private Vector3 offset;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        offset = transform.position - player.transform.position;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 cursorWorldPosition = ray.GetPoint(rayDistance);
            Vector3 directionToCursor = cursorWorldPosition - player.transform.position;

            if (directionToCursor.magnitude < minCursorDistance)
            {
                cursorWorldPosition = player.transform.position + directionToCursor.normalized * minCursorDistance;
            }

            Vector3 targetPosition = player.transform.position + offset;
            targetPosition += directionToCursor.normalized * cursorStrength;
            targetPosition.y = fixedY;

            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
        }
    }
}

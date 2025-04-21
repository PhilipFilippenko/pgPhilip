using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public PlayerController player;

    internal float distance = 15f;
    internal float smoothing = 0.01f;
    internal float cursorStrength = 3f;
    internal float fixedY = 12f;

    private Vector3 offset;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        offset = transform.position - player.transform.position;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, player.transform.position);

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 cursorWorldPosition = ray.GetPoint(rayDistance);
            Vector3 directionToCursor = cursorWorldPosition - player.transform.position;

            float cursorInfluence = Mathf.Clamp(directionToCursor.magnitude / 10f, 0f, 1f);

            Vector3 targetPosition = player.transform.position + offset + directionToCursor.normalized * cursorStrength * cursorInfluence;

            targetPosition.y = fixedY;

            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
        }
    }
}
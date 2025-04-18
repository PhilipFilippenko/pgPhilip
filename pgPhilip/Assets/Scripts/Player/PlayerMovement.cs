using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController player;
    private CharacterController controller;
    private Animator animator;

    void Awake()
    {
        player = GetComponent<PlayerController>();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    public void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        player.movementDirection = new Vector3(moveX, 0, moveZ).normalized;

        if (player.movementDirection.magnitude > 0)
        {
            controller.Move(player.movementDirection * player.speed * Time.deltaTime);
            var pos = player.transform.position;
            pos.y = 1;
            player.transform.position = pos;
        }

        Vector3 localDir = player.transform.InverseTransformDirection(player.movementDirection);
        animator.SetFloat("MoveZ", localDir.z);
        animator.SetFloat("MoveX", localDir.x);
    }

    public void RotateTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, player.transform.position);
        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Vector3 direction = (point - player.transform.position).normalized;
            direction.y = 0;

            if (direction.sqrMagnitude > 0.01f)
            {
                player.transform.rotation = Quaternion.Slerp(
                    player.transform.rotation,
                    Quaternion.LookRotation(direction),
                    player.rotationSpeed * Time.deltaTime
                );
            }
        }
    }
}
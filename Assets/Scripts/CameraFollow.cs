using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 120f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 60f;

    [Header("Offsets")]
    [SerializeField] private float height = 1.5f;
    [SerializeField] private float distance = 4f;
    [SerializeField] private float sideOffset = 0.5f;

    [Header("Overlap Collision")]
    [SerializeField] private float overlapRadius = 0.35f;
    [SerializeField] private LayerMask collisionMask;

    [Header("Smoothing")]
    [SerializeField] private float returnSpeed = 6f;

    // Used to know if camera was colliding in previous frame
    private bool wasOverlapping;
    private Vector3 collisionPosition;

    private InputSystem_Actions input;

    private float yaw;
    private float pitch;

    private Vector3 currentPosition;

    private void Start()
    {
        input = new InputSystem_Actions();
        input.Enable();

        // Store initial rotation values
        Vector3 initialEuler = transform.rotation.eulerAngles;
        yaw = initialEuler.y;
        pitch = initialEuler.x;

        // Lock cursor for third person camera control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        // First rotate camera based on mouse input & update camera position relative to target
        HandleMouseLook();
        FollowTarget();
    }

    private void HandleMouseLook()
    {
        Vector2 lookInput = input.Player.Look.ReadValue<Vector2>();

        // Horizontal rotation
        yaw += lookInput.x * mouseSensitivity * Time.deltaTime;

        // Vertical rotation with clamp
        pitch -= lookInput.y * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void FollowTarget()
    {
        // Position we want the camera to look at (slighlty above the player)
        Vector3 targetPosition = target.position + Vector3.up * height;

        // Ideal camera position (behind and slighlty to the side)
        Vector3 desiredPosition =
            targetPosition
            - transform.forward * distance
            + transform.right * sideOffset;

        // Check if camera would overlap with environment
        bool overlapping = Physics.OverlapSphere(
            desiredPosition,
            overlapRadius,
            collisionMask
        ).Length > 0;

        if (overlapping)
        {
            // If colliding move camera closer instantly
            collisionPosition = targetPosition - transform.forward * (distance * 0.5f);
            currentPosition = collisionPosition;
            wasOverlapping = true;
        }
        else
        {
            // If we were colliding before return to position
            if (wasOverlapping)
            {
                currentPosition = Vector3.Lerp(
                    currentPosition,
                    desiredPosition,
                    Time.deltaTime * returnSpeed
                );

                // Stop lerping once close enough
                if ((currentPosition - desiredPosition).sqrMagnitude < 0.001f)
                {
                    currentPosition = desiredPosition;
                    wasOverlapping = false;
                }
            }
            else
            {
                // Normal case: just stay at desired position :)
                currentPosition = desiredPosition;
            }
        }

        // Apply final position and make camera look at player
        transform.position = currentPosition;
        transform.LookAt(targetPosition);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize overlap sphere in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, overlapRadius);
    }
}
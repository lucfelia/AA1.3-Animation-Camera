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

        Vector3 initialEuler = transform.rotation.eulerAngles;
        yaw = initialEuler.y;
        pitch = initialEuler.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        HandleMouseLook();
        FollowTarget();
    }

    private void HandleMouseLook()
    {
        Vector2 lookInput = input.Player.Look.ReadValue<Vector2>();

        yaw += lookInput.x * mouseSensitivity * Time.deltaTime;
        pitch -= lookInput.y * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = target.position + Vector3.up * height;

        Vector3 desiredPosition =
            targetPosition
            - transform.forward * distance
            + transform.right * sideOffset;

        bool overlapping = Physics.OverlapSphere(
            desiredPosition,
            overlapRadius,
            collisionMask
        ).Length > 0;

        if (overlapping)
        {
            collisionPosition = targetPosition - transform.forward * (distance * 0.5f);
            currentPosition = collisionPosition;
            wasOverlapping = true;
        }
        else
        {
            if (wasOverlapping)
            {
                currentPosition = Vector3.Lerp(
                    currentPosition,
                    desiredPosition,
                    Time.deltaTime * returnSpeed
                );

                if ((currentPosition - desiredPosition).sqrMagnitude < 0.001f)
                {
                    currentPosition = desiredPosition;
                    wasOverlapping = false;
                }
            }
            else
            {
                currentPosition = desiredPosition;
            }
        }

        transform.position = currentPosition;
        transform.LookAt(targetPosition);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, overlapRadius);
    }
}

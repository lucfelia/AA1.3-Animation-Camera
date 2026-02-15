using UnityEngine;

public class CameraAimApplier : MonoBehaviour
{
    [SerializeField] private RaycastLookAt raycastLookAt;
    [SerializeField] private Transform lookAtTarget;

    private GameObject aimPointInstance;

    private void LateUpdate()
    {
        if (raycastLookAt == null)
            return;

        // Get the world position calculated by RaycastLookAt (this represents the real point the camera is aiming)
        Vector3 targetPosition = raycastLookAt.lookingAt;

        // Move the IK target so the character looks at the correct position
        if (lookAtTarget != null)
        {
            lookAtTarget.position = targetPosition;
        }

        // Visual debug to see the aim point in the scene
        if (aimPointInstance != null)
        {
            aimPointInstance.transform.position = targetPosition;
        }
    }
}
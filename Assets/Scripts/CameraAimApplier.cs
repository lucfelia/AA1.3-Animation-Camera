using UnityEngine;

public class CameraAimApplier : MonoBehaviour
{
    [SerializeField] private RaycastLookAt raycastLookAt;
    [SerializeField] private Transform lookAtTarget;
    [SerializeField] private GameObject aimPointPrefab;

    private GameObject aimPointInstance;

    private void Start()
    {
        if (aimPointPrefab != null)
        {
            aimPointInstance = Instantiate(aimPointPrefab);
        }
    }

    private void LateUpdate()
    {
        if (raycastLookAt == null)
        {
            return;
        }

        Vector3 targetPosition = raycastLookAt.lookingAt;

        if (lookAtTarget != null)
        {
            lookAtTarget.position = targetPosition;
        }

        if (aimPointInstance != null)
        {
            aimPointInstance.transform.position = targetPosition;
        }
    }
}

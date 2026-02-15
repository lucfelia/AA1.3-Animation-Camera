using UnityEngine;

public class RealAimController : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private Transform muzzle;

    [Header("Raycast")]
    [SerializeField] private float maxDistance = 1000f;
    [SerializeField] private LayerMask hitMask;

    [Header("UI")]
    [SerializeField] private RectTransform realAim;
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasGroup canvasGroup;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        // Safety check to prevent null reference errors (learned the hard way...)
        if (muzzle == null || realAim == null || mainCamera == null || canvasGroup == null)
            return;

        // Get the real world position where the weapon is aiming
        Vector3 targetWorldPosition = GetRealAimPoint();

        // Convert world position to screen space (needed for UI)
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(targetWorldPosition);

        // If the point is behind the camera, hide the UI indicator
        if (screenPosition.z < 0f)
        {
            canvasGroup.alpha = 0f;
            return;
        }

        canvasGroup.alpha = 1f;

        // Convert screen space into canvas local space so the UI element matches the real hit position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out Vector2 localPoint
        );

        realAim.anchoredPosition = localPoint;
    }

    private Vector3 GetRealAimPoint()
    {
        // Raycast from the muzzle forward to get the real impact position
        RaycastHit hit;

        if (Physics.Raycast(
            muzzle.position,
            muzzle.forward,
            out hit,
            maxDistance,
            hitMask))
        {
            return hit.point;
        }

        // If nothing is hit, aim straight forward at max distance
        return muzzle.position + muzzle.forward * maxDistance;
    }
}
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
        if (muzzle == null || realAim == null || mainCamera == null || canvasGroup == null)
        {
            return;
        }

        Vector3 targetWorldPosition = GetRealAimPoint();
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(targetWorldPosition);

        if (screenPosition.z < 0f)
        {
            canvasGroup.alpha = 0f;
            return;
        }

        canvasGroup.alpha = 1f;

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

        return muzzle.position + muzzle.forward * maxDistance;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericGun : MonoBehaviour
{
    [Header("Weapon")]
    public int clipMax = 30;
    public int clipCurrent = 30;
    public bool automatic = true;
    [Min(1f / 60f)]
    public float fireRate = 0.1f;
    public float reloadTime = 0.5f;

    [Header("Firing")]
    public UnityEvent onFire;
    public Transform firePoint;
    public GameObject bullet;

    [Header("Animation")]
    public float positionRecover;
    public float rotationRecover;
    public Vector3 knockbackPosition;
    public Vector3 knockbackRotation;

    Vector3 originalPosition;
    Quaternion originalRotation;

    // Timer to control fire rate between shots
    private float fireTimer;

    // Flag to know if weapon is currently reloading
    private bool reloading;

    // Input system reference to read attack button
    private InputSystem_Actions input;

    private void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;

        // Initialize input system to detect shooting
        input = new InputSystem_Actions();
        input.Enable();
    }

    void Update()
    {
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            originalPosition,
            positionRecover * Time.deltaTime
        );

        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            originalRotation,
            rotationRecover * Time.deltaTime
        );

        // Handle shooting logic every frame
        HandleFire();
    }

    // Controls when the weapon can shoot: automatic mode, fire rate, ammo & reolad
    private void HandleFire()
    {
        if (reloading)
        {
            return;
        }

        fireTimer -= Time.deltaTime;

        bool wantsToFire =
            automatic
                ? input.Player.Attack.IsPressed()
                : input.Player.Attack.WasPressedThisFrame();

        if (!wantsToFire || fireTimer > 0f)
        {
            return;
        }

        if (clipCurrent <= 0)
        {
            StartCoroutine(ReloadCoroutine());
            return;
        }

        Fire();
        clipCurrent--;
        fireTimer = fireRate;
    }

    // Waits reloadTime and refills the clip
    private IEnumerator ReloadCoroutine()
    {
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        clipCurrent = clipMax;
        reloading = false;
    }

    public void Fire()
    {
        Destroy(Instantiate(bullet, firePoint.position, firePoint.rotation), 10);
        onFire.Invoke();
        StartCoroutine(Knockback_Corutine());
    }

    IEnumerator Knockback_Corutine()
    {
        yield return null;
        transform.localPosition -= new Vector3(
            Random.Range(-knockbackPosition.x, knockbackPosition.x),
            Random.Range(0, knockbackPosition.y),
            Random.Range(-knockbackPosition.z, -knockbackPosition.z * .5f)
        );

        transform.localEulerAngles -= new Vector3(
            Random.Range(knockbackRotation.x * 0.5f, knockbackRotation.x),
            Random.Range(-knockbackRotation.y, knockbackRotation.y),
            Random.Range(-knockbackRotation.z, knockbackRotation.z)
        );
    }
}

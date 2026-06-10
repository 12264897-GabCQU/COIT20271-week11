using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 12f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 22f;
    public float fireRate = 0.25f;

    private Rigidbody rb;
    private float nextFireTime = 0f;
    private bool setupOk = false;
    private bool hasLoggedShootConfigError = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("[PlayerController] Missing Rigidbody on player.");
            return;
        }

        if (bulletPrefab == null)
            Debug.LogError("[PlayerController] bulletPrefab is not assigned!");
        else if (firePoint == null)
            Debug.LogError("[PlayerController] firePoint is not assigned!");
        else
            setupOk = true;

        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void Update()
    {
        if (Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space))
            TryShoot();
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0f, v).normalized;
        rb.AddForce(dir * moveSpeed, ForceMode.Force);

        if (dir.magnitude > 0.1f)
        {
            if (AudioManager.instance != null)
                AudioManager.instance.PlayBallMove();
        }
        else
        {
            if (AudioManager.instance != null)
                AudioManager.instance.StopBallMove();
        }

        Vector3 pos = rb.position;
        if (pos.y != 0.5f)
        {
            pos.y = 0.5f;
            rb.MovePosition(pos);
        }
    }

    void TryShoot()
    {
        if (!setupOk || Time.time < nextFireTime) return;

        if (bulletPrefab == null || firePoint == null)
        {
            setupOk = false;

            if (!hasLoggedShootConfigError)
            {
                Debug.LogError("[PlayerController] bulletPrefab or firePoint is missing.");
                hasLoggedShootConfigError = true;
            }

            return;
        }

        nextFireTime = Time.time + fireRate;

        if (AudioManager.instance != null)
            AudioManager.instance.PlayShoot();

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        Rigidbody bRb = bullet.GetComponent<Rigidbody>();

        if (bRb != null)
        {
            bRb.linearVelocity = firePoint.forward * bulletSpeed;
        }
        else
        {
            Debug.LogError("[PlayerController] Bullet prefab is missing a Rigidbody!");
        }

        Destroy(bullet, 4f);
    }

    void OnDisable()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.StopBallMove();
    }
}
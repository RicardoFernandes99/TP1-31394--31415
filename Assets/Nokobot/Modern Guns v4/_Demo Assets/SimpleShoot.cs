using UnityEngine;

[AddComponentMenu("Nokobot/Modern Guns/Simple Shoot")]
public class SimpleShoot : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Location References")]
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    [Header("Settings")]
    [Tooltip("Specify time to destroy the casing object")]
    [SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")]
    [SerializeField] private float shotPower = 500f;
    [Tooltip("Casing Ejection Speed")]
    [SerializeField] private float ejectPower = 150f;

    [Header("Shotgun Settings")]
    [Tooltip("Dispara múltiplas balas com dispersão")]
    public bool isShotgun = false;
    [Tooltip("Número de balas por disparo quando isShotgun=true")]
    public int pellets = 6;
    [Tooltip("Dispersão em graus (cone)")]
    public float spreadAngle = 5f;

    [Header("Recoil Settings")]
    [Tooltip("Quanto a arma recua")]
    [SerializeField] private float recoilKickback = 0.1f;
    [Tooltip("Velocidade a que a arma regressa")]
    [SerializeField] private float recoilReturnSpeed = 8f;

    [Header("Audio Settings")]
    [Tooltip("Som do disparo em .wav")]
    public AudioClip shotClip;
    [Tooltip("Fonte de áudio para disparo")]
    public AudioSource audioSource;

    private Vector3 originalLocalPos;
    private Vector3 recoilOffset = Vector3.zero;

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();

        // Caso não tenhas arrastado o AudioSource no Inspector
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        originalLocalPos = transform.localPosition;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            gunAnimator.SetTrigger("Fire");
    }

    void LateUpdate()
    {
        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, recoilReturnSpeed * Time.deltaTime);
        transform.localPosition = originalLocalPos + recoilOffset;
    }

    // Chamado pela animação no momento do disparo
    void Shoot()
    {
        // som do disparo
        if (shotClip != null && audioSource != null)
            audioSource.PlayOneShot(shotClip);

        recoilOffset += Vector3.back * recoilKickback;

        if (muzzleFlashPrefab)
        {
            GameObject tempFlash = Instantiate(
                muzzleFlashPrefab,
                barrelLocation.position,
                barrelLocation.rotation
            );
            Destroy(tempFlash, destroyTimer);
        }

        if (!bulletPrefab) return;

        if (isShotgun)
        {
            // Dispara vários projécteis com dispersão
            for (int i = 0; i < pellets; i++)
            {
                Quaternion spreadRot = barrelLocation.rotation *
                    Quaternion.Euler(
                        Random.Range(-spreadAngle, spreadAngle),   // pitch
                        Random.Range(-spreadAngle, spreadAngle),   // yaw
                        0f
                    );

                Rigidbody rb = Instantiate(bulletPrefab, barrelLocation.position, spreadRot)
                    .GetComponent<Rigidbody>();
                rb.AddForce(spreadRot * Vector3.forward * shotPower);
            }
        }
        else
        {
            Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation)
                .GetComponent<Rigidbody>()
                .AddForce(barrelLocation.forward * shotPower);
        }
    }

    void CasingRelease()
    {
        if (!casingExitLocation || !casingPrefab) return;

        GameObject tempCasing = Instantiate(
            casingPrefab,
            casingExitLocation.position,
            casingExitLocation.rotation
        );
        Rigidbody rb = tempCasing.GetComponent<Rigidbody>();
        rb.AddExplosionForce(
            Random.Range(ejectPower * 0.7f, ejectPower),
            casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f,
            1f
        );
        rb.AddTorque(
            new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)),
            ForceMode.Impulse
        );
        Destroy(tempCasing, destroyTimer);
    }
}

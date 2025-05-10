using UnityEngine;
using System.Collections.Generic;   // NOVO

[RequireComponent(typeof(CharacterController))]
public class MovimentoCC : MonoBehaviour
{
    /* ======= REFS ======= */
    [Header("Referências")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Transform gunHolder;

    /* ======= MOVIMENTO ======= */
    [Header("Movimento")]
    [SerializeField] float speed = 6f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float gravity = 12f;

    /* ======= CÂMARA ======= */
    [Header("Câmara")]
    [SerializeField] float sensMouse = 2f;
    [SerializeField] float clampPitch = 80f;

    /* ======= ARMAS ======= */
    [Header("Armas")]
    [SerializeField] Vector3 gunOffset = new(0.6f, -0.5f, 1f);
    [SerializeField] List<GameObject> gunPrefabs = new();   // NOVO
    GameObject armaActual;
    CharacterController cc;
    float pitch;
    Vector3 velY;

    /* ======= GUN BOBBING ======= */
    [Header("Gun Bobbing")]
    [SerializeField] float bobbingSpeed = 10f;
    [SerializeField] float bobbingAmount = 0.05f;
    private float defaultPosY = 0f;
    private float timer = 0f;
    private Vector3 originalGunPosition;
    private bool isMoving = false;


    void Start()
    {
        cc = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        EquiparArma(0);
        gunHolder.SetParent(playerCamera.transform, false);
        gunHolder.localPosition = gunOffset;
        originalGunPosition = gunOffset;
        defaultPosY = gunOffset.y;
    }

    void Update()
    {
        OlharCamera();
        Mover();
        GunBobbing();
    }

    void OlharCamera()
    {
        if (GetComponent<Health>().currentHealth <= 0)
            return;
        float yaw = Input.GetAxis("Mouse X") * sensMouse;
        float pitchDelta = Input.GetAxis("Mouse Y") * sensMouse;

        transform.Rotate(0f, yaw, 0f);

        pitch = Mathf.Clamp(pitch - pitchDelta, -clampPitch, clampPitch);
        playerCamera.transform.localEulerAngles = new(pitch, 0f, 0f);
    }

    /* ======= MOVIMENTO ======= */
    void Mover()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 move = (transform.forward * v + transform.right * h).normalized * speed;

        // Set isMoving flag for the gun bobbing effect
        isMoving = (h != 0 || v != 0) && cc.isGrounded;

        if (cc.isGrounded)
        {
            velY.y = -1f;
            if (Input.GetKeyDown(KeyCode.Space))
                velY.y = jumpSpeed;
        }
        else
        {
            velY.y -= gravity * Time.deltaTime;
        }

        cc.Move((move + velY) * Time.deltaTime);
    }

    /* ======= GUN BOBBING ======= */
    void GunBobbing()
    {
        if (GetComponent<Health>().currentHealth <= 0)
            return;

        if (isMoving)
        {
            // Calculate bobbing movement
            timer += Time.deltaTime * bobbingSpeed;
            float bobbingY = defaultPosY + Mathf.Sin(timer) * bobbingAmount;

            // Apply the bobbing effect
            Vector3 newPosition = originalGunPosition;
            newPosition.y = bobbingY;
            gunHolder.localPosition = newPosition;
        }
        else
        {
            // Gradually return to original position when not moving
            timer = 0;
            gunHolder.localPosition = Vector3.Lerp(gunHolder.localPosition, originalGunPosition, Time.deltaTime * bobbingSpeed);
        }
    }

    public void EquiparArma(int index)
    {
        if (index < 0 || index >= gunPrefabs.Count)
        {
            Debug.LogWarning($"Índice de arma inválido: {index}");
            return;
        }

        if (armaActual != null)
            Destroy(armaActual);

        armaActual = Instantiate(gunPrefabs[index], gunHolder);
        armaActual.transform.localPosition = Vector3.zero;
        armaActual.transform.localRotation = Quaternion.identity;
        armaActual.transform.localScale = Vector3.one;

        gunHolder = armaActual.transform;
        gunHolder.SetParent(playerCamera.transform, false);
        gunHolder.localPosition = gunOffset;
    }
    public void Heal(int heal)
    {
        GetComponent<Health>().Heal(heal);
    }
    public void TakeDamage(int damage)
    {
        GetComponent<Health>().TakeDamage(damage);

    }
}

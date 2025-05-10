using UnityEngine;
using System.Collections.Generic;

public class RPGShooter : MonoBehaviour
{
    [Header("Referências externas")]
    public Transform arma;
    public Transform pontoRecarga;
    public GameObject rpgPrefab;

    [Header("Disparo")]
    public float forcaDisparo = 1200f;
    public float tempoEntreDisparos = 1.2f;

    [Header("Áudio")]
    public AudioClip somDisparo;

    [Header("Animação de recarregar")]
    public Vector3 rotacaoParaBaixo = new Vector3(-40f, 0f, 0f);
    public Vector3 rotacaoParaCima = Vector3.zero;
    public float velocidadeAnim = 5f;

    GameObject rpgCarregado;
    RPGProjectile rpgProjectileComponent;
    bool aRecarregar = false;

    void Start()
    {
        SpawnRpgCarregado();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !aRecarregar)
            Dispara();
    }

    void Dispara()
    {
        if (rpgCarregado == null)
            return;

        if (somDisparo != null)
            AudioSource.PlayClipAtPoint(somDisparo, transform.position);

        /* Solta a granada do tubo */
        GameObject rocketToFire = rpgCarregado;  // Cria referência local
        rpgCarregado = null;  // Limpa a referência global imediatamente

        rocketToFire.transform.parent = null;
        var rb = rocketToFire.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(arma.forward * forcaDisparo, ForceMode.Impulse);

        // Notifica o projetil que foi disparado
        if (rocketToFire.TryGetComponent<RPGProjectile>(out var projectile))
        {
            projectile.Disparar();
        }

        StartCoroutine(RecarregarRoutine());
    }

    System.Collections.IEnumerator RecarregarRoutine()
    {
        aRecarregar = true;

        while (Quaternion.Angle(arma.localRotation, Quaternion.Euler(rotacaoParaBaixo)) > 0.5f)
        {
            arma.localRotation = Quaternion.Lerp(
                arma.localRotation,
                Quaternion.Euler(rotacaoParaBaixo),
                Time.deltaTime * velocidadeAnim);
            yield return null;
        }

        SpawnRpgCarregado();
        yield return new WaitForSeconds(tempoEntreDisparos);

        while (Quaternion.Angle(arma.localRotation, Quaternion.Euler(rotacaoParaCima)) > 0.5f)
        {
            arma.localRotation = Quaternion.Lerp(
                arma.localRotation,
                Quaternion.Euler(rotacaoParaCima),
                Time.deltaTime * velocidadeAnim);
            yield return null;
        }

        aRecarregar = false;
    }

    /* utilitário: cria uma granada "presa" ao cano */
    void SpawnRpgCarregado()
    {
        if (pontoRecarga == null || rpgPrefab == null) return;

        rpgCarregado = Instantiate(
            rpgPrefab,
            pontoRecarga.position,
            pontoRecarga.rotation,
            pontoRecarga);

        var rb = rpgCarregado.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }
}
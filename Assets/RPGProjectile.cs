using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RPGProjectile : MonoBehaviour
{
    [Header("Explosão")]
    [Tooltip("Prefab do efeito visual da explosão (opcional).")]
    public GameObject explosionPrefab;

    [Tooltip("Raio de dano da explosão.")]
    public float raioExplosao = 5f;

    [Header("Áudio")]
    public AudioClip somImpacto;
    [Range(0f, 1f)] public float volumeSom = 1f;

    // AudioSource que fica no mesmo GameObject da granada
    AudioSource audioSrc;
    
    // Estado do projétil - adicionado para controle
    private bool foiDisparado = false;
    private bool estaDestruido = false;

    void Awake()
    {
        // Cria ou reaproveita um AudioSource dedicado a efeitos
        audioSrc = GetComponent<AudioSource>();
        if (audioSrc == null) audioSrc = gameObject.AddComponent<AudioSource>();

        audioSrc.spatialBlend = 1f;   // som 3D
        audioSrc.playOnAwake = false;
    }
    
    // Método para marcar que o projétil foi disparado
    public void Disparar()
    {
        foiDisparado = true;
    }

    void TocaSomImpacto(Vector3 pos)
    {
        if (somImpacto == null) return;

        // cria um emissor temporário
        GameObject go = new GameObject("Impacto-RPG-SFX");
        go.transform.position = pos;

        var src = go.AddComponent<AudioSource>();

        src.clip = somImpacto;
        src.spatialBlend = 1f;          // som 3D
        src.volume = volumeSom;         // se quiseres exagerar, põe 1.5f, 2f, etc.

        // **estes dois controlam o alcance**:
        src.minDistance = 4f;           // raio a 100 % de volume
        src.maxDistance = 120f;         // distância onde atenua a zero
        src.rolloffMode = AudioRolloffMode.Linear;  // linear ou log, consoante preferires

        src.Play();
        Destroy(go, somImpacto.length);
    }

    void OnCollisionEnter(Collision col)
    {
        // Verifica se este projétil já foi destruído ou não foi disparado
        if (estaDestruido || !foiDisparado)
            return;
            
        // Marca como destruído para evitar processamento múltiplo
        estaDestruido = true;
        
        Vector3 impacto = col.contacts.Length > 0 ? col.contacts[0].point : transform.position;

        TocaSomImpacto(impacto);

        // 2) Efeito visual (se existir)
        if (explosionPrefab != null)
        {
            GameObject fx = Instantiate(explosionPrefab, impacto, Quaternion.identity);
            CameraShake.Instance?.Shake(0.15f, 0.2f);   // 0.25 s de tremor com 0.4 de intensidade

            fx.transform.localScale *= 2f; // Make the explosion 2 times bigger

            if (fx.TryGetComponent(out ParticleSystem ps))
            {
                var main = ps.main;
                float vida = main.duration + main.startLifetime.constantMax;
                vida = vida / 2;
                Destroy(fx, vida);
            }
            else
            {
                Destroy(fx, 5f);
            }
        }

        // 3) Elimina todos os zombies dentro do raio
        Collider[] hits = Physics.OverlapSphere(impacto, raioExplosao);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("zombie"))
            {
                hit.gameObject.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
            }
        }

        Debug.Log($"RPG explodiu contra {col.gameObject.name}. {hits.Length} colliders verificados.");

        // 4) Destrói a própria granada com pequeno atraso para evitar conflitos
        Destroy(gameObject, 0.05f);
    }
}
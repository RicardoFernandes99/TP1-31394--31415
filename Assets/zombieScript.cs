using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ZombieScript : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;                    // para seguir
    public GameObject player_to_kill;           // para causar dano
    public GameObject player_to_kill_follow;    // para medir distância

    [Header("Attack Settings")]
    public float hitDistance = 1.5f;            // distância para atacar
    public float attackDamage = 10f;            // dano por ataque

    [Header("Death Settings")]
    public float timeToDie = 10f;               // segundos até destruir após morrer

    [Header("Sound Settings")]
    public AudioClip[] zombieSounds;            // array de sons do zombie
    public float minSoundInterval = 5f;         // intervalo mínimo entre sons
    public float maxSoundInterval = 15f;        // intervalo máximo entre sons

    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;
    private Transform zombieTransform;

    private bool dead = false;
    private bool isHitting = false;
    private float nextSoundTime = 0f;

    void Start()
    {
        zombieTransform = transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        animarCorrer();
        ScheduleNextSound();
    }

    void Update()
    {
        if (dead || isHitting || player_to_kill_follow == null)
            return;

        agent.SetDestination(player.position);

        // atacar se estiver perto
        float dist = Vector3.Distance(zombieTransform.position, player_to_kill_follow.transform.position);
        if (dist <= hitDistance)
            StartCoroutine(DoAttack());

        // tocar som de vez em quando
        if (Time.time >= nextSoundTime)
            PlayRandomSound();
    }

    IEnumerator DoAttack()
    {
        isHitting = true;
        animator.SetTrigger("zombiehit");
        player_to_kill.GetComponent<MovimentoCC>().TakeDamage((int)attackDamage);
        yield return new WaitForSeconds(1f);    // tempo de animação de ataque
        isHitting = false;
        animarCorrer();
    }

    void PlayRandomSound()
    {
        if (zombieSounds.Length == 0)
            return;

        var clip = zombieSounds[Random.Range(0, zombieSounds.Length)];
        audioSource.PlayOneShot(clip);
        ScheduleNextSound();
    }

    void ScheduleNextSound()
    {
        nextSoundTime = Time.time + Random.Range(minSoundInterval, maxSoundInterval);
    }

    void animarCorrer()
    {
        animator.SetTrigger("zombierun");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (dead) return;

        if (collision.gameObject.CompareTag("tiro"))
        {
            Destroy(collision.gameObject);
            Die();
        }
    }

    void Die()
    {
        dead = true;
        agent.isStopped = true;
        animator.SetTrigger("zombiedie");
        player_to_kill.GetComponent<MovimentoCC>().Heal((int)2);
        player_to_kill.GetComponent<Score>().UpdateScore(1); 
        // notifica o manager
        WaveManager wm = FindFirstObjectByType<WaveManager>();
        if (wm != null)
            wm.NotifyZombieDeath();

        // desactivar colisores para não bloquear o caminho
        foreach (var col in GetComponents<Collider>())
            col.enabled = false;

        StartCoroutine(WaitAndDestroy());
    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(timeToDie);
        Destroy(gameObject);
    }
}
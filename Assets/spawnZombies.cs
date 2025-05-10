using UnityEngine;

[AddComponentMenu("Nokobot/Modern Guns/Spawn Zombies")]
public class SpawnZombies : MonoBehaviour
{
    [Header("Prefab do Zombie")]
    public GameObject zombiePrefab;

    [Header("Configurações de Spawn")]
    [Tooltip("Raio dentro do qual os zombies vão aparecer")]
    public float spawnRadius = 10f;
    [Tooltip("Número médio de zombies a criar")]
    public int baseAmount = 10;
    [Tooltip("Variação (offset) no número de zombies, ex: 10±5")]
    public int amountOffset = 5;


    public void SpawnZombiesFunc(float radius, int baseCount, int countOffset)
    {
        // Calcula número total de zombies: baseCount ± countOffset
        int totalZombies = baseCount + Random.Range(-countOffset, countOffset + 1);
        totalZombies = Mathf.Max(0, totalZombies);

        for (int i = 0; i < totalZombies; i++)
        {
            // Gera uma posição aleatória dentro do círculo de raio radius no plano XZ
            Vector2 rndCircle = Random.insideUnitCircle * radius;
            Vector3 spawnPos = new Vector3(rndCircle.x, 0f, rndCircle.y) + transform.position;

            Instantiate(zombiePrefab, spawnPos, Quaternion.identity);
        }
    }

    void Start()
    {
        if (zombiePrefab == null)
        {
            Debug.LogWarning("Prefab de zombie não atribuído em " + name);
            return;
        }
        SpawnZombiesFunc(spawnRadius, baseAmount, amountOffset);
    }
}

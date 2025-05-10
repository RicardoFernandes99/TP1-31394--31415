using UnityEngine;
using System.Collections;

[System.Serializable]
public class Wave
{
    public int zombieCount;       // quantos zombies nesta wave
    public float spawnRate = 1f;  // zombies por segundo

    public int gun_number;       // número da arma a matar para zombies
}

public class WaveManager : MonoBehaviour
{
    public MonoBehaviour player_script; // script do jogador
    public Wave[] waves;                  // configuras no Inspector
    public Transform[] spawnPoints;       // locais de spawn
    public GameObject zombiePrefab;       // prefab do zombie
    public int currentWave = 0;
    private int aliveZombies = 0;

    public int goBackWaves = 3; // Quantas waves voltar quando acabar

    public int percentagemExtra = 0;


    void Start()
    {
        StartCoroutine(RunWave(currentWave));
    }

    IEnumerator RunWave(int waveIndex)
    {
        Wave wave = waves[waveIndex];
        for (int i = 0; i < wave.zombieCount; i++)
        {
            SpawnZombie();
            aliveZombies++;
            yield return new WaitForSeconds(1f / wave.spawnRate);
        }
        player_script.SendMessage("EquiparArma", wave.gun_number); // muda a arma do jogador
    }

    void SpawnZombie()
    {
        // escolhe um ponto de spawn aleatório
        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(zombiePrefab, sp.position, sp.rotation);
    }

    // este método é chamado por cada zombie quando morre
    public void NotifyZombieDeath()
    {
        aliveZombies--;
        if (aliveZombies <= 0)
        {
            currentWave++;
            if (currentWave < waves.Length)
            {
                StartCoroutine(RunWave(currentWave));
            }
            else
            {
                Debug.Log("Acabaram-se as waves! Voltando atrás para continuar infinitamente.");
                currentWave = Mathf.Max(0, waves.Length - goBackWaves); // Volta X waves ou para a primeira
                percentagemExtra = percentagemExtra + 10; // Aumenta a dificuldade
                for (int i = 0; i < waves.Length; i++)
                {
                    waves[i].zombieCount += Mathf.FloorToInt(waves[i].zombieCount * percentagemExtra / 100);
                    waves[i].spawnRate += Mathf.FloorToInt(waves[i].spawnRate * percentagemExtra / 100);
                }
                StartCoroutine(RunWave(currentWave));
            }
        }
    }
}

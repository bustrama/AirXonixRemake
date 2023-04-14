using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager :MonoBehaviour {
    private int waveIndex = 0;
    public int enemyCount;

    public GameObject arena;
    public int[] waves;
    public GameObject[] enemies;

    private float spawnRange;

    // Start is called before the first frame update
    void Start() {
        BoxCollider arenaBox = arena.GetComponent<BoxCollider>();
        spawnRange = arenaBox.size.z * 2;
        SpawnEnemyWave(waves[waveIndex]);
    }

    void SpawnEnemyWave(int numOfEnemies) {
        for (int i = 0; i < numOfEnemies; i++) {
            Instantiate(enemies[0], GenerateSpawnPosition(enemies[0]), enemies[0].transform.rotation);
            if (i > 0) {
                Instantiate(enemies[1], GenerateSpawnPosition(enemies[1]), enemies[1].transform.rotation);
            }
            if (i > 3) {
                Instantiate(enemies[2], GenerateSpawnPosition(enemies[2]), enemies[2].transform.rotation);
            }
        }
    }

    private Vector3 GenerateSpawnPosition(GameObject prefab) {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);
        return new Vector3(spawnPosX, prefab.transform.position.y, spawnPosZ);
    }

    // Update is called once per frame
    void Update() {
        enemyCount = FindObjectsOfType<Enemy>().Length;
        if (enemyCount == 0) {
            if (waveIndex + 1 >= waves.Length) {
                Debug.Log("Game Over");
            } else {
                waveIndex++;
                // Instantiate(powerupPrefab, GenerateSpawnPosition(), powerupPrefab.transform.rotation);
                SpawnEnemyWave(waves[waveIndex]);
            }
        }
    }
}

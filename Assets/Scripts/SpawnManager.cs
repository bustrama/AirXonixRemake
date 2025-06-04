using System.Collections;
using UnityEngine;

public class SpawnManager :MonoBehaviour {
    public int[] waves;
    public GameObject[] enemies;

    private GameObject arena;
    private int waveIndex = 0;
    private int enemyCount;
    private float spawnRange;
    private bool initialized = false;

    // Start is called before the first frame update
    void Start() {
        var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (sceneName == "MainMenu" || sceneName == "LevelSelect") {
            return;
        }
        StartCoroutine(WaitForArena());
    }

    IEnumerator WaitForArena() {
        // Wait until the end of the frame
        yield return new WaitForEndOfFrame();

        arena = GameObject.Find("arena");

        // Wait until the game object has been created
        while (arena == null) {
            yield return null;
        }

        // The game object has been created, do something with it
        BoxCollider arenaBox = arena.GetComponent<BoxCollider>();
        spawnRange = arenaBox.size.z * 2;
        SpawnEnemyWave(waves[waveIndex]);
        initialized = true;
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
        if (initialized) {
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
}

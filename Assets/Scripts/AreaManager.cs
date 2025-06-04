using System.Collections.Generic;
using UnityEngine;

public class AreaManager : MonoBehaviour {
    public int gridSize = 20;
    public GameObject arena;
    public float cellSize;
    public GameObject groundTilePrefab;

    private Dictionary<Vector2Int, GameObject> groundTiles = new Dictionary<Vector2Int, GameObject>();

    void Start() {
        if (arena == null) {
            arena = GameObject.Find("arena");
        }
        float arenaSize = arena != null ? arena.transform.localScale.x : 1f;
        cellSize = (arenaSize * 10f) / gridSize;

        // Create border ground tiles
        for (int x = 0; x < gridSize; x++) {
            for (int z = 0; z < gridSize; z++) {
                if (x == 0 || z == 0 || x == gridSize - 1 || z == gridSize - 1) {
                    CreateGroundTile(new Vector2Int(x, z));
                }
            }
        }
    }

    public Vector2Int WorldToGrid(Vector3 pos) {
        float startX = -arena.transform.localScale.x * 5f + cellSize / 2f;
        float startZ = -arena.transform.localScale.z * 5f + cellSize / 2f;
        int gx = Mathf.Clamp(Mathf.RoundToInt((pos.x - startX) / cellSize), 0, gridSize - 1);
        int gz = Mathf.Clamp(Mathf.RoundToInt((pos.z - startZ) / cellSize), 0, gridSize - 1);
        return new Vector2Int(gx, gz);
    }

    public Vector3 GridToWorld(Vector2Int index) {
        float startX = -arena.transform.localScale.x * 5f + cellSize / 2f;
        float startZ = -arena.transform.localScale.z * 5f + cellSize / 2f;
        return new Vector3(startX + index.x * cellSize, 0.5f, startZ + index.y * cellSize);
    }

    public void FillTrailArea(List<Vector3> positions) {
        if (positions == null || positions.Count == 0) return;

        Vector2Int min = WorldToGrid(positions[0]);
        Vector2Int max = min;
        foreach (var p in positions) {
            Vector2Int g = WorldToGrid(p);
            if (g.x < min.x) min.x = g.x;
            if (g.y < min.y) min.y = g.y;
            if (g.x > max.x) max.x = g.x;
            if (g.y > max.y) max.y = g.y;
        }
        for (int x = min.x; x <= max.x; x++) {
            for (int z = min.y; z <= max.y; z++) {
                CreateGroundTile(new Vector2Int(x, z));
            }
        }
    }

    private void CreateGroundTile(Vector2Int index) {
        if (groundTiles.ContainsKey(index)) return;

        Vector3 worldPos = GridToWorld(index);
        GameObject tile;
        if (groundTilePrefab != null) {
            tile = Instantiate(groundTilePrefab, worldPos, Quaternion.identity, transform);
        } else {
            tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.transform.position = worldPos;
            tile.transform.localScale = new Vector3(cellSize, 1f, cellSize);
        }
        tile.tag = "Ground";
        groundTiles[index] = tile;
    }
}

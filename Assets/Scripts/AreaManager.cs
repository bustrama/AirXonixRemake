using System.Collections.Generic;
using UnityEngine;

public class AreaManager : MonoBehaviour {
    public int gridSize = 20;
    public float cellSize;

    private Dictionary<Vector2Int, GameObject> groundTiles = new Dictionary<Vector2Int, GameObject>();

    void Start() {
        float arenaSize = transform.localScale.x;
        cellSize = (arenaSize * 10f) / gridSize;
    }

    public Vector2Int WorldToGrid(Vector3 pos) {
        Vector3 origin = transform.position - new Vector3(transform.localScale.x * 5f, 0f, transform.localScale.z * 5f)
                         + new Vector3(cellSize / 2f, 0f, cellSize / 2f);
        int gx = Mathf.Clamp(Mathf.RoundToInt((pos.x - origin.x) / cellSize), 0, gridSize - 1);
        int gz = Mathf.Clamp(Mathf.RoundToInt((pos.z - origin.z) / cellSize), 0, gridSize - 1);
        return new Vector2Int(gx, gz);
    }

    public Vector3 GridToWorld(Vector2Int index) {
        Vector3 origin = transform.position - new Vector3(transform.localScale.x * 5f, 0f, transform.localScale.z * 5f)
                         + new Vector3(cellSize / 2f, 0f, cellSize / 2f);
        return new Vector3(origin.x + index.x * cellSize, 0.5f, origin.z + index.y * cellSize);
    }

    public void FillTrailArea(List<Vector3> pathPoints) {
        if (pathPoints == null || pathPoints.Count < 3) return;

        // convert to 2D polygon (x,z)
        List<Vector2> polygon = new List<Vector2>();
        foreach (var p in pathPoints) {
            polygon.Add(new Vector2(p.x, p.z));
        }

        // compute bounding box in world coordinates
        Vector2 minWorld = polygon[0];
        Vector2 maxWorld = polygon[0];
        foreach (var p in polygon) {
            if (p.x < minWorld.x) minWorld.x = p.x;
            if (p.y < minWorld.y) minWorld.y = p.y;
            if (p.x > maxWorld.x) maxWorld.x = p.x;
            if (p.y > maxWorld.y) maxWorld.y = p.y;
        }

        Vector2Int min = WorldToGrid(new Vector3(minWorld.x, 0, minWorld.y));
        Vector2Int max = WorldToGrid(new Vector3(maxWorld.x, 0, maxWorld.y));

        for (int x = min.x; x <= max.x; x++) {
            for (int z = min.y; z <= max.y; z++) {
                Vector3 worldPos = GridToWorld(new Vector2Int(x, z));
                Vector2 p = new Vector2(worldPos.x, worldPos.z);
                if (IsPointInPolygon(p, polygon)) {
                    CreateGroundTile(new Vector2Int(x, z));
                }
            }
        }
    }

    private bool IsPointInPolygon(Vector2 point, List<Vector2> polygon) {
        bool inside = false;
        for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++) {
            Vector2 pi = polygon[i];
            Vector2 pj = polygon[j];
            if (((pi.y > point.y) != (pj.y > point.y)) &&
                (point.x < (pj.x - pi.x) * (point.y - pi.y) / (pj.y - pi.y) + pi.x)) {
                inside = !inside;
            }
        }
        return inside;
    }

    private void CreateGroundTile(Vector2Int index) {
        if (groundTiles.ContainsKey(index)) return;

        Vector3 worldPos = GridToWorld(index);
        GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tile.transform.position = worldPos;
        tile.transform.localScale = new Vector3(cellSize, 1f, cellSize);
        BoxCollider boxCollider = tile.GetComponent<BoxCollider>();
        boxCollider.center = new Vector3(0, 1, 0);
        boxCollider.size = new Vector3(1, 3, 1);
        boxCollider.isTrigger = true;
        tile.tag = "Ground";
        groundTiles[index] = tile;
    }
}

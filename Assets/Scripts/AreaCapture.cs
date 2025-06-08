using System.Collections.Generic;
using UnityEngine;

public class AreaCapture : MonoBehaviour {
    private bool[,] captured;
    private int width;
    private int height;
    private GameObject arena;

    void Start() {
        arena = GameObject.Find("arena");
        if (arena != null) {
            width = Mathf.RoundToInt(arena.transform.localScale.x * 10);
            height = Mathf.RoundToInt(arena.transform.localScale.z * 10);
            captured = new bool[width, height];
        }
    }

    public void FillArea(List<GameObject> trailColliders) {
        if (arena == null || trailColliders == null || trailColliders.Count == 0) {
            return;
        }

        bool[,] trail = new bool[width, height];
        foreach (GameObject go in trailColliders) {
            Vector3 pos = go.transform.position;
            int gx = Mathf.Clamp(Mathf.RoundToInt(pos.x + width * 0.5f), 0, width - 1);
            int gy = Mathf.Clamp(Mathf.RoundToInt(pos.z + height * 0.5f), 0, height - 1);
            trail[gx, gy] = true;
        }

        bool[,] visited = new bool[width, height];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        for (int x = 0; x < width; x++) {
            Enqueue(x, 0);
            Enqueue(x, height - 1);
        }
        for (int y = 0; y < height; y++) {
            Enqueue(0, y);
            Enqueue(width - 1, y);
        }

        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };
        while (queue.Count > 0) {
            Vector2Int p = queue.Dequeue();
            for (int i = 0; i < 4; i++) {
                int nx = p.x + dx[i];
                int ny = p.y + dy[i];
                if (nx >= 0 && nx < width && ny >= 0 && ny < height && !visited[nx, ny] && !trail[nx, ny]) {
                    visited[nx, ny] = true;
                    queue.Enqueue(new Vector2Int(nx, ny));
                }
            }
        }

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (!visited[x, y] && !trail[x, y] && !captured[x, y]) {
                    captured[x, y] = true;
                    SpawnFloor(x, y);
                }
            }
        }

        void Enqueue(int x, int y) {
            if (!visited[x, y] && !trail[x, y]) {
                visited[x, y] = true;
                queue.Enqueue(new Vector2Int(x, y));
            }
        }
    }

    void SpawnFloor(int x, int y) {
        Vector3 pos = new Vector3(x - width * 0.5f + 0.5f, 0.01f, y - height * 0.5f + 0.5f);
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.position = pos;
        quad.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        quad.tag = "Ground";
    }
}

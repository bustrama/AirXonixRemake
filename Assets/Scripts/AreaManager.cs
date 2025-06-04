using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the arena grid and handles area filling when the player closes a loop
/// </summary>
public class AreaManager : MonoBehaviour
{
    public int gridSize = 20;

    private bool[,] groundGrid;
    private GameObject arena;
    private float cellSize;

    void Start()
    {
        arena = GameObject.Find("arena");
        if (arena == null)
        {
            Debug.LogError("Arena object not found");
            return;
        }
        gridSize = Mathf.RoundToInt(arena.transform.localScale.x * 10);
        groundGrid = new bool[gridSize, gridSize];
        cellSize = (arena.transform.localScale.x * 10f) / gridSize;

        // mark borders as ground
        for (int x = 0; x < gridSize; x++)
        {
            groundGrid[x, 0] = true;
            groundGrid[x, gridSize - 1] = true;
            groundGrid[0, x] = true;
            groundGrid[gridSize - 1, x] = true;
        }
    }

    public Vector2Int WorldToGrid(Vector3 position)
    {
        float half = gridSize * 0.5f * cellSize;
        int gx = Mathf.Clamp(Mathf.RoundToInt((position.x + half) / cellSize), 0, gridSize - 1);
        int gy = Mathf.Clamp(Mathf.RoundToInt((position.z + half) / cellSize), 0, gridSize - 1);
        return new Vector2Int(gx, gy);
    }

    public void MarkGround(Vector3 position)
    {
        Vector2Int g = WorldToGrid(position);
        groundGrid[g.x, g.y] = true;
    }

    /// <summary>
    /// Fill the area enclosed by the player path using a flood fill starting from enemies
    /// </summary>
    public void CloseArea(List<Vector3> pathPoints, List<Transform> enemies)
    {
        if (pathPoints == null || pathPoints.Count == 0)
            return;

        bool[,] temp = (bool[,])groundGrid.Clone();
        foreach (var p in pathPoints)
        {
            Vector2Int g = WorldToGrid(p);
            temp[g.x, g.y] = true;
        }

        bool[,] visited = new bool[gridSize, gridSize];
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        foreach (var enemy in enemies)
        {
            Vector2Int ge = WorldToGrid(enemy.position);
            if (!visited[ge.x, ge.y])
            {
                visited[ge.x, ge.y] = true;
                q.Enqueue(ge);
            }
        }

        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };
        while (q.Count > 0)
        {
            Vector2Int c = q.Dequeue();
            for (int i = 0; i < 4; i++)
            {
                int nx = c.x + dx[i];
                int ny = c.y + dy[i];
                if (nx >= 0 && ny >= 0 && nx < gridSize && ny < gridSize && !visited[nx, ny] && !temp[nx, ny])
                {
                    visited[nx, ny] = true;
                    q.Enqueue(new Vector2Int(nx, ny));
                }
            }
        }

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (!visited[x, y] && !groundGrid[x, y])
                {
                    groundGrid[x, y] = true;
                    CreateGroundCell(x, y);
                }
            }
        }
    }

    private void CreateGroundCell(int gx, int gy)
    {
        float half = gridSize * 0.5f * cellSize;
        GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cell.transform.localScale = new Vector3(cellSize, 1f, cellSize);
        cell.transform.position = new Vector3((gx + 0.5f) * cellSize - half, 0.5f, (gy + 0.5f) * cellSize - half);
        cell.tag = "Ground";
    }
}


# Xonix Remake – Agents Overview (AGENTS.md)

## 1. Summary
Xonix is a territory-capture arcade game. The player steers a marker over a rectangular “sea” and, by drawing a closed path, converts the enclosed water into solid land.  
A modern Unity remake hinges on four pillars:

1. **Grid representation** of the play-field.  
2. **Line tracing** that lays a temporary trail while the player is off-land.  
3. **Area-fill algorithm** that decides which side of the finished path becomes new land.  
4. **Enemy AI** that patrols water and punishes collisions with the path or player.  

The recommended fill routine is **dual flood-fill**: flood from each enemy position; any water *not* reached becomes land.  
This avoids tricky polygon math and naturally handles concave shapes and multiple islands.

Every core behaviour is wrapped in a small, single-purpose *agent* (MonoBehaviour).  
This file documents those agents so that ChatGPT Codex (or any other LLM) can load the project with full context.

---

## 2. Game-World Model

| Concept            | Notes                                                        |
|--------------------|--------------------------------------------------------------|
| **CellType enum**  | `Water`, `Land`, `Trail`, `Enemy`                            |
| **Playfield**      | `int width, height; CellType[,] grid;`                       |
| **Player state**   | `bool isDrawing; List<Vector2Int> currentTrail;`             |
| **Enemies**        | `List<EnemyAgent>`; each enemy stores its own path logic     |

A grid keeps updates **O(1)** per cell and enables trivial flood-fills without real-time boolean mesh ops.

---

## 3. Agent Catalogue

### 3.1 GameManagerAgent
* **Role:** Bootstraps the grid, tracks score/lives/level, dispatches global events (`GameOver`, `AreaCaptured`).
* **Key API**

    ```csharp
    void Init(int w, int h, int enemyCount);
    void OnTrailClosed(List<Vector2Int> loop);
    ```

### 3.2 PlayerAgent
* 4-way movement on keyboard/controller.  
* Lays `Trail` cells while `isDrawing`.  
* Emits `TrailClosed` when head re-enters land.  
* Dies on collision with an enemy or if an enemy touches an open trail.

### 3.3 EnemyAgent
* Moves continuously on water (land-only variants later).  
* Simple random-turn or A* toward the player.  
* Registers its grid position with **FloodFillAgent** each frame so the fill algorithm always knows enemy seeds.

### 3.4 FloodFillAgent
* **API**

    ```csharp
    void CaptureArea(List<Vector2Int> loop,
                     IEnumerable<Vector2Int> enemySeeds);
    ```

* Performs dual flood-fill (see §4).  
* Calculates captured percentage and notifies GameManagerAgent.

### 3.5 UIAgent
* Renders HUD: score, lives, captured-percent bar, level number.  
* Shows Game-Over and Next-Level screens.

---

## 4. Filling Area Algorithm – Dual Flood-Fill

1. **Commit the player’s loop**  
   * Convert `Trail` cells in `loop` to a temporary wall (`TempWall`).  
   * Verify the loop is watertight (each consecutive pair shares an edge).  

2. **Flood from every enemy**  
   * Breadth-first search through water, starting at each enemy; mark visited cells `EnemyReachable`.  

3. **Convert residual water**  
   * Iterate the grid; any `Water` cell *not* marked becomes `Land`.  
   * Convert `TempWall` → `Land` and `EnemyReachable` → `Water`.  

4. **Update score** by the number of new land cells.

### Why it works
Flooding from enemy positions guarantees you never fill an area containing an active enemy, matching the classic rule set.  
The technique is robust to concavities, islands, and multiple enemies.

### Performance Tips
* Use a `byte[,]` instead of an enum array to keep cache lines hot.  
* Split the flood into 32 × 32 tiles and process a few per frame for huge maps.  
* For enormous grids, port the flood code to Unity Job System + Burst; classic fields (≈ 200 × 200) run fine on the main thread.  
* Scan-line polygon fill is faster for tiny loops but needs logic to choose “which side” to keep.

---

## 5. Typical Agent Interaction

    ```mermaid
    sequenceDiagram
        Player->>Player: Walks on Land
        Player->>Player: Enters Water (isDrawing=true)
        loop path progress
            Player->>Grid: mark cell = Trail
            Enemy->>Grid: move each frame
            Enemy->>Player: collision?
        end
        Player->>Player: returns to Land (loop closed)
        Player-->>GameManagerAgent: TrailClosed(loop)
        GameManagerAgent-->>FloodFillAgent: CaptureArea(loop, enemySeeds)
        FloodFillAgent-->>GameManagerAgent: percentCaptured
        alt level complete
            GameManagerAgent-->>Scene: load next level
        end
    ```

---

## 6. Notes for Codex-Powered Workflows
* Place **AGENTS.md** in the repo root so the LLM can load it as context.  
* When you ask for code, mention the agent and method you’re touching: *“Update FloodFillAgent to use Unity Jobs.”*  
* Because each agent has single responsibility, the model can open the correct file and stay within context limits.

---

## 7. Further Reading (non-exhaustive)
* Unity Forum threads on “Xonix” / “Qix” clones.  
* Stack Exchange discussions on grid flood-fill vs mesh boolean approaches.  
* Retro-computing blogs reverse-engineering original Qix hardware.  
* Classic computer-graphics texts on scan-line polygon fill algorithms.
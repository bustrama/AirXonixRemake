# AirXonixRemake – Agent Guidelines

## Summary
AirXonixRemake is a Unity 2021.3 project that reimagines the classic territory-capture arcade game "Xonix". The current prototype contains basic player movement, simple enemies and a minimal UI. The long term goal is to allow the player to claim areas of the arena by drawing closed loops while avoiding enemy contact.

## Repository Layout
- `Assets/Scripts` – C# MonoBehaviours that implement game logic.
- `Assets/Scenes` – Unity scenes (`Main Menu.unity`, `My Game.unity`).
- `Assets/Docs` – design PDF and reference images.
- `Assets/Prefabs`, `Assets/Textures`, `Assets/Materials`, `Assets/Sounds` – art and audio assets.
- `ProjectSettings` – Unity project settings (2021.3.22f1).
- `Packages` – package manifest and lock file.

## Existing Agents
- **GameManager** – creates the arena, borders and spawns the player.
- **PlayerControls** – handles 4‑way movement, places trail colliders and triggers `GameOver` on collisions.
- **Enemy** – basic movement along axes and interaction with player trails.
- **SpawnManager** – spawns waves of enemies once the arena exists.
- **GameOverUI** – shows a restart screen when the player dies.
- **MainMenu** – builds the start menu UI.
- **Spin** – spins an assigned object for simple effects.

## Development Notes
- Keep one MonoBehaviour per C# file inside `Assets/Scripts`.
- Use PascalCase for class and method names and camelCase for private fields.
- Expose private inspector variables with `[SerializeField]` where needed.
- Let the Unity editor generate `.meta` files for new assets.

## Building and Testing
- Open the project with Unity **2021.3.22f1** or newer.
- There are no automated tests; verify behaviour by running the scenes in the Unity editor.
- Ensure `Main Menu` and `My Game` continue to load correctly after changes.

## Future Work
- Implement grid-based territory capture and the dual flood‑fill algorithm described in the design docs.
- Expand enemy AI and add level progression.

## Using This Document with Codex
- `AGENTS.md` resides at the repo root so AI tools can load it automatically.
- When asking for code, mention the script and method you want changed: e.g. “Modify `SpawnManager.SpawnEnemyWave` to randomise enemy types.”

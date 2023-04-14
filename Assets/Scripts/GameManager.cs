using UnityEngine;

public class GameManager :MonoBehaviour {
    [SerializeField] private GameObject player;
    [SerializeField] private int size;
    private GameObject arena;
    private GameObject[] arenaBorders;

    void Start() {
        SetupArena();
        SetupBorders();
        SpawnPlayer();
    }

    void SetupArena() {
        arena = GameObject.CreatePrimitive(PrimitiveType.Plane);
        BoxCollider boxCollider = arena.AddComponent<BoxCollider>();
        // Set the position of the plane
        arena.transform.position = new Vector3(0, 0, 0);
        // Set the size of the plane
        arena.transform.localScale = new Vector3(size, 1, size);
        arena.name = "arena";
    }

    void SetupBorders() {
        arenaBorders = new GameObject[4];
        for (var i = 0; i < 4; i++) {
            arenaBorders[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arenaBorders[i].transform.localScale = new Vector3(2, 1, 10 * size);

            // Setup Collider for each border
            BoxCollider boxCollider = arenaBorders[i].GetComponent<BoxCollider>();
            boxCollider.center = new Vector3(0, 1, 0);
            boxCollider.size = new Vector3(1, 3, 1);
            boxCollider.isTrigger = true;

            // Setup `Ground` tag for each border
            arenaBorders[i].tag = "Ground";
        }

        // Right and Left borders
        arenaBorders[0].transform.position = new Vector3(5 * size - 1, 0.5f, 0);
        arenaBorders[1].transform.position = new Vector3(-5 * size + 1, 0.5f, 0);

        // Top and Bottom Borders
        arenaBorders[2].transform.position = new Vector3(0, 0.5f, 5 * size - 1);
        arenaBorders[2].transform.Rotate(new Vector3(0, 90, 0));
        arenaBorders[3].transform.position = new Vector3(0, 0.5f, -5 * size + 1);
        arenaBorders[3].transform.Rotate(new Vector3(0, 90, 0));
    }

    void SpawnPlayer() {
        Instantiate(player, new Vector3(0, 1.5f, -5 * size + 1), player.transform.rotation);
    }
}

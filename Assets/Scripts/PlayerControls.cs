using System.Collections.Generic;
using UnityEngine;

public class PlayerControls :MonoBehaviour {
    [SerializeField] private float moveSpeed = 20.0f;
    [SerializeField] private bool onGround = false;
    [SerializeField] private bool gameOver = false;
    [SerializeField] private GameObject trailColliderPrefab;
    [SerializeField] private ParticleSystem explosionPrefab;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private AreaManager areaManager;

    private TrailRenderer trailRenderer;
    private List<GameObject> trailColliders = new List<GameObject>();
    private Vector3 lastColliderInitialPoint;

    private AudioSource explosionSound;
    private float horizontalInput;
    private float verticalInput;
    private GameObject arena;
    private Vector3 lastDirection;
    private float maxDistance;

    // Start is called before the first frame update
    void Start() {
        arena = GameObject.Find("arena");
        maxDistance = 5 * arena.transform.localScale.x - 1;
        transform.position = new Vector3(0, 2, -maxDistance);

        trailRenderer = gameObject.GetComponent<TrailRenderer>();

        explosionSound = gameObject.GetComponent<AudioSource>();

        if (gameOverUI == null) {
            gameOverUI = FindObjectOfType<GameOverUI>();
        }
        if (areaManager == null) {
            areaManager = FindObjectOfType<AreaManager>();
        }
    }

    // Update is called once per frame
    void Update() {
        PlayerMovement();
        PlayerTrail();
    }

    void PlayerMovement() {
        // get the user's vertical input
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        if (onGround) {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime * -verticalInput);
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime * horizontalInput);
        } else {
            if (lastDirection != Vector3.up && lastDirection != Vector3.down && Mathf.Abs(verticalInput) > Mathf.Abs(horizontalInput)) {
                float direction = verticalInput > 0 ? 1 : -1;
                transform.Translate(Vector3.up * moveSpeed * Time.deltaTime * -direction);
                lastDirection = verticalInput < 0 ? Vector3.up : Vector3.down;
                CreateTrailCollider();
            } else if (lastDirection != Vector3.right && lastDirection != Vector3.left && Mathf.Abs(horizontalInput) > Mathf.Abs(verticalInput)) {
                float direction = horizontalInput > 0 ? 1 : -1;
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime * direction);
                lastDirection = horizontalInput > 0 ? Vector3.right : Vector3.left;
                CreateTrailCollider();
            } else {
                transform.Translate(lastDirection * moveSpeed * Time.deltaTime);
                if (lastDirection == Vector3.up || lastDirection == Vector3.down) {
                    trailColliders[trailColliders.Count - 1].transform.position = new Vector3(
                        trailColliders[trailColliders.Count - 1].transform.position.x, 
                        trailColliders[trailColliders.Count - 1].transform.position.y, 
                        (transform.position.z + lastColliderInitialPoint.z) / 2);
                    trailColliders[trailColliders.Count - 1].transform.localScale = new Vector3(
                        trailColliders[trailColliders.Count - 1].transform.localScale.x, 
                        trailColliders[trailColliders.Count - 1].transform.localScale.y, 
                        Mathf.Abs(transform.position.z - lastColliderInitialPoint.z));
                } else {
                    trailColliders[trailColliders.Count - 1].transform.position = new Vector3(
                        (transform.position.x + lastColliderInitialPoint.x) / 2, 
                        trailColliders[trailColliders.Count - 1].transform.position.y, 
                        trailColliders[trailColliders.Count - 1].transform.position.z);
                    trailColliders[trailColliders.Count - 1].transform.localScale = new Vector3(
                        Mathf.Abs(transform.position.x - lastColliderInitialPoint.x), 
                        trailColliders[trailColliders.Count - 1].transform.localScale.y, 
                        trailColliders[trailColliders.Count - 1].transform.localScale.z);
                }
            }
        }

        if (transform.position.x > maxDistance) {
            transform.position = new Vector3(maxDistance, transform.position.y, transform.position.z);
        }
        if (transform.position.x < -maxDistance) {
            transform.position = new Vector3(-maxDistance, transform.position.y, transform.position.z);
        }
        if (transform.position.z > maxDistance) {
            transform.position = new Vector3(transform.position.x, transform.position.y, maxDistance);
        }
        if (transform.position.z < -maxDistance) {
            transform.position = new Vector3(transform.position.x, transform.position.y, -maxDistance);
        }
    }

    void PlayerTrail() {
        if (onGround) {
            // Clear the trail renderer if the player is grounded
            trailRenderer.Clear();
        }
    }

    void CreateTrailCollider() {
        Debug.Log("Created Trail Collider");
        GameObject trailCollider = Instantiate(trailColliderPrefab, transform.position, trailColliderPrefab.transform.rotation);
        lastColliderInitialPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        trailColliders.Add(trailCollider);
    }

    void KillTrailColliders() {
        Debug.Log("Killing Trail Colliders");
        foreach (var trail in trailColliders) {
            Destroy(trail.gameObject);
        }
        trailColliders = new List<GameObject>();
    }

    void GameOver() {
        gameOver = true;
        explosionSound.Play();
        explosionPrefab.Play();
        if (gameOverUI != null) {
            gameOverUI.Show();
        }
    }

    private void OnTriggerEnter(Collider other) {
        // Enemy collision
        if (other.gameObject.CompareTag("Enemy")) {
            Debug.Log("Game Over " + "Collided with: " + other.gameObject.name);
            GameOver();
        }

        // Player trail collision
        if (other.gameObject.CompareTag("PlayerTrail") && !other.gameObject.Equals(trailColliders[trailColliders.Count - 1])) {
            Debug.Log("Game Over " + "Collided with: " + other.gameObject.name);
            GameOver();
        }

        // Back to ground
        if (other.gameObject.CompareTag("Ground")) {
            onGround = true;
            lastDirection = Vector3.zero;
            if (trailColliders.Count > 0 && areaManager != null) {
                List<Vector3> positions = new List<Vector3>();
                foreach (var t in trailColliders) {
                    positions.Add(t.transform.position);
                }
                areaManager.FillTrailArea(positions);
            }
            KillTrailColliders();
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Ground")) {
            onGround = true;
            lastDirection = Vector3.zero;
            if (trailColliders.Count > 0) {
                if (areaManager != null) {
                    List<Vector3> positions = new List<Vector3>();
                    foreach (var t in trailColliders) {
                        positions.Add(t.transform.position);
                    }
                    areaManager.FillTrailArea(positions);
                }
                KillTrailColliders();
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Ground")) {
            onGround = false;
            // CreateTrailCollider();
        }
    }
}

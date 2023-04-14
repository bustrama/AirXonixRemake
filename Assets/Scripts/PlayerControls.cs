using UnityEngine;

public class PlayerControls :MonoBehaviour {
    [SerializeField] private float moveSpeed = 20.0f;

    [SerializeField] private bool onGround = false;
    [SerializeField] private float horizontalInput;
    [SerializeField] private float verticalInput;
    [SerializeField] private bool gameOver = false;
    private Vector3 lastDirection;

    private GameObject arena;
    private float maxDistance;

    // Start is called before the first frame update
    void Start() {
        arena = GameObject.Find("arena");
        maxDistance = 5 * arena.transform.localScale.x - 1;
        transform.position = new Vector3(0, 2, -maxDistance);
    }

    // Update is called once per frame
    void Update() {
        // get the user's vertical input
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");


        if (onGround) {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime * -verticalInput);
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime * horizontalInput);
        } else {
            if (Mathf.Abs(verticalInput) > Mathf.Abs(horizontalInput)) {
                float direction = verticalInput > 0 ? 1 : -1;
                transform.Translate(Vector3.up * moveSpeed * Time.deltaTime * -direction);
                lastDirection = verticalInput < 0 ? Vector3.up : Vector3.down;
            } else if (Mathf.Abs(horizontalInput) > Mathf.Abs(verticalInput)) {
                float direction = horizontalInput > 0 ? 1 : -1;
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime * direction);
                lastDirection = horizontalInput > 0 ? Vector3.right : Vector3.left;
            } else {
                transform.Translate(lastDirection * moveSpeed * Time.deltaTime);
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

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Enemy")) {
            Debug.Log("Game Over");
            gameOver = true;
        }

        if (other.gameObject.CompareTag("Ground")) {
            onGround = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Ground")) {
            onGround = false;
        }
    }
}

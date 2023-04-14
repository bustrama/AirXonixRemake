using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls :MonoBehaviour {
    public GameObject arena;
    public float moveSpeed = 20.0f;

    public bool onGround = false;
    public float horizontalInput;
    public float verticalInput;
    public bool gameOver = false;

    private float maxWidth;
    private float maxDepth;

    // Start is called before the first frame update
    void Start() {
        BoxCollider arenaBox = arena.GetComponent<BoxCollider>();
        maxDepth = arenaBox.size.z * 2.4f;
        maxWidth = arenaBox.size.x * 2.4f;
        transform.position = new Vector3(0, 2, -maxDepth);
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
                transform.Translate(Vector3.up * moveSpeed * Time.deltaTime * -verticalInput);
            } else if (Mathf.Abs(horizontalInput) > Mathf.Abs(verticalInput)) {
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime * horizontalInput);
            }
        }

        if (transform.position.x > maxDepth) {
            transform.position = new Vector3(maxDepth, transform.position.y, transform.position.z);
        }
        if (transform.position.x < -maxDepth) {
            transform.position = new Vector3(-maxDepth, transform.position.y, transform.position.z);
        }
        if (transform.position.z > maxWidth) {
            transform.position = new Vector3(transform.position.x, transform.position.y, maxWidth);
        }
        if (transform.position.z < -maxWidth) {
            transform.position = new Vector3(transform.position.x, transform.position.y, -maxWidth);
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

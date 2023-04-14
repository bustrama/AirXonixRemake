using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy :MonoBehaviour {
    public float speed;
    public bool xAxis = true;
    public bool zAxis = true;

    private int xDirection = 1;
    private int zDirection = 1;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        transform.Translate(new Vector3(xAxis ? 1 * xDirection : 0, 0, zAxis ? 1 * zDirection : 0) * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Ground")) {
            if (Mathf.Abs(transform.position.x) > Mathf.Abs(transform.position.z)) {
                xDirection *= -1;
            } else {
                zDirection *= -1;
            }
        }
    }
}
 
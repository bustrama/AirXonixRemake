using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin :MonoBehaviour {
    public GameObject objectToSpin;
    public float spinSpeed = 50.0f;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        objectToSpin.transform.Rotate(Vector3.forward * Time.deltaTime * spinSpeed);
    }
}

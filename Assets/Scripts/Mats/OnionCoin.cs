using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnionCoin : MonoBehaviour {
    private Vector3 target;
    private float   rotSpeed;
    private float   speed;
    private float   scaleSpeed;
    private float   actualRot = 0;

    void Start() {
        target = transform.position + new Vector3(0, 1, 0);
        speed = 1 / (Time.fixedDeltaTime / 2);
        rotSpeed = 359 / (Time.fixedDeltaTime / 2);
        scaleSpeed = transform.localScale.y / (Time.fixedDeltaTime / 2);
    }

    void Update() {
        if (actualRot >= 360) {
            GameObject.Destroy(gameObject);
        }
        if ((target - transform.position).magnitude < 0.05f) {
            transform.Rotate(0, Time.deltaTime * rotSpeed, 0, Space.World);
            transform.localScale -= new Vector3(Time.deltaTime * scaleSpeed ,Time.deltaTime * scaleSpeed, Time.deltaTime * scaleSpeed);
            actualRot += Time.deltaTime * rotSpeed;
        } else {
            transform.position += new Vector3(0, Time.deltaTime * speed);
        }
    }
}

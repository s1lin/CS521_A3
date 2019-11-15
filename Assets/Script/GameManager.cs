using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private Transform parent;
    void Start() {
        parent = GameObject.FindGameObjectWithTag("thief").transform;
        parent.transform.position = RandomLocation();
    }

    Vector3 RandomLocation() {
        float x = Random.Range(-15f, 16.7f);
        float y = Random.Range(-13.34f, 11.55f);

        if (y < 5.42f && y > -5.4f) {
            while (x > -3.17f && x < 4.59f) {
                x = Random.Range(-15f, 16.7f);
            }
        }

        return new Vector3(x, y, -1f);
    }
}

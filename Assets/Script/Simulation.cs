using System;
using UnityEngine;
using UnityEngine.UI;

public class Simulation : MonoBehaviour {

    private float speed = 1f;
    private float fps = 120f;

    private Text tFps;
    private Text tSpeed;

    private void Start() {
        Application.targetFrameRate = (int)fps;
        tFps = GameObject.FindGameObjectWithTag("fps").GetComponent<Text>();
        tSpeed = GameObject.FindGameObjectWithTag("speed").GetComponent<Text>();
        tSpeed.text = string.Format("{0}", 1);
    }

    private void Update() {

        tFps.text = string.Format("{0}", fps);

        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.Equals)) {
            speed *= 2f;
            fps /= speed;
            if (fps < 1f) {
                fps = 1;
                speed /= 2f;
                tSpeed.text = string.Format("{0}", speed + " Maximum accelration");
            } else {
                tSpeed.text = string.Format("{0}", speed);
                Application.targetFrameRate = (int)fps;
                tFps.text = string.Format("{0}", Application.targetFrameRate);
            }
           
        }

        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) {
            fps *= speed;
            speed /= 2f;            
            tSpeed.text = string.Format("{0}", speed);
            Application.targetFrameRate = (int)fps;
            tFps.text = string.Format("{0}", Application.targetFrameRate);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour {

    [SerializeField]
    [Range(.1f, 4)]
    float period = 1f;

    [SerializeField]
    Vector3 movementDirection;

    Vector3 startingPosition;
    Vector3 offset;

    // Use this for initialization
    void Start () {
        //UpdateOffset();
        startingPosition = transform.position;
    }

    private void UpdateOffset() {
        float movementSpeed = Mathf.Sin(Time.time / Mathf.Max(0.1f, period));
        offset = movementDirection * movementSpeed;
    }

    private void UpdateDirection() {
        
    }

    // Update is called once per frame
    void Update () {
        UpdateObjectPosition();
    }

    private void UpdateObjectPosition() {
        UpdateOffset();
        transform.position = startingPosition + offset;
    }
}
